using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnObject : MonoBehaviour
{
    public float waitTime = 2.0f;

    Coroutine coroutine = null;

    Rigidbody objRigidbody = null;
    Transform objTranform = null;

    Vector3 myOriginPosition = Vector3.zero;
    Quaternion myOriginRotation = Quaternion.identity;

    private void Start()
    {
        objRigidbody = GetComponent<Rigidbody>();
        objTranform = GetComponent<Transform>();

        myOriginPosition = objTranform.position;
        myOriginRotation = objTranform.rotation; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("GabableObject"))
        {
            if (coroutine == null)
            {
                coroutine = StartCoroutine(ObjectReturn());
            }
           
        }
    }

    IEnumerator ObjectReturn()
    {
        yield return new WaitForSeconds(waitTime);

        objRigidbody.velocity = Vector3.zero;
        objRigidbody.angularVelocity = Vector3.zero;

        objTranform.position = myOriginPosition;
        objTranform.rotation = myOriginRotation;

        coroutine = null;

        yield return null;
    }
}
