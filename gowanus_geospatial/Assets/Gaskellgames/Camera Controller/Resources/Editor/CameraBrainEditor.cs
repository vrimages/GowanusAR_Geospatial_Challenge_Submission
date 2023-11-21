#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController
{
    [CustomEditor(typeof(CameraBrain))] [CanEditMultipleObjects]
    public class CameraBrainEditor : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty activeCamera;
        SerializedProperty previousCamera;

        SerializedProperty blendingStyle;
        SerializedProperty fadeCurve;
        SerializedProperty fadeColor;
        SerializedProperty fadeSpeed;
        SerializedProperty fadeFullScreen;
        SerializedProperty canvasGroup;

        SerializedProperty follow;
        SerializedProperty lookAt;
        SerializedProperty lens;
        SerializedProperty CameraOrbit;

        bool InfoGroup = true;
        bool ShowFadeGroup = true;
        bool FadeGroup = true;
        bool OrbitsGroup = true;

        private void OnEnable()
        {
            activeCamera = serializedObject.FindProperty("activeCamera");
            previousCamera = serializedObject.FindProperty("previousCamera");

            blendingStyle = serializedObject.FindProperty("blendingStyle");
            fadeCurve = serializedObject.FindProperty("fadeCurve");
            fadeColor = serializedObject.FindProperty("fadeColor");
            fadeSpeed = serializedObject.FindProperty("fadeSpeed");
            fadeFullScreen = serializedObject.FindProperty("fadeFullScreen");
            canvasGroup = serializedObject.FindProperty("canvasGroup");

            follow = serializedObject.FindProperty("follow");
            lookAt = serializedObject.FindProperty("lookAt");
            lens = serializedObject.FindProperty("lens");
            CameraOrbit = serializedObject.FindProperty("CameraOrbit");
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            CameraBrain cameraBrain = (CameraBrain)target;
            serializedObject.Update();

            /*
            // draw default inspector
            base.OnInspectorGUI();
            */

            // banner
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Camera Controller/Resources/Icons/inspectorBanner_CameraController.png", typeof(Texture));
            GUILayout.Box(banner, GUILayout.ExpandWidth(true), GUILayout.Height(Screen.width / 7.5f));

            // custom inspector
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(activeCamera);
            EditorGUILayout.PropertyField(previousCamera);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(blendingStyle);
            string value = cameraBrain.GetBlendingStyle();
            if (value == "FadeToColor") { ShowFadeGroup = true; } else { ShowFadeGroup = false; }
            if(ShowFadeGroup)
            {
                FadeGroup = EditorGUILayout.BeginFoldoutHeaderGroup(FadeGroup, "Fade Settings");
                if (FadeGroup)
                {
                    GUI.contentColor = Color.white;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(fadeCurve);
                    EditorGUILayout.PropertyField(fadeSpeed);
                    EditorGUILayout.PropertyField(fadeFullScreen);
                    if(!fadeFullScreen.boolValue)
                    {
                        EditorGUILayout.PropertyField(canvasGroup);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(fadeColor);
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            EditorGUILayout.Space();
            GUI.contentColor = Color.gray;
            CameraRig rig = cameraBrain.GetActiveCamera();
            InfoGroup = EditorGUILayout.BeginFoldoutHeaderGroup(InfoGroup, "Active CameraRig");
            if (InfoGroup)
            {
                GUI.contentColor = Color.white;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(follow);
                EditorGUILayout.PropertyField(lookAt);
                EditorGUILayout.PropertyField(lens);
                if (rig != null)
                {
                    CameraFreelookRig FreelookRig = rig.GetFreelookRig();
                    if (FreelookRig != null)
                    {
                        EditorGUILayout.PropertyField(CameraOrbit);
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (rig == null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("No Active Camera Assigned", MessageType.Warning);
            }

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

#endif
