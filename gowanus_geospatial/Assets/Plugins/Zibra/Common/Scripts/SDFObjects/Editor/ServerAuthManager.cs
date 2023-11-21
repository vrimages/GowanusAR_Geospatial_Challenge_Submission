#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

namespace com.zibra.common.Editor.SDFObjects
{
    /// <summary>
    ///     Class responsible for managing licensing and allowing server communication.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [InitializeOnLoad]
    public class ServerAuthManager
    {
#region Public Interface

        /// <summary>
        ///     Status of license validation.
        /// </summary>
        public enum Status
        {
            NotInitialized = 0,
            OK,
            KeyValidationInProgress,
            NetworkError,
            InvalidKey,
            NotRegistered,
#if !ZIBRA_EFFECTS_OTP_VERSION
            NoMaintance,
            Expired
#endif
        }

        /// <summary>
        ///     License key used for the plugin.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Not necessarily correct.
        ///         Use <see cref="IsLicenseVerified"/> to check for that.
        ///     </para>
        ///     <para>
        ///         May be empty.
        ///     </para>
        /// </remarks>
        public string[] PluginLicenseKeys { get; private set; } = new string[0];

        /// <summary>
        ///     License key used for the generation.
        /// </summary>
        /// <remarks>
        ///     It's either validated key suitable for generation or an empty string.
        /// </remarks>
        public string GenerationLicenseKey { get; private set; } = "";

        /// <summary>
        ///     URL address for generation API.
        /// </summary>
        /// <remarks>
        ///     Includes <see cref="GenerationLicenseKey"/> in the URL.
        /// </remarks>
        public string GenerationURL { get; private set; } = "";

        /// <summary>
        ///     Returns current status of license validation for specified effect.
        /// </summary>
        /// <remarks>
        ///     Never returns NotInitialized, since it initialized validation in this case.
        /// </remarks>
        public Status GetStatus(PluginManager.Effect effect)
        {
            return CurrentStatus[(int)effect];
        }

        private void SetStatus(Status newStatus, PluginManager.Effect effect)
        {
            SessionState.SetInt(STATUS_SESSION_KEY + effect, (int)newStatus);
            CurrentStatus[(int)effect] = newStatus;
        }

        private void SetStatusGlobal(Status newStatus)
        {
            for (int j = 0; j < (int)PluginManager.Effect.Count; ++j)
            {
                SetStatus(newStatus, (PluginManager.Effect)j);
            }
        }

        /// <summary>
        ///     Checks whether license is verified for specified effect.
        /// </summary>
        /// <returns>
        ///     True if license is valid, false otherwise.
        /// </returns>
        public bool IsLicenseVerified(PluginManager.Effect effect)
        {
            switch (GetStatus(effect))
            {
            case Status.OK:
#if !ZIBRA_EFFECTS_OTP_VERSION
                case Status.NoMaintance:
#endif
                return true;
            default:
                return false;
            }
        }

