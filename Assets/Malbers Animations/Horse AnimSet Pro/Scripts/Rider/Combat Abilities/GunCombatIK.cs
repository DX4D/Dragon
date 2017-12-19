using System;
using MalbersAnimations.Weapons;
using UnityEngine;

namespace MalbersAnimations.HAP
{
    [CreateAssetMenu(menuName = "Malbers Animations/HAP/Gun Combat IK")]
    public class GunCombatIK : GunCombat
    {
        private static Keyframe[] KeyFrames = { new Keyframe(0, 0.61f), new Keyframe(1.25f, 0.61f), new Keyframe(2, 0.4f) };

        [Space]
        public Vector3 RightHandOffset;
        public Vector3 LeftHandOffset;

        public AnimationCurve HandIKDistance = new AnimationCurve(KeyFrames);


        public override void ActivateAbility()
        {
            base.ActivateAbility();
            EnableAimIKBehaviour(true); //Activate the IK MODE on the Animator Animations States
        }
    }
}