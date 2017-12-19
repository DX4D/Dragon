using System;
using MalbersAnimations.Weapons;
using UnityEngine;

namespace MalbersAnimations.HAP
{
    /// <summary>
    /// Ability that it will Manage the Bow Combat System while Riding
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/HAP/Bow Combat")]
    public class BowCombat : RiderCombatAbility
    {
        bool isHolding;             //for checking if the Rider is Holding/Tensing the String
        float HoldTime;             //Time pass since the Rider started tensing the string

        private static Keyframe[] KeyFrames = 
            { new Keyframe(0, 1), new Keyframe(1.25f, 1), new Keyframe(1.5f, 0), new Keyframe(2f, 0) };

        [Header("Right Handed Bow Offsets")]
        public Vector3 ChestRight = new Vector3(25,0,0);
        public Vector3 ShoulderRight = new Vector3(5, 0, 0);
        public Vector3 HandRight;

        [Header("Left Handed Bow Offsets")]
        public Vector3 ChestLeft = new Vector3(-25,0,0);
        public Vector3 ShoulderLeft = new Vector3(-5, 0, 0);
        public Vector3 HandLeft;


        [Space]
        [Tooltip("This Curve is for straightening the aiming Arm while is on the Aiming State")]
        public AnimationCurve AimWeight = new AnimationCurve(KeyFrames);

        protected bool KnotToHand;
        protected Quaternion Delta_Hand;

        public override bool TypeOfAbility(IMWeapon weapon)
        {
            return weapon is IBow;
        }

        public override WeaponType WeaponType()
        {
            return MalbersAnimations.WeaponType.Bow;
        }

        public override void StartAbility(RiderCombat ridercombat)
        {
            base.StartAbility(ridercombat);
            KnotToHand = false;
        }

        public override void UpdateAbility()
        {
            if (RC.WeaponAction != WeaponActions.Fire_Proyectile)
            {
                BowAttack(RC.Active_IMWeapon as IBow);       //Fire Bow
            }


            //Important !! Reset the bow state if is holding but is not aiming
            if (!RC.IsAiming && isHolding)
            {
                isHolding = false;
                HoldTime = 0;
                RC.Anim.SetFloat(Hash.IDFloat, 0);            //Reset Hold Animator Values
                (RC.Active_IMWeapon as IBow).BendBow(0);
            }

            BowKnotInHand(); //Update the BowKnot in the Hand if is not Firing
        }

        public override void LateUpdateAbility()
        {
            FixAimPoseBow();
        }

        /// <summary>
        /// Bow Attack Mode
        /// </summary>
        protected virtual void BowAttack(IBow Bow)
        {
            if (RC.IsAiming)                                           //Shoot arrows only when is aiming
            {
                bool isInRange = RC.Active_IMWeapon.RightHand ? RC.HorizontalAngle < 0.5f : RC.HorizontalAngle > -0.5f; //Calculate the Imposible range to shoot

                if (!isInRange)
                {
                    isHolding = false;
                    HoldTime = 0;
                    return;
                }

                if (RC.InputAttack1.GetInput && !isHolding)            //If Attack is pressed Start Bending for more Strength the Bow
                {
                    RC.SetAction(WeaponActions.Hold);
                    isHolding = true;
                    HoldTime = 0;
                }

                if (RC.InputAttack1.GetInput && isHolding)             // //If Attack is pressed Continue Bending the Bow for more Strength the Bow
                {
                    HoldTime += Time.deltaTime;

                    if (HoldTime<=Bow.HoldTime+Time.deltaTime)
                        Bow.BendBow(HoldTime / Bow.HoldTime);    //Bend the Bow

                    RC.Anim.SetFloat(Hash.IDFloat, HoldTime / Bow.HoldTime);
                }

                if (!RC.InputAttack1.GetInput && isHolding)            //If Attack is Release Go to next Action and release the Proyectile
                {
                    var Knot = Bow.KNot;
                    Knot.rotation = Quaternion.LookRotation(RC.AimDirection); //Alingns the Knot and Arrow to the AIM DIRECTION before Releasing the Arrow

                    RC.SetAction(WeaponActions.Fire_Proyectile);              //Go to Action FireProyectile
                    isHolding = false;
                    HoldTime = 0;
                    RC.Anim.SetFloat(Hash.IDFloat, 0);            //Reset Hold Animator Values
                    Bow.ReleaseArrow(RC.AimDirection);

                    Bow.BendBow(0);

                    RC.OnAttack.Invoke(RC.Active_IMWeapon);                 //Invoke the On Attack Event
                }
            }
        }

