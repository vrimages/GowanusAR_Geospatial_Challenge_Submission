using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.InputEventSystem
{
    public class TriggerActionTarget : MonoBehaviour
    {
        #region Variables

        [Space][TagDropdown, SerializeField] private string triggerTag = "";
        [Space][SerializeField] private InspectorEvent OnEnterActionSensor;
        [Space][SerializeField] private InspectorEvent OnExitActionSensor;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Triggers

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                OnEnterActionSensor.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                OnExitActionSensor.Invoke();
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Public Functions

        public void ForceExit()
        {
            OnExitActionSensor.Invoke();
        }

        #endregion

    } //class end
}