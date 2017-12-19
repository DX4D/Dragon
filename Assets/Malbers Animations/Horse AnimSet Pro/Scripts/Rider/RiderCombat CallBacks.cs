using UnityEngine;
using System.Collections;
using System.Reflection;
using MalbersAnimations.Weapons;

namespace MalbersAnimations.HAP
{
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    /// CALLBACKS
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    public partial class RiderCombat
    {
       /// <summary>
       ///Messages Get from the Animator
       /// </summary>
       public virtual void OnAnimatorBehaviourMessage(string message, object value)
        {
            this.InvokeWithParams(message, value);

            if (ActiveAbility)
            {
                ActiveAbility.ListenAnimator(message, value);
            }
        }

        public virtual void AddAbility(RiderCombatAbility newAbility)
        {
            newAbility.StartAbility(this);
            CombatAbilities.Add(newAbility);
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// This will be call by the Animator Layer "Rider Arm Right"  to free the Reins from the Right Hand 
        /// </summary>
        public virtual void RightHand_is_Free(bool value)
        {
            if (rider.Montura != null)
            {
                IKReins IK_Reins = rider.Montura.transform.GetComponent<IKReins>();

                if (IK_Reins)
                    IK_Reins.RightHand_is_Free(value);                                      //Send to the Reins Script that the  RIGHT Hand is free or not
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// This will be call by the Animator Layer "Rider Arm Left"  to free the Reins from the Left Hand 
        /// </summary>
        public virtual void LeftHand_is_Free(bool value)
        {
            if (rider.Montura != null)
            {
                IKReins IK_Reins = rider.Montura.transform.GetComponent<IKReins>();

                if (IK_Reins)
                    IK_Reins.LeftHand_is_Free(value); //Send to the Reins Script that the LEFT Hand is free or not
            }
        }

        /// <summary>
        /// Set the active Weapon
        /// </summary>
        public virtual void SetActiveWeapon(GameObject weapon)
        {
            activeWeapon = weapon;
        }

        /// <summary>
        /// Resets the Aim if is Aiming
        /// </summary>
        public virtual void ResetAiming()
        {
            if (IsAiming)
            {
                isAiming = false;                                                                           //Reset the aiming state
                OnAimSide.Invoke(0);                                                                        //Send that is not aiming anymore
                SetAction(IsInCombatMode ? WeaponActions.Idle : WeaponActions.None);
            }
        }

        /// <summary>
        /// Resets the Rider Combat Mode and DO Reset the Action to NONE, also ask for Store the Current Weapon
        /// </summary>
        /// <param name="storeWeapon">if the weapon needs to be stored when this method is called</param>
        public virtual void ResetRiderCombat(bool storeWeapon)
        {
            ResetRiderCombat();

            SetAction(WeaponActions.None);

            if (storeWeapon) Store_Weapon();  //Store when Dismounting

            LinkAnimator();
        }

        /// <summary>
        /// Sets IsinCombatMode=false, ActiveAbility=null,WeaponType=None and Resets the Aim Mode. DOES NOT RESET THE ACTION TO NONE
        /// </summary>
        public virtual void ResetRiderCombat()
        {
            IsInCombatMode = false;
            SetWeaponType(WeaponType.None);
            ResetActiveAbility();
            ResetAiming();
        }


        /// <summary>
        /// Execute Draw Weapon Animation without the need of an Active Weapon
        /// No Need of Active Weapons or IMweapon Interfaces
        /// </summary>
        /// <param name="holder">Which Holder the weapon is going to be draw from</param>
        /// <param name="weaponType">What type of weapon</param>
        /// <param name="isRightHand">Is it going to be draw with the left or the right hand</param>
        public virtual void Draw_Weapon(WeaponHolder holder, WeaponType weaponType, bool isRightHand = true)
        {
            EnableMountAttack(false);                                                                   //Disable Attack Animations of the Animal
            ResetRiderCombat();

            SetAction(isRightHand ? WeaponActions.DrawFromRight : WeaponActions.DrawFromLeft);

            SetWeaponIdleAnimState(weaponType, isRightHand);
            _weaponType = weaponType;

            LinkAnimator();
            if (debug) Debug.Log("Draw with No Active Weapon");  //Debug
        }

        /// <summary>
        /// Execute Store Weapon Animation without the need of an Active Weapon
        /// </summary>
        /// <param name="holder">The holder that the weapon is going to be Stored</param>
        /// <param name="isRightHand"></param>
        public virtual void Store_Weapon(WeaponHolder holder, bool isRightHand = true)
        {
            EnableMountAttack(true);                                                        //Enable Attack Animations of the Animal

            _weaponType = WeaponType.None;                                                  //Set the weapon ID to None (For the correct Animations)

            ActiveHolderSide = holder;
            SetAction(isRightHand ? WeaponActions.StoreToRight : WeaponActions.StoreToLeft); //Set the  Weapon Action to -1 to Store Weapons to Right or -2 to left

            LinkAnimator();
            ResetRiderCombat();
            if (debug) Debug.Log("Store with No Active Weapon");
        }

        /// <summary>
        /// Sets in the animator the Correct Idle Animation State for the Right/Left Hand Ex: "Melee Idle Right Hand" which is the exact name in the Animator
        /// </summary>
        public virtual void SetWeaponIdleAnimState(bool IsRightHand)
        {
            string WeaponIdle = " Idle " + (IsRightHand ? "Right" : "Left") + " Hand";

            switch (_weaponType)
            {
                case WeaponType.Melee:
                    WeaponIdle = "Melee" + WeaponIdle;        //Melee Idle Right/Left Hand
                    break;
                case WeaponType.Bow:
                    WeaponIdle = "Bow" + WeaponIdle;         //Bow Idle Right/Left Hand
                    break;
                case WeaponType.Pistol: 
                    WeaponIdle = "Pistol" + WeaponIdle;      //Pistol Idle Right/Left Hand
                    break;
                case WeaponType.Rifle:
                    WeaponIdle = "Rifle" + WeaponIdle;       //Rifle Idle Right/Left Hand
                    break;
                default:
                    WeaponIdle = "Melee" + WeaponIdle;
                    break;
            }

            _anim.CrossFade(WeaponIdle, 0.25f, Active_IMWeapon.RightHand ? Layer_RiderArmRight : Layer_RiderArmLeft); //Active the Layer Right/Left Arm for Idle Pose
        }

        /// <summary>
        /// Sets in the animator the Correct Idle Animation State for the Right/Left Hand 
        /// Ex: "Melee Idle Right Hand" which is the exact name in the Animator
        /// No Need of IMWeapon Interface
        /// </summary>
        public virtual void SetWeaponIdleAnimState(WeaponType weapon, bool isRightHand)
        {
            SetWeaponType(weapon);
            SetWeaponIdleAnimState(isRightHand);
        }

        /// <summary>
        /// Returns the Weapon type
        /// </summary>
        public virtual WeaponType GetWeaponType()
        {
            if (Active_IMWeapon != null)
            {
                if (Active_IMWeapon is IMelee) return WeaponType.Melee;
                if (Active_IMWeapon is IBow) return WeaponType.Bow;
                if (Active_IMWeapon is IGun) return WeaponType.Pistol;
            }

            return WeaponType.None;
        }


        /// <summary>
        /// Set the WeaponType
        /// </summary>
        public virtual void SetWeaponType(WeaponType weapon)
        {
            _weaponType = weapon;
            _anim.SetInteger(Hash_WeaponType, (int)_weaponType);          //Set the WeaponType in the Animator
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///Get a Callback From the RiderCombat Layer Weapons States
        /// </summary>
        public virtual void WeaponSound(int SoundID)
        {
            if (Active_IMWeapon != null)
                Active_IMWeapon.PlaySound(SoundID);
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///Set if the rider is in Combat Mode
        /// </summary>
        public void CombatMode(bool active)
        {
            isInCombatMode = active;
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///This will recieve the messages Animator Behaviors the moment the rider make an action on the weapon
        /// </summary>
        /// <param name="value"> the Integer Value of the Action</param>
        public virtual void Action(int value)
        {
            SetAction ((WeaponActions)value);
        }
        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Updates the Weapon Action on this script an on the animator 
        /// </summary>
        public virtual void SetAction(WeaponActions action)
        {  
            _weaponAction = action;
            _anim.SetInteger(Hash_WeaponAction,(int) action);
            OnWeaponAction.Invoke(_weaponAction);
        }

        /// <summary>
        /// Updates the Attack1 Parameter in the animator
        /// </summary>
        public void Attack1(bool v)
        {
            _anim.SetBool(Hash.Attack1, v);
        }

        /// <summary>
        /// Updates the Attack2 Parameter in the animator
        /// </summary>
        public void Attack2(bool v)
        {
            _anim.SetBool(Hash.Attack2, v);
        }
    }
}