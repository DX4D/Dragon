using System;
using UnityEngine;


namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public bool invertLookAxis = false;
        public float lookMaxAngle = 100.0f;
        public float lookSpeed = 1.0f;

        public Vector3 followOffset; //Default offsets
        public Vector3 minFollowOffset = new Vector3(1f, 1f, 1f);
        public Vector3 maxFollowOffset = new Vector3(10f, 10f, 10f);

        private float horizontalAdjust {
            get
            {
                float adjust = Input.GetAxis("HorizontalTurn");
                if (adjust == 0.0f)
                {
                    if (Input.GetKey(KeyCode.LeftArrow)) { adjust = -1.0f; }
                    else if (Input.GetKey(KeyCode.RightArrow)) { adjust = 1.0f; }
                }
                adjust *= lookSpeed * Time.deltaTime * lookMaxAngle;
                return adjust;
            }
        }
        private float verticalAdjust
        {
            get
            {
                float adjust = Input.GetAxis("VerticalTurn");
                if (adjust == 0.0f)
                {
                    if (Input.GetKey(KeyCode.UpArrow)) { adjust = -1.0f; }
                    else if (Input.GetKey(KeyCode.DownArrow)) { adjust = 1.0f; }
                }
                adjust *= lookSpeed * Time.deltaTime * lookMaxAngle;
                return adjust;
            }
        }

        private void LateUpdate()
        {
            if (target != null) //Make sure we have something to position around
            {
                //Reposition the Camera (Vertical)
                transform.position = (target.position + followOffset); //Reposition the Camera
                //Rotate the Camera (Horizontal)
                transform.RotateAround(target.position, Vector3.up, -horizontalAdjust);
                //Rotate the Camera (Vertical)
      //          transform.Rotate(target.position, Vector3.right, verticalAdjust);
                //Adjust the offset (Vertical)
                //if (invertLookAxis) maxCameraHeight *= -1; //Make the value negative if the Look Axis is Inverted
                //TODO Adjust minCameraHeight based on Target Height

                /*
                //INPUT
                //Adjust Camera based on Right Thumbstick or Left/Right Arrow Keys
                horizontalAdjust = Input.GetAxis("HorizontalTurn");
                if (Input.GetKey(KeyCode.LeftArrow)) { horizontalAdjust = -1.0f; }
                else if (Input.GetKey(KeyCode.RightArrow)) { horizontalAdjust = 1.0f; }
                horizontalAdjust *= Time.deltaTime * lookSensitivity;

                //Adjust Camera based on Right Thumbstick or Up/Down Arrows
                verticalAdjust = Input.GetAxis("VerticalTurn");
                if (Input.GetKey(KeyCode.UpArrow)) { verticalAdjust = 1.0f; }
                else if (Input.GetKey(KeyCode.DownArrow)) { verticalAdjust = -1.0f; }
                verticalAdjust *= Time.deltaTime * lookSensitivity;




                followOffset.x += lookSpeed * horizontalAdjust;
                followOffset.y += lookSpeed * verticalAdjust;

                horizontalAdjust = 0f;
                verticalAdjust = 0f;

                //Constrain the Camera
                //followOffset = clamp(followOffset, minFollowOffset, maxFollowOffset); //Adjust the Cameraa height above the target



                //Rotate the Camera (Vertical)
                //transform.RotateAround(target.position, Vector3.forward, followOffset.y);
                //Rotate the Camera (Horizontal)
                transform.RotateAround(target.position, Vector3.up, horizontalAdjust);
                */
            }
        }

        private Vector3 clamp(Vector3 toConstrain, Vector3 minConstraints, Vector3 maxConstraints)
        {
            toConstrain.x = clamp(toConstrain.x, minConstraints.x, maxConstraints.x);
            toConstrain.y = clamp(toConstrain.y, minConstraints.y, maxConstraints.y);
            toConstrain.z = clamp(toConstrain.z, minConstraints.z, maxConstraints.z);
            return toConstrain;
        }
        private float clamp(float toClamp, float minValue, float maxValue)
        {
            if (toClamp < minValue) { toClamp = minValue; }
            else if (toClamp > maxValue) { toClamp = maxValue; }
            return toClamp;
        }
    }
}