#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.InputEventSystem
{
    [CustomEditor(typeof(PlayerInputController))]
    public class PlayerInputControllerEditor : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty legacyInputSystem;
        SerializedProperty legacyInputs;
        SerializedProperty playerInput;
        SerializedProperty info;

        bool LegacyGroup = false;

        private void OnEnable()
        {
            legacyInputSystem = serializedObject.FindProperty("legacyInputSystem");
            legacyInputs = serializedObject.FindProperty("legacyInputs");
            playerInput = serializedObject.FindProperty("playerInput");
            info = serializedObject.FindProperty("info");
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            PlayerInputController playerInputController = (PlayerInputController)target;
            serializedObject.Update();

            // banner
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Input Event System/Resources/Icons/inspectorBanner_InputEventSystem.png", typeof(Texture));
            GUILayout.Box(banner, GUILayout.ExpandWidth(true), GUILayout.Height(Screen.width / 7.5f));

            // custom inspector
            EditorGUILayout.PropertyField(legacyInputSystem);
            LegacyGroup = legacyInputSystem.boolValue;
            if (LegacyGroup)
            {
                EditorGUILayout.PropertyField(legacyInputs);
            }
            else
            {
                EditorGUILayout.PropertyField(playerInput);
            }
            EditorGUILayout.PropertyField(info);

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

    } // class end
}

#endif
