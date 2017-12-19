using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.HAP;


namespace MalbersAnimations.Weapons
{
    public class AimIKBehaviour : StateMachineBehaviour
    {
        [Header("This is Link to the GunCombatIKMode")]
        public bool active = true;

        float Weight = 0;

        RiderCombat RC;

        GunCombatIK IkMode;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            RC = animator.GetComponent<RiderCombat>();
            IkMode = (RC.ActiveAbility as GunCombatIK);

            active = false;

            if (IkMode) active = true;
        }


        //OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK(inverse kinematics) should be implemented here.
        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!active) return;

            bool isRightHand = RC.Active_IMWeapon.RightHand;

            var origin = isRightHand ? RC.RightShoulder.position : RC.LeftShoulder.position;

            float Hand_Distance = isRightHand ? IkMode.HandIKDistance.Evaluate(1 + RC.HorizontalAngle) : IkMode.HandIKDistance.Evaluate(1 - RC.HorizontalAngle); //Values for the Distance of the Arm while rotating

            Vector3 LookDirection = RC.Target ? RC.AimDirection : RC.AimDot ? Utilities.MalbersTools.DirectionFromCameraNoRayCast(RC.AimDot.position) : Camera.main.transform.forward;
            //Vector3 aimDirection = Camera.main.transform.forward;

            Ray RayHand = new Ray(origin, LookDirection);

            Vector3 IKPoint = RayHand.GetPoint(Hand_Distance);

            Vector3 LookDirectionFromHand = (RC.AimRayCastHit.point - (isRightHand ? RC.RightHand.position : RC.LeftHand.position)).normalized;

            if (RC.IsAiming)
            {
                Weight = Mathf.Lerp(Weight, 1, Time.deltaTime * 10);

                var HandRotation =
                    Quaternion.LookRotation(LookDirection) * Quaternion.Euler(isRightHand ? IkMode.RightHandOffset : IkMode.LeftHandOffset); //Set the Aim Look Rotation for the  Right or Left Hand

                var ikGoal = isRightHand ? AvatarIKGoal.RightHand : AvatarIKGoal.LeftHand;  //Set the IK goal acording the Right or Left Hand


                //ArmIK
                animator.SetIKPosition(ikGoal, IKPoint);
                animator.SetIKPositionWeight(ikGoal, Weight);

                if (RC.WeaponAction != MalbersAnimations.WeaponActions.Fire_Proyectile)
                {
                    animator.SetIKRotation(ikGoal, HandRotation);
                    animator.SetIKRotationWeight(ikGoal, Weight);
                }


                //HeadIK
                animator.SetLookAtWeight(1 * Weight, 0.3f * Weight);
                animator.SetLookAtPosition(RayHand.GetPoint(10));

            }

        }
    }
}