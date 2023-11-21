#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.InputEventSystem
{
    public class GaskellgamesMenuItemInputEventSystem : GaskellgamesMenuItem
    {
        #region Tools Menu

        private const string GaskellgamesToolsMenu_InputActions = GaskellgamesToolsMenu + "/Input Event System";

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region GameObject Menu
        
        private const string GaskellgamesGameobjectMenu_InputActions = GaskellgamesGameobjectMenu + "/Input Event System";
        
        [MenuItem(GaskellgamesGameobjectMenu_InputActions + "/Input Actions Manager", false, 10)]
        private static void Gaskellgames_GameobjectMenu_InputActionsManager()
        {
            // Create a custom game object
            GameObject go = new GameObject("InputActionsManager");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            InputActionManager iam = go.AddComponent<InputActionManager>();
            iam.SetupInputActionManager();
        }
        
        [MenuItem(GaskellgamesGameobjectMenu_InputActions + "/Input Actions Manager", true, 10)]
        private static bool Gaskellgames_GameobjectMenu_InputActionsManagerValidate()
        {
            InputActionManager exists = GameObject.FindObjectOfType<InputActionManager>();
            if (exists) { return false; } else { return true; }
        }
        
        [MenuItem(GaskellgamesGameobjectMenu_InputActions + "/Input Event", false, 25)]
        private static void Gaskellgames_GameobjectMenu_InputEvent()
        {
            // Create a custom game object
            GameObject go = new GameObject("InputEvent");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            go.AddComponent<InputEvent>();
        }
        
        [MenuItem(GaskellgamesGameobjectMenu_InputActions + "/Trigger Action Target", false, 25)]
        private static void Gaskellgames_GameobjectMenu_TriggerActionTarget()
        {
            // Create a custom game object
            GameObject go = new GameObject("TriggerActionTarget");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            go.AddComponent<TriggerActionTarget>();
        }
        
        [MenuItem(GaskellgamesGameobjectMenu_InputActions + "/Trigger Action Sensor (Sphere)", false, 25)]
        private static void Gaskellgames_GameobjectMenu_TriggerActionSensorSphere()
        {
            // Create a custom game object
            GameObject go = new GameObject("TriggerActionSensor (Sphere)");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            go.AddComponent<SphereCollider>();
            go.AddComponent<TriggerActionSensor>();
        }
        
        [MenuItem(GaskellgamesGameobjectMenu_InputActions + "/Trigger Action Sensor (Box)", false, 25)]
        private static void Gaskellgames_GameobjectMenu_TriggerActionSensorCube()
        {
            // Create a custom game object
            GameObject go = new GameObject("TriggerActionSensor (Box)");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            go.AddComponent<BoxCollider>();
            go.AddComponent<TriggerActionSensor>();
        }
        
        [MenuItem(GaskellgamesGameobjectMenu_InputActions + "/Input Controller (Player)", false, 40)]
        private static PlayerInput Gaskellgames_GameobjectMenu_PlayerInputController()
        {
            // add input action manager
            AddInputActionsManager();
            
            // Create a custom game object
            GameObject go = new GameObject("InputController (Player)");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            PlayerInputController pic = go.AddComponent<PlayerInputController>();
            PlayerInput pi = go.AddComponent<PlayerInput>();
            pic.SetPlayerInput(pi);
            pi.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Gaskellgames/Input Event System/Resources/Input Actions/InputActionsGaskellgames.inputactions");

            return pi;
        }
        
        [MenuItem(GaskellgamesGameobjectMenu_InputActions + "/Input Controller (VR)", false, 40)]
        private static void Gaskellgames_GameobjectMenu_VRInputController()
        {
            // add input action manager
            AddInputActionsManager();
            
            // Create a custom game object
            GameObject go = new GameObject("InputController (VR)");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            VRInputController pic = go.AddComponent<VRInputController>();
            PlayerInput pi = go.AddComponent<PlayerInput>();
            pic.SetPlayerInput(pi);
            pi.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Gaskellgames/Input Event System/Resources/Input Actions/InputActionsGaskellgames.inputactions");
        }
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Public Functions

        public static PlayerInput AddPlayerInputController()
        {
            return Gaskellgames_GameobjectMenu_PlayerInputController();
        }

        public static void AddInputActionsManager()
        {
            if (Gaskellgames_GameobjectMenu_InputActionsManagerValidate())
            {
                Gaskellgames_GameobjectMenu_InputActionsManager();
            }
        }

        #endregion
        
    } // class end
}

#endif