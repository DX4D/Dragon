using System;
using System.Collections;
using UnityEngine;

public class JumpFly : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public TimeSpan pressDelay = new TimeSpan(0, 0, 0, 1);
    DateTime jumpLastPressed;
    bool jumping = false;

    private void Start()
    {
        jumpLastPressed = new DateTime(0,0,0);
        jumping = false;
    }
    private void LateUpdate()
    {
        
        //transform.position = target.position + offset;
        if (target != null) //Make sure we have something to position around
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                jumping = true;
                jumpLastPressed = DateTime.Now;
            }
            if (Input.GetKeyUp(KeyCode.Space) && Input.GetKeyUp(KeyCode.Joystick1Button0))
            {
                jumping = false;
            }
            if (jumping && DateTime.Now.Subtract(jumpLastPressed) <= pressDelay) { /*FLY*/ }
        }
    }
}