using UnityEngine;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames
{
    [System.Serializable]
    public class VRInputs
    {
        public VRButtonInputsTouch primaryButton;
        public VRButtonInputsTouch secondaryButton;
        public VRButtonInputsTouch joystickButton;
        public ButtonInputs menuButton;

        [Range(-1, 1)] public float joystickHorizontal;
        [Range(-1, 1)] public float joystickVertical;
        [Range(0, 1)] public float triggerAxis;
        public bool triggerTouch;
        [Range(0, 1)] public float gripAxis;

    } // class end
}
