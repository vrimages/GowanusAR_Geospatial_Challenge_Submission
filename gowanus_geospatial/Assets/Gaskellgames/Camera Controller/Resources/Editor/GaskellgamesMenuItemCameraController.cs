#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

using Gaskellgames;
using Gaskellgames.InputEventSystem;

namespace Gaskellgames.CameraController
{
    public class GaskellgamesMenuItemCameraController : GaskellgamesMenuItem
    {
        #region Tools Menu

        private const string CameraControllerToolsMenu = GaskellgamesToolsMenu + "/Camera Controller";

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region GameObject Menu
        
        private const string CameraControllerGameobjectMenu = GaskellgamesGameobjectMenu + "/Camera Controller";
        
        [MenuItem(CameraControllerGameobjectMenu + "/Camera Brain", false, 10)]
        private static GameObject Gaskellgames_GameobjectMenu_CameraBrain()
        {
            // Create a custom game object
            GameObject go = new GameObject("CameraBrain");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            go.AddComponent<Camera>();
            go.AddComponent<AudioListener>();
            go.AddComponent<CameraBrain>();

            return go;
        }
        
        [MenuItem(CameraControllerGameobjectMenu + "/Camera Rig", false, 25)]
        private static void Gaskellgames_GameobjectMenu_CameraRig()
        {
            // Create a custom game object
            GameObject go = new GameObject("CameraRig");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            go.AddComponent<CameraRig>();
        }
        
        [MenuItem(CameraControllerGameobjectMenu + "/Camera Rig (Freelook)", false, 25)]
        private static void Gaskellgames_GameobjectMenu_CameraFreelookRig()
        {
            // add input action manager
            GaskellgamesMenuItemInputEventSystem.AddInputActionsManager();
            
            // Create a custom game object
            GameObject go = new GameObject("CameraFreelookRig");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            CameraFreelookRig cfr =  go.AddComponent<CameraFreelookRig>();
        }
        
        [MenuItem(CameraControllerGameobjectMenu + "/Camera Trigger Zone", false, 40)]
        private static void Gaskellgames_GameobjectMenu_CameraTriggerZone()
        {
            // Create a custom game object
            GameObject go = new GameObject("CameraTriggerZone");
            GameObject goChild1 = new GameObject("CameraRig");
            GameObject goChild2 = new GameObject("Ref: CamLookAt");
            GameObject goChild3 = new GameObject("Camera Trigger Sensor");
            goChild1.transform.SetParent(go.transform);
            goChild2.transform.SetParent(go.transform);
            goChild3.transform.SetParent(go.transform);
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            goChild1.transform.position = new Vector3(-3, 2, -3);
            CameraRig cr = goChild1.AddComponent<CameraRig>();
            cr.SetCameraLookAt(goChild2.transform);
            goChild2.transform.position = Vector3.zero;
            goChild3.transform.position = Vector3.zero;
            goChild3.transform.localScale = new Vector3(3, 0.2f, 3);
            BoxCollider box = goChild3.AddComponent<BoxCollider>();
            box.isTrigger = true;
            CameraTriggerSensor cts = goChild3.AddComponent<CameraTriggerSensor>();
            cts.SetCameraRig(cr);
        }
        
        [MenuItem(CameraControllerGameobjectMenu + "/Camera Trigger Zone (Multi Target)", false, 40)]
        private static void Gaskellgames_GameobjectMenu_CameraTriggerZoneMultiTarget()
        {
            // Create a custom game object
            GameObject go = new GameObject("CameraTriggerZone (Multi Target)");
            GameObject goChild1 = new GameObject("CameraRig");
            GameObject goChild2 = new GameObject("Ref: CamLookAt");
            GameObject goChild3 = new GameObject("Camera Trigger Sensor");
            goChild1.transform.SetParent(go.transform);
            goChild2.transform.SetParent(go.transform);
            goChild3.transform.SetParent(go.transform);
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            goChild1.transform.position = new Vector3(-3, 2, -3);
            CameraRig cr = goChild1.AddComponent<CameraRig>();
            cr.SetCameraLookAt(goChild2.transform);
            goChild2.transform.position = Vector3.zero;
            goChild3.transform.position = Vector3.zero;
            goChild3.transform.localScale = new Vector3(3, 0.2f, 3);
            BoxCollider box = goChild3.AddComponent<BoxCollider>();
            box.isTrigger = true;
            CameraMultiTarget cmt = goChild3.AddComponent<CameraMultiTarget>();
            cmt.SetRefCamLookAt(goChild2.transform);
            CameraTriggerSensor cts = goChild3.AddComponent<CameraTriggerSensor>();
            cts.SetCameraRig(cr);
        }
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Public Functions

        public static GameObject AddCameraBrain()
        {
            return Gaskellgames_GameobjectMenu_CameraBrain();
        }

        #endregion
        
    } // class end
}

#endif