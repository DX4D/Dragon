using System;
using System.Collections;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, 0f);
    public bool invertLookAxis = false;
    public float lookSensitivity = 1.0f;
    public float lookHorizontalAngle = 180.0f;

    private void LateUpdate()
    {
        //transform.position = target.position + offset;
        if (target != null) //Make sure we have something to position around
        {
            float horizontal = 0.0f;
            horizontal = Input.GetAxis("HorizontalTurn");
            if (Input.GetKey(KeyCode.LeftArrow)) { horizontal = -1.0f; }
            if (Input.GetKey(KeyCode.RightArrow)) { horizontal = 1.0f; }
            horizontal *= (lookHorizontalAngle / 2) * lookSensitivity * Time.deltaTime;
            //Rotate the Camera (Horizontal)
            if(horizontal > 0.0f || horizontal < 0.0f)
            {
                transform.RotateAround(target.position, Vector3.up, horizontal);
            }
        }
    }
}