namespace RESC
{
    // C#
    using System.Collections;
    using System.Collections.Generic;

    // Unity
    using UnityEngine;

    public class SinMovement : MonoBehaviour
    {
        private float speed = 3.0f;
        private float length = 0.1f;

        private float runningTime = 0.0f;
        private float yPos = 0.0f;

        private bool isLanded = false;

        private void Update()
        {
            if (isLanded)
            {
                runningTime += Time.deltaTime * speed;
                yPos = Mathf.Sin(runningTime) * length;
                this.transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("WaterLine"))
            {
                Debug.Log("OnTriggerEnter" + other.gameObject.name);
                isLanded = true;

                this.transform.rotation = Quaternion.identity;

                Rigidbody rb = GetComponent<Rigidbody>();
                Collider collider = GetComponent<Collider>();

                rb.useGravity = false;
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;

                collider.isTrigger = true;
            }
        }
    }
}
