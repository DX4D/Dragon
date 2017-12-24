using UnityEngine;
using System;


namespace MalbersAnimations.HAP
{
    public class MountBehavior : StateMachineBehaviour
    {
        public AnimationCurve MovetoMountPoint;

        protected Rider3rdPerson rider;
        protected Transform MountTrigger;
        protected Transform transform;
        protected Transform hip;

        const float toMountPoint = 0.2f; //Smooth time to put the Rider in the right position for mount

        float ScaleFactor;

        TransformAnimation Fix;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(Hash.MountSide, 0);                //remove the side of the mounted ****IMPORTANT

            rider = animator.GetComponent<Rider3rdPerson>();

            transform = animator.transform;
            hip = animator.GetBoneTransform(HumanBodyBones.Hips);

            ScaleFactor = rider.Montura.Animal.ScaleFactor;         //Get the scale Factor from the Montura

            ResetFloatParameters(animator);

            float CenterDiference = animator.transform.position.y - animator.pivotPosition.y;           //Mount Animations have the pivot on the CoG (Center of Gravity)

            transform.position = new Vector3(transform.position.x, transform.position.y + CenterDiference, transform.position.z);  //Change the Position of the Rider from Feet to CoG

            MountTrigger = rider.MountTrigger.transform;

            Fix = rider.MountTrigger.Adjustment;            //Store the Fix
          

            rider.Start_Mounting();
        }

        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            transform.position += (animator.velocity * Time.deltaTime * ScaleFactor * (Fix ? Fix.time : 1)); //Scale the animations Root Position   
            //transform.position = animator.rootPosition;
            transform.rotation = animator.rootRotation;

            Vector3 Mount_Position = rider.Montura.MountPoint.position;

            //Smootly move to the Mount Start Position && rotation
            if (stateInfo.normalizedTime < toMountPoint)
            {
                Vector3 NewPos = new Vector3(MountTrigger.position.x, transform.position.y, MountTrigger.position.z);
                transform.position = Vector3.Lerp(transform.position, NewPos, stateInfo.normalizedTime / toMountPoint);

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(MountTrigger.forward), stateInfo.normalizedTime / toMountPoint);
            }

            //Smoothy adjust the rider to the Mount Position/Rotation

            if (Fix)
            {
                if (!Fix.SeparateAxisPos)
                {
                    transform.position = Vector3.LerpUnclamped(transform.position, Mount_Position, Fix.PosCurve.Evaluate(stateInfo.normalizedTime));
                }
                else
                {
                    float x = Mathf.LerpUnclamped(transform.position.x, Mount_Position.x, Fix.PosXCurve.Evaluate(stateInfo.normalizedTime)* Fix.Position.x);
                    float y = Mathf.LerpUnclamped(transform.position.y, Mount_Position.y, Fix.PosYCurve.Evaluate(stateInfo.normalizedTime)* Fix.Position.y);
                    float z = Mathf.LerpUnclamped(transform.position.z, Mount_Position.z, Fix.PosZCurve.Evaluate(stateInfo.normalizedTime)* Fix.Position.z);

                    Vector3 newPos = new Vector3(x,y,z);

                   // Debug.Log(Fix.PosXCurve.Evaluate(stateInfo.normalizedTime) * Fix.Position.x + "," + Fix.PosYCurve.Evaluate(stateInfo.normalizedTime) * Fix.Position.y +"," + Fix.PosZCurve.Evaluate(stateInfo.normalizedTime) * Fix.Position.z);
                   

                    transform.position = newPos;
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, rider.Montura.MountPoint.rotation, Fix.RotCurve.Evaluate(stateInfo.normalizedTime));
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, rider.Montura.MountPoint.position, MovetoMountPoint.Evaluate(stateInfo.normalizedTime));
                transform.rotation = Quaternion.Lerp(transform.rotation, rider.Montura.MountPoint.rotation, MovetoMountPoint.Evaluate(stateInfo.normalizedTime));
            }
        }
       
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rider.End_Mounting();
        }

        private static void ResetFloatParameters(Animator animator)
        {
            foreach (AnimatorControllerParameter parameter in animator.parameters)                          //Set All Float values to their defaut
            {
                if (parameter.type == AnimatorControllerParameterType.Float)
                {
                    if (parameter.nameHash == Hash.IKLeftFoot || parameter.nameHash == Hash.IKRightFoot) break;
                    animator.SetFloat(parameter.nameHash, parameter.defaultFloat);
                }
            }
        }
    }
}

