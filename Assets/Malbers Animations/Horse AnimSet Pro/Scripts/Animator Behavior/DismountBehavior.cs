using UnityEngine;

using System.Collections;

namespace MalbersAnimations.HAP
{
    public class DismountBehavior : StateMachineBehaviour
    {
        [Tooltip("Use to Align the RootMotion with Hip Position, instead with the last FeetPositions")]
        public bool UseHip;

        Rider3rdPerson rider;
        Vector3 MountPosition; //Feets Next Postition
        AnimatorTransitionInfo transition;
        Transform transform;
        Transform hip;
        Vector3 laspos;

        float ScaleFactor;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rider = animator.GetComponent<Rider3rdPerson>();

            ScaleFactor = rider.Montura.Animal.ScaleFactor;                                     //Get the scale Factor from the Montura

            Vector3 LeftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            Vector3 RightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

            laspos = (LeftFoot + RightFoot) / 2;
            MountPosition = animator.rootPosition;

            transform = animator.transform;
            rider.Start_Dismounting();

            hip = animator.GetBoneTransform(HumanBodyBones.Hips);   //Get the Hip Bone
            animator.SetInteger(Hash.MountSide, 0);                 //remove the side of the mounted **IMPORTANT*** otherwise it will keep trying to dismount
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rider.End_Dismounting();
        }

        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            transition = animator.GetAnimatorTransitionInfo(layerIndex);
            transform.position += (animator.velocity * Time.deltaTime * ScaleFactor);
            // transform.position = animator.rootPosition;
            transform.rotation = animator.rootRotation;

            //Smoothly move the center of mass to the desired position in the first Transition
            if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime < 0.5f)
            {
                //animator.MatchTarget(rider.Montura.MountPoint.position, Quaternion.FromToRotation(animT.up, Vector3.up) * animT.rotation, AvatarTarget.Root,)

                if (UseHip)
                {
                    Vector3 diferencia = hip.position - rider.Montura.MountPoint.position;
                    transform.position = transform.position - diferencia;
                }
                else
                {
                    transform.position = Vector3.Lerp(MountPosition, laspos, transition.normalizedTime);
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation, transition.normalizedTime);
            }
        

            //Don't go under the floor
            if (rider.MountTrigger)
            {
                if (transform.position.y < rider.MountTrigger.transform.position.y)
                {
                    transform.position = new Vector3(transform.position.x, rider.MountTrigger.transform.position.y, transform.position.z);
                }
            }

            //Smoothly  Deactivate Mount Layer
            if (stateInfo.normalizedTime > 0.8f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation, transition.normalizedTime);
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, rider.MountTrigger.transform.position.y, transform.position.z), Time.deltaTime * 5f);
            }

            animator.rootPosition = transform.position;
        }
    }
}