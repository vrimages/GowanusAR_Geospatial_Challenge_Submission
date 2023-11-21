using System.Collections.Generic;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.InputEventSystem
{
    public class TriggerActionSensor : MonoBehaviour
    {
        #region Variables

        [Space] [SerializeField] private Color32 triggerColour = new Color32(000, 179, 223, 079);
        [SerializeField] private Color32 triggerOutlineColour = new Color32(000, 179, 223, 128);
        private Collider collider;
        private bool sphereTrigger = false;
        private float sphereRadius = 1.0f;
        private Vector3 boxSize = Vector3.one;
        [SerializeField] private bool alwaysShowZone = true;
        [SerializeField] private bool destroyOnEnter = false;
        [Tooltip("Exit event will only run when targets in sensor list is empty (i.e, when final object exits)")]
        [SerializeField] private bool exitEventOnEmpty = false;
        [TagDropdown, SerializeField] private string triggerTag = "";
        [Space][SerializeField] private InspectorEvent OnEnter;
        [Space][SerializeField] private InspectorEvent OnExit;
        [SerializeField] private List<TriggerActionTarget> actionTargetsInSensor = new List<TriggerActionTarget>();

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Game Loop

        private void Reset()
        {
            InitialiseSettings();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

    #if UNITY_EDITOR

        #region Editor Gizmos

        private void OnDrawGizmos()
        {
            UpdateCollider();

            if (alwaysShowZone)
            {
                DrawZone();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!alwaysShowZone)
            {
                DrawZone();
            }
        }

        #endregion

    #endif

        //----------------------------------------------------------------------------------------------------

        #region Triggers

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                if (other.GetComponent<TriggerActionTarget>() != null)
                {
                    AddTargetToList(other.GetComponent<TriggerActionTarget>());
                }

                OnEnter.Invoke();

                if (destroyOnEnter)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                if (other.GetComponent<TriggerActionTarget>() != null)
                {
                    RemoveTargetFromList(other.GetComponent<TriggerActionTarget>());
                }

                if((exitEventOnEmpty && actionTargetsInSensor.Count == 0) || !exitEventOnEmpty)
                {
                    OnExit.Invoke();
                }
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Functions

        public void ForceReset()
        {
            for (int i = 0; i < actionTargetsInSensor.Count; i++)
            {
                actionTargetsInSensor[i].ForceExit();
            }

            actionTargetsInSensor.Clear();
            OnExit.Invoke();
        }

        private void InitialiseSettings()
        {
            SetupCollider();
        }

        private void SetupCollider()
        {
            collider = gameObject.GetComponent<Collider>();

            if (collider == null)
            {
                gameObject.AddComponent<BoxCollider>();
                collider = gameObject.AddComponent<Collider>();
            }
            collider.isTrigger = true;
        }

        private void UpdateCollider()
        {
            SphereCollider sphere = gameObject.GetComponent<SphereCollider>();
            BoxCollider box = gameObject.GetComponent<BoxCollider>();

            if (box != null)
            {
                collider = box;
                sphereTrigger = false;
                boxSize = box.size;
            }
            else if (sphere != null)
            {
                collider = sphere;
                sphereTrigger = true;
                sphereRadius = sphere.radius;
            }
        }

        private void DrawZone()
        {
            Matrix4x4 resetMatrix = Gizmos.matrix;
            Gizmos.matrix = gameObject.transform.localToWorldMatrix;

            if(sphereTrigger)
            {
                Gizmos.color = triggerColour;
                Gizmos.DrawSphere(Vector3.zero, sphereRadius);
                Gizmos.color = triggerOutlineColour;
                Gizmos.DrawWireSphere(Vector3.zero, sphereRadius);
            }
            else
            {
                Gizmos.color = triggerColour;
                Gizmos.DrawCube(Vector3.zero, boxSize);
                Gizmos.color = triggerOutlineColour;
                Gizmos.DrawWireCube(Vector3.zero, boxSize);
            }

            Gizmos.matrix = resetMatrix;
        }

        public void AddTargetToList(TriggerActionTarget newTarget)
        {
            if(!actionTargetsInSensor.Contains(newTarget))
            {
                actionTargetsInSensor.Add(newTarget);
            }
        }

        public void RemoveTargetFromList(TriggerActionTarget newTarget)
        {
            if (actionTargetsInSensor.Contains(newTarget))
            {
                actionTargetsInSensor.Remove(newTarget);
            }
        }

        #endregion

    } // class end

}