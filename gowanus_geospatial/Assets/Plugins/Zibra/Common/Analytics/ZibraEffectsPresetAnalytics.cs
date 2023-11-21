#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using com.zibra.common.Editor.SDFObjects;
using com.zibra.common.Editor;

namespace com.zibra.common.PresetAnalytics
{
    [InitializeOnLoad]
    internal class ZibraEffectsPresetAnalytics : MonoBehaviour
    {
        public static void SendPresetAnalyticsData(string[] presetNames)
        {
            ZibraEffectsPresetAnalyticsStruct data = GetPresetAnalyticsData(presetNames);

            if (data.Preset_Names.Length == 0)
                return;

            string jsonData = JsonUtility.ToJson(data);
            ZibraEffectsPresetAnalyticsSender.SendAnalyticsData(jsonData);
        }

        private struct ZibraEffectsPresetAnalyticsStruct
        {
            public string LicenseKeys;
            public string HardwareId;
            public string PluginSKU;
            public bool Liquids_Available;
            public bool SmokeAndFire_Available;
            public string[] Preset_Names;
        }

        private static ZibraEffectsPresetAnalyticsStruct GetPresetAnalyticsData(string[] presetNames)
        {
            ZibraEffectsPresetAnalyticsStruct data = new ZibraEffectsPresetAnalyticsStruct();

            data.LicenseKeys = String.Join(",", ServerAuthManager.GetInstance().PluginLicenseKeys);
            data.HardwareId = PluginManager.GetHardwareID();
#if ZIBRA_EFFECTS_OTP_VERSION
            data.PluginSKU = "OTP";
#else
            data.PluginSKU = "Pro";
#endif
            data.Liquids_Available = PluginManager.IsAvailable(PluginManager.Effect.Liquid);
            data.SmokeAndFire_Available = PluginManager.IsAvailable(PluginManager.Effect.Smoke);
            data.Preset_Names = FilterPresetNames(presetNames);

            return data;
        }

        private static string[] FilterPresetNames(string[] presetNames)
        {
            int countNonEmptyStrings = 0;
            foreach (string preset in presetNames)
            {
                if (preset != "")
                    countNonEmptyStrings++;
            }

            string[] filteredPresetNames = new string[countNonEmptyStrings];
            int arrayIndex = 0;
            foreach (string preset in presetNames)
            {
                if (preset != "")
                {
                    filteredPresetNames[arrayIndex] = preset;
                    arrayIndex++;
                }
            }

            return filteredPresetNames;
        }

    }

    internal static class ZibraEffectsPresetAnalyticsSender
    {
        const string ANALYTIC_API_URL = "https://analytics.zibra.ai/api/assetsAnalytics";
        const int SUCCESS_CODE = 201;

        private static UnityWebRequestAsyncOperation request;

        public static void SendAnalyticsData(string jsonData)
        {
            if (request != null)
            {
                return;
            }
#if UNITY_2022_2_OR_NEWER
            request = UnityWebRequest.PostWwwForm(ANALYTIC_API_URL, jsonData).SendWebRequest();
#else
            request = UnityWebRequest.Post(ANALYTIC_API_URL, jsonData).SendWebRequest();
#endif
            request.completed += UpdateRequest;
        }

        private static void UpdateRequest(AsyncOperation obj)
        {
            if (request == null || !request.isDone)
            {
                return;
            }

            request.webRequest.Dispose();
            request = null;
        }
    }
}
#endif
