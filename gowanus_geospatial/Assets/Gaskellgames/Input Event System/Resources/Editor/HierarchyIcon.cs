#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.InputEventSystem
{
    [InitializeOnLoad]
    public class HierarchyIcon
    {
        private static readonly Texture2D icon;

        static HierarchyIcon()
        {
            icon = AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Input Event System/Resources/Icons/Icon_InputEventSystem.png", typeof(Texture2D)) as Texture2D;
            if (icon == null) { return; }
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon;
        }

        private static void DrawHierarchyIcon(int instanceID, Rect rect)
        {
            if (icon == null) { return; }
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) { return; }
            var componant = gameObject.GetComponent<InputActionManager>();
            if (componant == null) { return; }
            float hierarchyPixelHeight = 16;
            EditorGUIUtility.SetIconSize(new Vector2(hierarchyPixelHeight, hierarchyPixelHeight));
            var iconPosition = new Rect(rect.xMax - hierarchyPixelHeight, rect.yMin, rect.width, rect.height);
            var iconGUIContent = new GUIContent(icon);
            EditorGUI.LabelField(iconPosition, iconGUIContent);
        }
    }
}

#endif