        /// <summary>
        ///     Checks whether specified string is correctly formated list of keys.
        /// </summary>
        public static bool CheckKeysFormat(string keys)
        {
            if (keys.Trim().Length == 0)
                return false;

            foreach (string key in keys.Split(','))
            {
                if (!Regex.IsMatch(key.Trim(), KEY_FORMAT_REGEX))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Checks whether generation is available.
        /// </summary>
        /// <returns>
        ///     True if license is valid and not expired, false otherwise.
        /// </returns>
        public bool IsGenerationAvailable()
        {
            return GenerationLicenseKey != "";
        }

        /// <summary>
        ///     Human readable error message string returned from server.
        /// </summary>
        public string ServerErrorMessage { get; private set; } = "";

        /// <summary>
        ///     Returns human readable string explaining error based on <see cref="Status"/>.
        /// </summary>
        public string GetErrorMessage(Status status)
        {
            switch (status)
            {
            case Status.KeyValidationInProgress:
                return "License key validation in progress. Please wait.";
            case Status.NetworkError:
                return "Network error. Please ensure you are connected to the internet and try again.";
            case Status.InvalidKey:
                return ServerErrorMessage;
#if !ZIBRA_EFFECTS_OTP_VERSION
            case Status.Expired:
                return "License expired.";
#endif
            case Status.NotRegistered:
                return "Plugin is not registered.";
            default:
                return "";
            }
        }

        /// <summary>
        ///     Returns human readable string explaining current error with activation of specified effect.
        /// </summary>
        public string GetErrorMessage(PluginManager.Effect effect)
        {
            return GetErrorMessage(GetStatus(effect));
        }

        /// <summary>
        ///     Returns human readable string explaining current error with generation availability.
        /// </summary>
        public string GetGenerationErrorMessage()
        {
            if (GenerationLicenseKey != "")
            {
                return "";
            }

            int highestPriorityError = (int)Status.NotRegistered;
            for (int i = 0; i < (int)PluginManager.Effect.Count; ++i)
            {
                highestPriorityError = Math.Min(highestPriorityError, (int)CurrentStatus[i]);
            }
            return GetErrorMessage((Status)highestPriorityError);
        }

        /// <summary>
        ///     Returns singleton of this class.
        /// </summary>
        /// <remarks>
        ///     Creates and initializes instance if needed.
        /// </remarks>
        public static ServerAuthManager GetInstance()
        {
            return Instance;
        }

        /// <summary>
        ///     Sets license key and starts key validation process.
        /// </summary>
        public void RegisterKey(string key)
        {
            ValidateLicense(key.Split(','));
        }

        /// <summary>
        ///     Sets list of license keys and starts key validation process.
        /// </summary>
        public void ValidateLicense(string[] keys)
        {
            if (keys.Length == 0)
            {
                SetStatusGlobal(Status.NotRegistered);
                return;
            }

            SetLicenceKeys(keys);

            LicenseKeyRequest licenseKeyRequest = new LicenseKeyRequest();
            licenseKeyRequest.license_keys = keys;
            licenseKeyRequest.random_numbers = new RandomNumberDeclaration[PluginManager.AvailableCount()];
            licenseKeyRequest.hardware_id = HardwareID;
            licenseKeyRequest.engine = "unity";

            int j = 0;
            for (int i = 0; i < (int)PluginManager.Effect.Count; ++i)
            {
                PluginManager.Effect effect = (PluginManager.Effect)i;
                if (!PluginManager.IsAvailable(effect))
                {
                    continue;
                }
                licenseKeyRequest.random_numbers[j] = new RandomNumberDeclaration();
                var currentRandomNumberDeclaration = licenseKeyRequest.random_numbers[j];
                currentRandomNumberDeclaration.product = PluginManager.GetEffectName(effect);
                currentRandomNumberDeclaration.number = PluginManager.LicensingGetRandomNumber(effect).ToString();
                ++j;
            }

            string json = JsonUtility.ToJson(licenseKeyRequest);

            if (Request != null)
            {
                Request.Dispose();
            }

#if UNITY_2022_2_OR_NEWER
            Request = UnityWebRequest.PostWwwForm(LICENSE_VALIDATION_URL, json);
#else
            Request = UnityWebRequest.Post(LICENSE_VALIDATION_URL, json);
#endif
            RequestOperation = Request.SendWebRequest();
            RequestOperation.completed += UpdateLicenseRequest;
            SetStatusGlobal(Status.KeyValidationInProgress);
        }

        /// <summary>
        ///     Removes current license key.
        /// </summary>
        ///
        public void RemoveKey()
        {
            EditorPrefs.DeleteKey(LICENSE_KEYS_PREF_KEY);
            SessionState.EraseString(GENERATION_KEY_SESSION_KEY);
            PluginLicenseKeys = new string[0];
            GenerationLicenseKey = "";
            SetStatusGlobal(Status.NotRegistered);
#if ZIBRA_EFFECTS_OTP_VERSION
            RegistrationManager.Reset();
#endif
        }

#endregion
#region Implementation details
        /// <summary>
        ///     Restores state after domain reload
        /// </summary>
        /// <returns>
        ///     true if we need to retry license validation.
        /// </returns>
        private bool RestoreSessionState()
        {
            bool needValidation = true;
            for (int i = 0; i < (int)PluginManager.Effect.Count; ++i)
            {
                PluginManager.Effect effect = (PluginManager.Effect)i;
                if (!PluginManager.IsAvailable(effect))
                {
                    continue;
                }

                Status restoredStatus = (Status)SessionState.GetInt(STATUS_SESSION_KEY + effect, 0);
                switch (restoredStatus)
                {
                case Status.NotInitialized:
                case Status.KeyValidationInProgress:
                case Status.NetworkError:
                    restoredStatus = Status.NotInitialized;
                    break;
                case Status.OK:
                    needValidation = false;
                    break;
                }
                CurrentStatus[i] = restoredStatus;
            }
            GenerationLicenseKey = SessionState.GetString(GENERATION_KEY_SESSION_KEY, "");
            return needValidation;
        }

        private ServerAuthManager()
        {
            HardwareID = PluginManager.GetHardwareID();
            PluginLicenseKeys = GetEditorPrefsLicenseKey();

            bool needValidation = RestoreSessionState();
            if (!needValidation)
            {
                GenerationURL = CreateGenerationRequestURL("compute");
                return;
            }

            ValidateLicense(PluginLicenseKeys);
        }

        ~ServerAuthManager()
        {
            if (Request != null)
            {
                Request.Dispose();
            }
        }

        static ServerAuthManager()
        {
            Instance = new ServerAuthManager();
        }

        private const string BASE_URL = "https://generation.zibra.ai/";
        private const string LICENSE_VALIDATION_URL = "https://license.zibra.ai/api/licenseExpiration";
        private const string VERSION_DATE = "2023.05.22";

#if ZIBRA_EFFECTS_OTP_VERSION
        private const string LICENSE_KEYS_PREF_KEY = "ZibraEffectsLicenceKeyOTP";
#else
        private const string LICENSE_KEYS_PREF_KEY = "ZibraEffectsLicenceKey";
#endif
        private const string LICENSE_KEYS_OLD_PREF_KEY = "ZibraLiquidsLicenceKey";
        private const string STATUS_SESSION_KEY = "ZibraEffectsLicenseStatus";
        private const string GENERATION_KEY_SESSION_KEY = "ZibraEffectsGenerationKey";
        private const string KEY_FORMAT_REGEX =
            "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";
        private string HardwareID = "";
        private UnityWebRequestAsyncOperation RequestOperation;
        private UnityWebRequest Request;

        private Status[] CurrentStatus = new Status[(int)PluginManager.Effect.Count];

        private static ServerAuthManager Instance = null;

        internal delegate void OnProLicenseWarningCallback(string headerText, string bodyText, string url,
                                                           string buttonText);
        internal static OnProLicenseWarningCallback OnProLicenseWarning;

        private string[] GetEditorPrefsLicenseKey()
        {
            if (EditorPrefs.HasKey(LICENSE_KEYS_PREF_KEY))
            {
                string pref_keys = EditorPrefs.GetString(LICENSE_KEYS_PREF_KEY);
                return pref_keys.Split(',');
            }

            if (EditorPrefs.HasKey(LICENSE_KEYS_OLD_PREF_KEY))
            {
                string pref_keys = EditorPrefs.GetString(LICENSE_KEYS_OLD_PREF_KEY);
                EditorPrefs.SetString(LICENSE_KEYS_PREF_KEY, pref_keys);
                return pref_keys.Split(',');
            }

            return new string[0];
        }

        // C# doesn't know we use it with JSON deserialization
#pragma warning disable 0649
        /// @cond SHOW_INTERNAL_JSON_FIELDS
        [Serializable]
        public class RandomNumberDeclaration
        {
            public string product;
            public string number;
        }

        [Serializable]
        class LicenseKeyRequest
        {
            public string api_version = "2023.05.17";
            public string[] license_keys;
            public RandomNumberDeclaration[] random_numbers;
            public string hardware_id;
            public string engine;
        }

        [Serializable]
        class LicenseKeyResponse
        {
            public string license_info;
            public string signature;
        }

        // LicenseWarning needs to be public for JSON deserialization
        // But it is not intended to be used by end users
        [Serializable]
        public class LicenseWarning
        {
            public string header_text;
            public string body_text;
            public string button_text;
            public string URL;
        }

        [Serializable]
        class LicenseInfo
        {
            public string license_key;
            public string license;
            public string latest_version;
            public string random_number;
            public string hardware_id;
            public string engine;
            public string product;
            public string message;
            public LicenseWarning warning;
        }

        // Unity's built-in JSON parser can't process top level arrays
        // So we need to wrap them into another object and wrap json string into object
        [Serializable]
        class LicenseInfoWrapper
        {
            public LicenseInfo[] items;
        }

        [Serializable]
        class ErrorInfo
        {
            public string license_info;
        }

        /// @endcond
#pragma warning restore 0649

        private void ProcessServerResponse(string response)
        {
            LicenseKeyResponse parsedResponse = JsonUtility.FromJson<LicenseKeyResponse>(response);
            if (parsedResponse.signature == null || parsedResponse.license_info == null)
            {
                SetStatusGlobal(Status.InvalidKey);
                return;
            }

            try
            {
                ErrorInfo errorInfo = JsonUtility.FromJson<ErrorInfo>(parsedResponse.license_info);
                if (errorInfo != null)
                {
                    SetStatusGlobal(Status.InvalidKey);
                    ServerErrorMessage = errorInfo.license_info;
                    return;
                }
            }
            catch (Exception)
            {
                // No errors reported
            }

            LicenseInfoWrapper licenseInfoWrapper =
                JsonUtility.FromJson<LicenseInfoWrapper>("{\"items\":" + parsedResponse.license_info + "}");
            LicenseInfo[] licenseInfos = licenseInfoWrapper.items;

            bool activatedPlugin = false;

            if (licenseInfos == null)
            {
                SetStatusGlobal(Status.InvalidKey);
                return;
            }

            foreach (LicenseInfo info in licenseInfos)
            {
                PluginManager.Effect effect = PluginManager.ParseEffectName(info.product);

                if (effect >= PluginManager.Effect.Count)
                {
                    continue;
                }

                Debug.Log($"Zibra Effects {effect} License Info: {info.message}");

                // Unity's JsonUtility may create empty, non-null licenseInfo.warning
                // Need to check whether we have at least some data in licenseInfo.warning
                if (info.warning != null && info.warning.header_text != null)
                {
                    LicenseWarning warning = info.warning;
                    Debug.LogWarning(warning.header_text + "\n" + warning.body_text);
                    OnProLicenseWarning(warning.header_text, warning.body_text, warning.URL, warning.button_text);
                }
                switch (info.license)
                {
                case "ok":
                    SetStatus(Status.OK, effect);
                    GenerationLicenseKey = info.license_key;
                    break;
#if !ZIBRA_EFFECTS_OTP_VERSION
                case "old_version_only":
                    int comparison = String.Compare(info.latest_version, VERSION_DATE, StringComparison.Ordinal);
                    if (comparison < 0)
                    {
                        SetStatus(Status.Expired, effect);
                        continue;
                    }
                    SetStatus(Status.NoMaintance, effect);
                    break;
                case "expired":
                    SetStatus(Status.Expired, effect);
                    continue;
#endif
                default:
                    SetStatus(Status.InvalidKey, effect);
                    continue;
                }

                activatedPlugin = true;
                PluginManager.ValidateLicense(effect, response);
            }

            if (!activatedPlugin)
            {
                return;
            }

#if !ZIBRA_EFFECTS_OTP_VERSION
            UnityEditor.VSAttribution.ZibraAI.VSAttribution.SendAttributionEvent("ZibraEffects_Login", "ZibraAI",
                                                                                 String.Join(",", PluginLicenseKeys));
#endif
            // populate server request URL if everything is fine
            SessionState.SetString(GENERATION_KEY_SESSION_KEY, GenerationLicenseKey);
            GenerationURL = CreateGenerationRequestURL("compute");
        }

        private void UpdateLicenseRequest(AsyncOperation obj)
        {
            if (RequestOperation == null)
            {
                return;
            }

            if (RequestOperation.isDone)
            {
                var result = RequestOperation.webRequest.downloadHandler.text;
                if (result != null && RequestOperation.webRequest.result == UnityWebRequest.Result.Success)
                {
                    ProcessServerResponse(result);
                }
                else if (RequestOperation.webRequest.result != UnityWebRequest.Result.Success)
                {
                    SetStatusGlobal(Status.NetworkError);
                    ServerErrorMessage = RequestOperation.webRequest.downloadHandler.text;
                    if (ServerErrorMessage == null || ServerErrorMessage == "")
                    {
                        ServerErrorMessage = RequestOperation.webRequest.error;
                    }
                    Debug.LogError("Zibra Effects License Key validation error: " + RequestOperation.webRequest.error + "\n" +
                        RequestOperation.webRequest.downloadHandler.text);
                }
                RequestOperation = null;
                Request.Dispose();
                Request = null;
            }
            return;
        }

        private void SetLicenceKeys(string[] keys)
        {
            PluginLicenseKeys = keys;
            EditorPrefs.SetString(LICENSE_KEYS_PREF_KEY, String.Join(",", keys));
        }

        private string CreateGenerationRequestURL(string type)
        {
            string generationURL;

            generationURL = BASE_URL + "api/unity/" + type + "?";

            if (HardwareID != "")
            {
                generationURL += "&hardware_id=" + HardwareID;
            }

            if (GenerationLicenseKey != "")
            {
                generationURL += "&api_key=" + GenerationLicenseKey;
            }

            return generationURL;
        }
#endregion
    }
}

#endif
