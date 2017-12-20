using System;
using UnityEngine;


namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 0f, 0f);
        public bool invertLookAxis = false;
        public float lookSensitivity = 1.0f;
        public float lookHorizontalAngle = 180.0f;
        public float minCameraHeight = 1.0f;
        public float maxCameraHeight = 10.0f;
        private float currentYoffset = 7.5f;


        private void LateUpdate()
        {
            if (target != null) //Make sure we have something to position around
            {
                //Adjust the offset (Vertical)
                //if (invertLookAxis) maxCameraHeight *= -1; //Make the value negative if the Look Axis is Inverted
                //TODO Adjust minCameraHeight based on Target Height
                currentYoffset += Input.GetAxis("VerticalTurn") * Time.deltaTime * lookSensitivity * maxCameraHeight * 2f;
                if (currentYoffset > maxCameraHeight) currentYoffset = maxCameraHeight; //Limit to Max
                if (currentYoffset < minCameraHeight) currentYoffset = minCameraHeight; //Limit to Min
                offset.y = currentYoffset;

                //Reposition the Camera
                transform.position = target.position + offset;
                
                //Rotate the Camera (Horizontal)
                transform.RotateAround(target.position, Vector3.up, Input.GetAxis("HorizontalTurn") * (lookHorizontalAngle / 2) * Time.deltaTime * lookSensitivity);
            }
        }
    }
}
