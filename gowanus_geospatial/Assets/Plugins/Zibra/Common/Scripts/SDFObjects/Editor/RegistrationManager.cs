#if UNITY_EDITOR && ZIBRA_EFFECTS_OTP_VERSION

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace com.zibra.common.Editor.SDFObjects
{
    /// <summary>
    ///     Class responsible for managing registration of OTP version.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [InitializeOnLoad]
    class RegistrationManager
    {
        /// <summary>
        ///     Possible registration statuses.
        /// </summary>
        public enum Status
        {
            NotRegistered,
            InProgress,
            NetworkError,
            InvalidData,
            OK
        }

        /// <summary>
        ///     Current registration status.
        /// </summary>
        public Status CurrentStatus { get; private set; } = Status.NotRegistered;
        public static RegistrationManager Instance { get; private set; } = new RegistrationManager();
        public bool IsFirstRegistration { get; private set; } = false;
        public RegistrationManager()
        {
        }

        public static void Reset()
        {
            Instance = new RegistrationManager();
        }

        /// <summary>
        ///     Human readable error description.
        /// </summary>
        public string ErrorMessage { get; private set; } = "";

        /// <summary>
        ///     Method that tries to register the plugin.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Does nothing if license is already validated.
        ///     </para>
        ///     <para>
        ///         Will activate newly registered key on success.
        ///     </para>
        /// </remarks>
        public void Register(string OrderNumber, string Email, string Name)
        {
            if (ServerAuthManager.GetInstance().IsGenerationAvailable())
            {
                CurrentStatus = Status.OK;
                return;
            }

            RegistrationData data = new RegistrationData();
            data.order = OrderNumber;
            data.email = Email;
            data.name = Name;

            string json = JsonUtility.ToJson(data);

            if (Request != null)
            {
                Request.Dispose();
            }

#if UNITY_2022_2_OR_NEWER
            Request = UnityWebRequest.PostWwwForm(URL, json);
#else
            Request = UnityWebRequest.Post(URL, json);
#endif

            CurrentStatus = Status.InProgress;
            ErrorMessage = "Registration in progress";
            RequestOperation = Request.SendWebRequest();
            RequestOperation.completed += UpdateRequest;
        }

        private string URL = "https://generation.zibra.ai/api/apiKey?type=registration";
        [Serializable]
        private struct RegistrationData
        {
            public string email;
            public string name;
            public string order;
        }

        [Serializable]
        private struct RegistrationResponse
        {
            public string[] api_keys;
            public bool is_first_registration;
        }

        private UnityWebRequest Request;
        private UnityWebRequestAsyncOperation RequestOperation;

        ~RegistrationManager()
        {
            if (Request != null)
            {
                Request.Dispose();
            }
        }

        private void UpdateRequest(AsyncOperation obj)
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
                    CurrentStatus = RequestOperation.webRequest.result == UnityWebRequest.Result.ProtocolError ? Status.InvalidData : Status.NetworkError;
                    ErrorMessage = RequestOperation.webRequest.downloadHandler.text;
                    if (ErrorMessage == null || ErrorMessage == "")
                    {
                        ErrorMessage = "Network error. Please ensure you are connected to the Internet and try again.";
                    }
                    Debug.LogError("Zibra Registration error: " + RequestOperation.webRequest.error + "\n" +
                                   RequestOperation.webRequest.downloadHandler.text);
                }
                RequestOperation = null;
                Request.Dispose();
                Request = null;
            }
        }

        private void ProcessServerResponse(string response)
        {
            RegistrationResponse parsedResponse = JsonUtility.FromJson<RegistrationResponse>(response);
            if (parsedResponse.api_keys != null && parsedResponse.api_keys.Length > 0)
            {
                IsFirstRegistration = parsedResponse.is_first_registration;
                CurrentStatus = Status.OK;
                ErrorMessage = "";
                ServerAuthManager.GetInstance().ValidateLicense(parsedResponse.api_keys);
            }
            else
            {
                CurrentStatus = Status.InvalidData;
                ErrorMessage = "Invalid data provided.";
            }
        }
    }
}

#endif
