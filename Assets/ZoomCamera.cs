using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour {

    public int cameraCurrentZoom = 8;
    public int cameraZoomMax = 20;
    public int cameraZoomMin = 5;

    // Use this for initialization
    void Start () {
        Camera.main.orthographicSize = cameraCurrentZoom;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            if (cameraCurrentZoom < cameraZoomMax)
            {
                cameraCurrentZoom += 1;
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize + 1);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            if (cameraCurrentZoom > cameraZoomMin)
            {
                cameraCurrentZoom -= 1;
                Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize - 1);
            }
        }
        //OLD
        /*
                float currentAngle = transform.eulerAngles.y;
        float desiredAngle = target.transform.eulerAngles.y;
        float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);

        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        transform.position = target.transform.position - (rotation * offset);

        transform.LookAt(target.transform);
        */
    }
}
