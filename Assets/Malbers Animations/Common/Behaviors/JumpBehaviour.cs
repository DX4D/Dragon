using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MalbersAnimations
{
    public class JumpBehaviour : StateMachineBehaviour
    {
        [Header("Checking Fall")]
        public float fallRay = 1.7f;       //Ray to Check if the Terrain is the same
        public float treshold = 0.5f;      //for calculating something     
        public float willFall = 0.7f;

        [Header("Jump Up Cliff")]
        public float startEdge = 0.5f;
        public float finishEdge = 0.6f;
        public float CliffRay = 0.6f;

        [Space]
        public float JumpMultiplier = 1;

        float jumpPoint;
        float Rb_Y_Speed = 0;
        RaycastHit JumpRay;
        Animal animal;
        Rigidbody rb;
        Vector3 JumpControl;
        float smooth;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();
            rb = animator.GetComponent<Rigidbody>();

            jumpPoint = animator.transform.position.y;
            animal.InAir(true);
            animal.SetIntID(0);

            animal.OnJump.Invoke();     //Invoke that the Animal is Jumping

            Rb_Y_Speed = 0;                 //For Flying

            smooth =  0;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {


            //This code is execute when the animal can change to fall state if there's no future ground to land on
            if (Physics.Raycast(animal.Pivot_fall, -animal.transform.up, out JumpRay, animal.Chest_Pivot_Multiplier * fallRay, animal.GroundLayer))
            {
                if (animal.debug)
                {
                    Debug.DrawRay(animal.Pivot_fall, -animal.transform.up * animal.Chest_Pivot_Multiplier * fallRay, Color.red);
                }

                if ((jumpPoint - JumpRay.point.y) <= treshold * animal.ScaleFactor && (Vector3.Angle(JumpRay.normal, Vector3.up) < animal.maxAngleSlope) )   //If if finding a lower jump point;
                {
                    animal.SetIntID(0);                                               //Keep the INTID in 0
                }
                else
                {
                    if (stateInfo.normalizedTime > willFall) animal.SetIntID(111); //Set INTID to 111 to activate the FALL transition
                }
            }
            else
            {
                if (stateInfo.normalizedTime > willFall) animal.SetIntID(111); //Set INTID to 111 to activate the FALL transition
            }

            //───────────────────────────────────────────────Get jumping on a cliff ────────────────────────────────────────────────────────────────────────────────────────────────────

            if (stateInfo.normalizedTime >= startEdge && stateInfo.normalizedTime <= finishEdge)
            {
                Debug.DrawRay(animal.Chest_Pivot_Point + (animal.transform.forward * 0.2f), -animator.transform.up * CliffRay * animal.ScaleFactor, Color.black);

                if (Physics.Raycast(animal.Chest_Pivot_Point, -animal.transform.up, out JumpRay, CliffRay * animal.ScaleFactor, animal.GroundLayer))
                {
                    if (Vector3.Angle(JumpRay.normal, Vector3.up) < animal.maxAngleSlope) //Jump to a jumpable cliff not an inclined one
                    {
                        animal.SetIntID(110);
                    }
                }
            }


            if (animal.forwardJumpControl)  //If the jump can be controlled on air
            {
                Vector3 pos = animator.transform.position;
                JumpControl = Vector3.Lerp(pos, pos - new Vector3(animator.velocity.x, 0, animator.velocity.z), Time.deltaTime);

                if (animal.MovementReleased)
                {
                    animator.transform.position = Vector3.Lerp(animator.transform.position, JumpControl, smooth);
                }


                smooth += (animal.MovementReleased ? Time.deltaTime : -Time.deltaTime) * animal.smoothJumpForward;

                smooth = Mathf.Clamp01(smooth);
            }
           

            #region if is transitioning to flying
            //If the next animation is FLY smoothly remove the Y rigidbody speed
            if (animator.GetNextAnimatorStateInfo(layerIndex).tagHash == Hash.Fly && animator.IsInTransition(layerIndex) && rb)
            {
                float transitionTime = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
                Vector3 cleanY = rb.velocity;
                if (Rb_Y_Speed < cleanY.y) Rb_Y_Speed = cleanY.y; //Get the max Y SPEED

                cleanY.y = Mathf.Lerp(Rb_Y_Speed, 0, transitionTime);

                rb.velocity = cleanY;
            }
            #endregion
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal.SetIntID(0);


            //if the next animation is fly then clean the rigidbody velocity on the Y axis
            if (animator.GetCurrentAnimatorStateInfo(layerIndex).tagHash == Hash.Fly && rb)
            {
                Vector3 cleanY = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.velocity = cleanY;
            }


            if (   animator.GetCurrentAnimatorStateInfo(layerIndex).tagHash != Hash.Fall 
                && animator.GetCurrentAnimatorStateInfo(layerIndex).tagHash != Hash.Fly)
            {
                animal.IsInAir = false;
            }
        }
    }
}