        public virtual void EquipArrow()
        {
            IBow Bow = RC.Active_IMWeapon as IBow;
            Bow.EquipArrow();
        }

        public override bool ChangeAimCameraSide()
        {
            return false;
        }

        public override void ResetAbility()
        {
            KnotToHand = false;

        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// This will rotate the bones of the character to match the AIM direction 
        /// </summary>
        protected virtual void FixAimPoseBow()
        {
            if (RC.IsAiming)
            {
                float Weight = 
                    RC.Active_IMWeapon.RightHand ? AimWeight.Evaluate(1 + RC.HorizontalAngle) : AimWeight.Evaluate(1 - RC.HorizontalAngle); //The Weight evaluated on the AnimCurve

                Vector3 LookDirection = RC.Target ? RC.AimDirection : RC.AimDot ? Utilities.MalbersTools.DirectionFromCameraNoRayCast(RC.AimDot.position) : cam.forward;

                Quaternion LookRotation = Quaternion.LookRotation(LookDirection, RC.Target ? Vector3.up : cam.up);

                Vector3 ShoulderRotationAxis = RC.Target ? Vector3.Cross(Vector3.up, LookDirection).normalized : cam.right;

                RC.Chest.RotateAround(RC.Chest.position, ShoulderRotationAxis, (Vector3.Angle(Vector3.up, LookDirection) - 90) * Weight); //Nicely Done!!

                if (RC.Active_IMWeapon.RightHand)
                {
                    RC.Chest.rotation *= Quaternion.Euler(ChestRight);
                    RC.RightHand.rotation *= Quaternion.Euler(HandRight);
                    RC.RightShoulder.rotation = Quaternion.Lerp(RC.RightShoulder.rotation, LookRotation * Quaternion.Euler(ShoulderRight), Weight); // MakeDamage the boy always look to t
                }
                else
                {
                    RC.Chest.rotation *= Quaternion.Euler(ChestLeft);
                    RC.LeftHand.rotation *= Quaternion.Euler(HandLeft);
                    RC.LeftShoulder.rotation = Quaternion.Lerp(RC.LeftShoulder.rotation, LookRotation * Quaternion.Euler(ShoulderLeft), Weight); // MakeDamage the boy always look to t
                }
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///Put the Bow Knot to the fingers Hand This is called for the Animator
        /// </summary>
        public virtual void BowKnotToHand(bool enabled)
        {
            KnotToHand = enabled;

            var bow = RC.Active_IMWeapon as IBow;

            if (!KnotToHand && bow !=null)
            {
                bow.RestoreKnot();
            }
        }

        /// <summary>
        /// Updates the BowKnot position in the center of the hand if is active
        /// </summary>
        protected void BowKnotInHand()
        {
            if (KnotToHand)
            {
                var bow = RC.Active_IMWeapon as IBow;
                bow.KNot.position = 
                    (RC.Anim.GetBoneTransform(RC.Active_IMWeapon.RightHand ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal).position+
                    RC.Anim.GetBoneTransform(RC.Active_IMWeapon.RightHand ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal).position)/2;

                bow.KNot.position = bow.KNot.position;
            }
        }


        public override Transform AimRayOrigin()
        {
            return (RC.Active_IMWeapon as IBow).KNot;
        }
    }
}