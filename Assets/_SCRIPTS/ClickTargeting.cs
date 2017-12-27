using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ClickTargeting : MonoBehaviour
    {
        Camera cam;

        public void Start()
        {
            cam = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1)) {
                Ray r = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(r, out hit))
                {
                    Debug.Log("Clicked " + hit.collider.name + " at " + hit.point);
                    //Focus on Target
                    //Move to Target (if already Focused)
                    //RemoveTargets
                }
            }
           RotatePlayer();
        }

        //Make sure the camera is not rotated when the player is
        void LateUpdate()
        {
            
        }

        //Calculate and rotate towards the target
        private void RotatePlayer()
        {
            
        }
    }
