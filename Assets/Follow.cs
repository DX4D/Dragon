using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public Transform target;
    public Vector3 offset;
    float chaseSpeed;


    void Start () { // Use this for initialization
        
	}

    void Update()
    { //Move Characters Here

    }

    void LateUpdate()
    { //Move Camera Here
        var newX = target.position.x;
        var newZ = target.position.z;
        var y = target.position.y;

        transform.position = (new Vector3(newX, y, newZ) - offset);
        

        //    cameraLastTarget.position = new Vector3(cameraTarget.position.x, cameraTarget.position.y + cameraHeight, cameraTarget.position.z - cameraMinDistance);
        //    cameraLastTarget.LookAt(cameraTarget);
    }
}
