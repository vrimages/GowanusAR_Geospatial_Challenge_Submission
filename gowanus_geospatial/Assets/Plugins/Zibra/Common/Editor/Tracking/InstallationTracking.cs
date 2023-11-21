using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using com.zibra.common.Editor.SDFObjects;

namespace com.zibra.common.Editor
{
    internal static class InstallationTracking
    {
#if !ZIBRA_EFFECTS_OTP_VERSION
        const string IS_TRACKED_PREFS_KEY = "ZibraEffectsIsInstallationTracked";
#else
        const string IS_TRACKED_PREFS_KEY_TEMPLATE = "ZibraEffectsOTPIsInstallationTracked_";
#endif
        const string BASE_URL = "https://license.zibra.ai/api/installationTracking";
        const int SUCCESS_CODE = 200;

        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            // Don't track something that is potentially build-machine
            if (Application.isBatchMode)
            {
                return;
            }

#if !ZIBRA_EFFECTS_OTP_VERSION
            if (!EditorPrefs.GetBool(IS_TRACKED_PREFS_KEY, false))
            {
                ReportInstallation("effects");
            }
#else
            PluginManager.Effect[] effects = new PluginManager.Effect[] { PluginManager.Effect.Liquid, PluginManager.Effect.Smoke };
            foreach (var effect in effects)
            {
                string effectName = PluginManager.GetEffectName(effect);

                if (!EditorPrefs.GetBool(IS_TRACKED_PREFS_KEY_TEMPLATE + effectName, false))
                {
                    ReportInstallation(effectName);
                }
            }
#endif
        }

        static void ReportInstallation(string effect)
        {
            string URL = $"{BASE_URL}?product={effect}&hardware_id={PluginManager.GetHardwareID()}";

            UnityWebRequestAsyncOperation Request;
            Request = UnityWebRequest.Get(URL).SendWebRequest();
            Request.completed += (operation) =>
            {
                UpdateRequest(effect, operation);
            };
        }

        static void UpdateRequest(string effect, AsyncOperation obj)
        {
            UnityWebRequestAsyncOperation Request = obj as UnityWebRequestAsyncOperation;
            if (Request == null || !Request.isDone)
            {
                return;
            }

            if (Request.webRequest.result != UnityWebRequest.Result.Success)
            {
                Request.webRequest.Dispose();
                Request = null;
                return;
            }

            if (Request.webRequest.responseCode != SUCCESS_CODE)
            {
                Request.webRequest.Dispose();
                Request = null;
                return;
            }

#if !ZIBRA_EFFECTS_OTP_VERSION
            EditorPrefs.SetBool(IS_TRACKED_PREFS_KEY, true);
#else
            EditorPrefs.SetBool(IS_TRACKED_PREFS_KEY_TEMPLATE + effect, true);
#endif
            Request.webRequest.Dispose();
            Request = null;
        }
    }
}
