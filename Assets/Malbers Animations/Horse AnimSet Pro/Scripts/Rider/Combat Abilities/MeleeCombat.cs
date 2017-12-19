using MalbersAnimations.Weapons;
using UnityEngine;

namespace MalbersAnimations.HAP
{
    [CreateAssetMenu(menuName = "Malbers Animations/HAP/MeleeCombat")]
    public class MeleeCombat : RiderCombatAbility
    {
        [Tooltip("Time before attacking again with melee")]
        public float meleeAttackDelay = 0.5f;

        bool isAttacking = false;
        float timeOnAttack = 0;

        public override bool TypeOfAbility(IMWeapon weapon)
        {
            return weapon is IMelee;
        }

        public override WeaponType WeaponType()
        {
            return MalbersAnimations.WeaponType.Melee;
        }

        public override void UpdateAbility()
        {
            if (isAttacking)
            {
                if (Time.time - timeOnAttack > meleeAttackDelay)
                {
                    isAttacking = false;
                }
            }
            if (RC.InputAttack1.GetInput && !isAttacking) RiderMeleeAttack(false);                 //Attack with Left Hand
            if (RC.InputAttack2.GetInput && !isAttacking) RiderMeleeAttack(true);                  //Attack with Right Hand

           
        }

        public override void ActivateAbility()
        {
            timeOnAttack = 0;
            isAttacking = false;
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Set all parameters for Melee Attack
        /// </summary>
        /// <param name="rightSide">true = Right Arm.. false = Left Arm</param>
        protected virtual void RiderMeleeAttack(bool rightSide)
        {
            RC.Anim.SetInteger(Hash.IDInt, -99);            //Avoid to execute the Lower Attack Animation clip for the rider

            if (rightSide)
                RC.Anim.SetBool(Hash.Attack2, true);        //Set in the Animator Attack2 to true
            else
                RC.Anim.SetBool(Hash.Attack1, true);        //Set in the Animator Attack1 to true

            int attackID;

            if (RC.Active_IMWeapon.RightHand)                           //If the Active Weapon Is Right Handed
            {
                if (rightSide) attackID = Random.Range(1, 3);           // Set the Attacks for the RIGHT Side with the 'Right Hand'
                else attackID = Random.Range(3, 5);                     // Set the Attacks for the LEFT Side with the 'Right Hand'
            }
            else                                                        //Else Active Weapon is Left Handed
            {
                if (rightSide) attackID = Random.Range(7, 9);           // Set the Attacks for the RIGHT Side with the 'Left Hand'
                else attackID = Random.Range(5, 7);                     // Set the Attacks for the LEFT Side with the 'Left Hand'
            }

            RC.Anim.SetInteger(RiderCombat.Hash_WeaponAction, attackID);//Set Attack ID

            isAttacking = true;
            timeOnAttack = Time.time;

            RC.OnAttack.Invoke(RC.Active_IMWeapon);                      //Invoke the OnAttack Event
        }


        /// <summary>
        /// Call From the Animator in the melee state that the weapon can cause damage
        /// </summary>
        public virtual void OnCauseDamage(bool value)
        {
            (RC.Active_IMWeapon as IMelee).CanDoDamage(value);
        }


        /// <summary>
        /// Can the Ability Aim
        /// </summary>
        public override bool CanAim()
        {
            return false;
        }
    }
}