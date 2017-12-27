using System;
using System.Collections;
using UnityEngine;

public class Pitch : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 8f, -8f);
    public bool invertLookAxis = false;
    public float lookSensitivity = 1.0f;
    public float minCameraHeight = 1.0f;
    public float maxCameraHeight = 15.0f;
    private float currentYoffset = 1.0f;


    private void LateUpdate()
    {
        //transform.position = target.position + offset;
        if (target != null) //Make sure we have something to position around
        {
            //Adjust the offset (Vertical)
            //if (invertLookAxis) maxCameraHeight *= -1; //Make the value negative if the Look Axis is Inverted
            //TODO Adjust minCameraHeight based on Target Height
            float vertical = Input.GetAxis("VerticalTurn");
            if (Input.GetKey(KeyCode.UpArrow)) { vertical += 0.05f; }
            if (Input.GetKey(KeyCode.DownArrow)) { vertical -= 0.05f; }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
            { vertical += 0.05f; }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
            { vertical -= 0.05f; }
            currentYoffset += vertical * maxCameraHeight * 2f * lookSensitivity;
            if (currentYoffset > maxCameraHeight) currentYoffset = maxCameraHeight; //Limit to Max
            else if (currentYoffset < minCameraHeight) currentYoffset = minCameraHeight; //Limit to Min
            offset.y = Mathf.Lerp(currentYoffset, 0, Time.deltaTime);

            //Reposition the Camera
            transform.position = target.position + offset;

        }
    }
}