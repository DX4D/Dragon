using UnityEngine;
using System.Collections;

namespace MalbersAnimations.HAP
{
    [RequireComponent(typeof(WagonController))]
    public class MountableCarriage : Mountable
    {
        protected Transform hip;

        /// <summary>
        /// Gets the Master Animal Reference
        /// </summary>
        public override Animal Animal
        {
            get
            {
                if (!_animal)
                {
                    WagonController WG = GetComponent<WagonController>();

                    if (WG.HorseRigidBody == null) return null;

                    PullingHorses PH = WG.HorseRigidBody.GetComponent<PullingHorses>();
                    if (PH)
                    {
                        _animal = PH.RightHorse.GetComponent<Animal>();
                    }
                    else
                    {
                        _animal = WG.HorseRigidBody.GetComponent<Animal>();
                    }
                }
                return _animal;
            }
        }


        public override bool CanDismount
        {
            get
            {
                if (Animal != null)
                {
                    return Animal.Stand;
                }
                return true;
            }
        }

        public override void EnableControls(bool value)
        {
            WagonController WG = GetComponent<WagonController>();

            if (WG.HorseRigidBody == null) return;

            PullingHorses PH = WG.HorseRigidBody.GetComponent<PullingHorses>();

            if (PH)
            {
                if (PH.RightHorse)
                {
                    PH.RightHorse.MovementAxis = Vector3.zero;
                    PH.RightHorse.GetComponent<MalbersInput>().enabled = value;
                }

                if (PH.LeftHorse)
                {
                    PH.RightHorse.MovementAxis = Vector3.zero;
                    PH.LeftHorse.GetComponent<MalbersInput>().enabled = value;
                }
              
            }
            else
            {
                WG.HorseRigidBody.GetComponent<Animal>().GetComponent<MalbersInput>().enabled = value;
            }
        }

        //public override void SyncAnimator(Animator anim)
        //{
        //    if (Animal == null) return;

        //    anim.SetFloat(Hash.Vertical, Animal.Speed);
        //    anim.SetFloat(Hash.Horizontal, Animal.Direction / 2);
        //    anim.SetBool(Hash.Stand, Animal.Stand);
        //    if (Animal.SpeedHasChanged)
        //    {
        //        anim.SetInteger(Hash.IDInt, -10);
        //    }
        //    else
        //    {
        //        if (!Animal.Stand)
        //        {
        //            anim.SetInteger(Hash.IDInt, 0);
        //        }
        //    }
        //}

    }
}
