namespace RESC
{
    // C#
    using System.Collections;
    using System.Collections.Generic;

    // Unity
    using UnityEngine;

    // Project
    // Alias

    [RequireComponent(typeof(Rigidbody))]
    public class Interactable : MonoBehaviour
    {
        public Vector3 m_Offset = Vector3.zero;

        [HideInInspector]
        public Hand m_ActiveHand = null;

        public virtual void Action()
        {
            print("Action");
        }

        public void ApplyOffset(Transform hand)
        {
            transform.SetParent(hand);
            transform.localRotation = Quaternion.identity;
            transform.localPosition = m_Offset;
            transform.SetParent(null);
        }
    }
}
