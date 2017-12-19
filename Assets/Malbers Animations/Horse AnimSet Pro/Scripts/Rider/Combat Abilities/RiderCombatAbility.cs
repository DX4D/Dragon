using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.Weapons;

namespace MalbersAnimations.HAP
{
    /// <summary>
    /// All the Setup of the Combat Abilities are scripted on the Children of this class
    /// </summary>
    public abstract class RiderCombatAbility : ScriptableObject
    {
        /// <summary>
        /// Rider Combat Reference
        /// </summary>
        protected RiderCombat RC;
        protected Transform cam;
        protected Animator Anim;

        public abstract bool TypeOfAbility(IMWeapon weapon);

        public abstract WeaponType WeaponType();

        /// <summary>
        /// Called on the Start of the Rider Combat Script
        /// </summary>
        public virtual void StartAbility(RiderCombat ridercombat)
        {
            RC = ridercombat;                                                               //Get the reference for the RiderCombat Script

            Camera camera = RC.rider.MainCamera;
            if (camera) cam = camera.transform;                                             //Get the camera from MainCamera
            Anim = RC.Anim;
        }


        /// <summary>
        /// Called when the Weapon is Equiped
        /// </summary>
        public virtual void ActivateAbility()
        { }

        /// <summary>
        /// Called on the FixedUpdate of the Rider Combat Script
        /// </summary>
        public virtual void FixedUpdateAbility()
        { }


        /// <summary>
        /// Called on the Update of the Rider Combat Script
        /// </summary>
        public virtual void UpdateAbility()
        {  }

        /// <summary>
        /// Called on the Late Update of the Rider Combat Script
        /// </summary>
        public virtual void LateUpdateAbility()
        { }

        /// <summary>
        /// Resets the Ability when there's no Active weapon
        /// </summary>
        public virtual void ResetAbility()
        {
            if (RC.Active_IMWeapon == null) return;

            if (RC.debug)
            {
                Debug.Log("Ability Reseted");
            }
        }

        public virtual void ListenAnimator(string Method, object value)
        {
            this.Invoke(Method, value);
        }

        /// <summary>
        /// If the Ability can change the Camera Side State for better Aiming and better looks
        /// </summary>
        public virtual bool ChangeAimCameraSide()
        {
            return true;
        }

        /// <summary>
        /// Stuff Set in the OnAnimatorIK
        /// </summary>
        /// <returns></returns>
        public virtual void IK()
        {
        }

        /// <summary>
        /// Can the Ability Aim
        /// </summary>
        public virtual bool CanAim()
        {
            return true;
        }


        public virtual Transform AimRayOrigin()
        {
            return (RC.Active_IMWeapon.RightHand ? RC.RightShoulder : RC.LeftShoulder);
        }

        /// <summary>
        /// Not Implemented Yet
        /// </summary>
        public virtual void OnActionChange()
        { }
    }
}