using System;
using System.Collections;
using UnityEngine;
using MalbersAnimations;

namespace MalbersAnimations
{
    public class AnimalAttack : MonoBehaviour
    {
        public Animal target;


        private void LateUpdate()
        {
            //transform.position = target.position + offset;
            if (target != null) //Make sure we have something to position around
            {
                bool XPressed = Input.GetKey(KeyCode.Joystick1Button3);
                bool YPressed = Input.GetKey(KeyCode.Joystick1Button2);
                bool BPressed = Input.GetKey(KeyCode.Joystick1Button0);
                bool APressed = Input.GetKey(KeyCode.Joystick1Button1);

                bool ZKeyPressed = Input.GetKey(KeyCode.Z);
                bool XKeyPressed = Input.GetKey(KeyCode.X);
                float vertical = 0.0f;
                vertical = Input.GetAxis("Vertical");
                if (Input.GetKey(KeyCode.W)) { vertical += 1.0f; }
                if (Input.GetKey(KeyCode.S)) { vertical -= 1.0f; }

                if (vertical < 0f) { if (YPressed || ZKeyPressed) { target.SetAttack(13); } } //Down Pressed
                if (vertical > 0f) { if (YPressed || ZKeyPressed) { target.SetAttack(14); } } //Up Pressed
            }
        }
    }
}