using System;
using MalbersAnimations.Weapons;
using UnityEngine;

namespace MalbersAnimations.HAP
{
    /// <summary>
    /// This Class holds the common methods for GunCombatIK and GunCombatFK
    /// </summary>
    public abstract class GunCombat : RiderCombatAbility
    {
        protected bool isReloading;                  //If the gun is currently Reloading
        protected float currentFireRateTime = 0;
        protected InputButton DefaultInputType;

        public override bool TypeOfAbility(IMWeapon weapon)
        {
            return weapon is IGun;
        }

        public override WeaponType WeaponType()
        {
            return MalbersAnimations.WeaponType.Pistol;
        }


        public override void StartAbility(RiderCombat ridercombat)
        {
            base.StartAbility(ridercombat);
            DefaultInputType = RC.InputAttack1.GetPressed;              //Get the original ButtonType for Automatic or One Shoot
        }


        public override void ActivateAbility()
        {
            IGun Gun = RC.Active_IMWeapon as IGun;  //Store the Current Gun
            isReloading = false;
            if (Gun == null) return;

            if (!Gun.IsAutomatic) RC.InputAttack1.GetPressed = InputButton.Down;  //if the gun is not automatic change the Attack input button type to down
            currentFireRateTime = 0;
        }


        public override void UpdateAbility()
        {
            IGun Gun = RC.Active_IMWeapon as IGun;  //Store the Current Gun
            if (Gun == null) return;

            if (Gun.IsAiming != RC.IsAiming)
            {
                Gun.IsAiming = RC.IsAiming;
            }

            if (RC.Reload.GetInput)                                                         //Check For Reload
                PistolReload(RC.Active_IMWeapon as IGun);

            if (RC.IsAiming && RC.WeaponAction != WeaponActions.Fire_Proyectile)
            {   // && Gun.FireRate < (Time.time - currentFireRateTime))            //If is aiming and is not already firing a proyectile
                PistolAttack(Gun);                                                          //Fire Pistol
            }
            //Debug.Log((Time.time - currentFireRateTime));
        }


        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Pistol Attack Mode
        /// </summary>
        protected virtual void PistolAttack(IGun Gun)
        {
            if (isReloading) return;


            if (Gun.AmmoInChamber > 0)                                                                                 //If there's Ammo on the chamber
            {
                if (RC.InputAttack1.GetInput)                                                                //If Attack Is pressed
                {


                    if (RC.WeaponAction == WeaponActions.Fire_Proyectile)
                    {
                        //Mefalta interrumpir el mismo tiro
                    }

                    RC.SetAction(WeaponActions.Fire_Proyectile);                                            //Set the Weapon Action to Fire Proyectile  

                    // RC.Anim.SetInteger(HashCombat.IDIntHash, -99);                                            //Avoid to execute the Lower Attack Animation clip (HorseSyncAnimation) for the rider
                    // RC.Anim.SetBool(HashCombat.attack1Hash, true);                                            //Set Attack1 = True in the Animator

                    Gun.FireProyectile(RC.AimRayCastHit);

                    RC.OnAttack.Invoke(RC.Active_IMWeapon);                                                        //Invoke the On Attack Event
                    currentFireRateTime = Time.time;
                }
            }
            else
            {
                if (RC.InputAttack1.GetInput)                                                                   //When there's no bullets in the chamber
                {
                    if (Time.time - currentFireRateTime > 0.5f)
                    {
                        Gun.PlaySound(4);                                                                       //Play Empty Sound Which is stored in the 4 Slot     
                        currentFireRateTime = Time.time;
                    }
                }
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// If the Weapon is a Gun Reload
        /// </summary>
        public virtual void PistolReload(IGun Gun)
        {
            if (Gun.Reload())                                                                                       //If the Gun can Reload Activate the Reload Animation and Reload
            {
                RC.SetAction((RC.Active_IMWeapon.RightHand ? WeaponActions.ReloadRight : WeaponActions.ReloadLeft));     //Set the Animator to the Reload Animations
            }
            else
            {
                RC.SetAction(WeaponActions.Idle);
            }
        }


        public override void ResetAbility()
        {
            base.ResetAbility();

            IGun Gun = RC.Active_IMWeapon as IGun;  //Store the Current Gun

            if (Gun == null) return;
            isReloading = false;

            if (!(Gun.IsAutomatic)) RC.InputAttack1.GetPressed = DefaultInputType; //Set back to the default Pressed button Type

            EnableAimIKBehaviour(false);
        }


        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// If finish reload but is still aiming go to the aiming animation
        /// </summary>
        public virtual void FinishReload()
        {
            RC.SetAction(RC.IsAiming ?
                (RC.Active_IMWeapon.RightHand ? WeaponActions.AimRight : WeaponActions.AimLeft) : WeaponActions.Idle);
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Set if is in the reloading state
        /// </summary>
        public void IsReloading(bool value)
        {
            isReloading = value;
        }


        /// <summary>
        /// Invoked from the Animator
        /// </summary>
        public void ResetDoubleShoot()
        {
            Anim.SetInteger(Hash.IDInt, 0);
        }

        protected void EnableAimIKBehaviour(bool value)
        {
            AimIKBehaviour[] aimIK = Anim.GetBehaviours<AimIKBehaviour>();

            foreach (var item in aimIK)
            {
                item.active = value;
            }
        }
    }
}