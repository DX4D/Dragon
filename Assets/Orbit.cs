using UnityEngine;

public class Orbit : MonoBehaviour
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
        transform.position = target.position + offset;
        if (target != null) //Make sure we have something to position around
        {
            //Adjust the offset (Vertical)
            //if (invertLookAxis) maxCameraHeight *= -1; //Make the value negative if the Look Axis is Inverted
            //TODO Adjust minCameraHeight based on Target Height
            float vertical = 0.0f;
            vertical = Input.GetAxis("VerticalTurn");
            if (Input.GetKey(KeyCode.UpArrow)) { vertical -= 0.2f; }
            if (Input.GetKey(KeyCode.DownArrow)) { vertical += 0.2f; }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
            { vertical = -1.0f; }
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
            { vertical = 1.0f; }
            currentYoffset += vertical * lookSensitivity * maxCameraHeight * 2f * Time.deltaTime;
            if (currentYoffset > maxCameraHeight) currentYoffset = maxCameraHeight; //Limit to Max
            if (currentYoffset < minCameraHeight) currentYoffset = minCameraHeight; //Limit to Min
            offset.y = currentYoffset;

            //Reposition the Camera
            transform.position = target.position + offset;

            float horizontal = 0.0f;
            horizontal = Input.GetAxis("HorizontalTurn");
            if (Input.GetKey(KeyCode.LeftArrow)) { horizontal = -1.0f; }
            if (Input.GetKey(KeyCode.RightArrow)) { horizontal = 1.0f; }
            horizontal *= (lookHorizontalAngle / 2) * Time.deltaTime * lookSensitivity;
            //Rotate the Camera (Horizontal)
            transform.RotateAround(target.position, Vector3.up, horizontal);
        }
    }
}