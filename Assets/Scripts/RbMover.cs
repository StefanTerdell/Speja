using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RbMover : MonoBehaviour
{
    Rigidbody caughtRigidbody;
    Camera cam;
    float holdTime;
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (caughtRigidbody)
            {
                caughtRigidbody.isKinematic = true;
                caughtRigidbody.gameObject.layer = 8;

                var multiplier = cam.orthographic ? cam.orthographicSize * .05f : Vector3.Distance(transform.position, caughtRigidbody.position) * .025f;

                caughtRigidbody.position += transform.TransformDirection(new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"))) * multiplier;

                holdTime += Time.deltaTime;
            }
            else
            {
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hitInfo))
                {
                    caughtRigidbody = hitInfo.rigidbody;
                }
            }
        }
        else if (caughtRigidbody)
        {
            if (holdTime < .2f)
                caughtRigidbody.isKinematic = false;

            caughtRigidbody.gameObject.layer = 0;
            holdTime = 0;
            caughtRigidbody = null;
        }
    }
}
