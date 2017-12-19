using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace MalbersAnimations.Weapons
{
    [System.Serializable]
    public class WeaponEvent : UnityEvent<IMWeapon>{}
    [System.Serializable]
    public class WeaponActionEvent : UnityEvent<WeaponActions>{}

    public abstract class MWeapon : MonoBehaviour, IMWeapon
    {
        public int weaponID;                                // Weapon ID unique Number

        public float minDamage = 10;                             //Weapon minimum Damage
        public float maxDamage = 20;                             //Weapon Max Damage
        public float minForce = 500;                              //Weapon min Force to push rigid bodies;
        public float maxForce = 1000;                              //Weapon max Force to push rigid bodies;


        public bool rightHand = true;                       // With which hand you will draw the Weapon;

        public WeaponHolder holder = WeaponHolder.None;     // From which Holder you will draw the Weapon
        public LayerMask hitMask;

        public Vector3
               positionOffset,                              //Position Offset
               rotationOffset;                              //Rotation Offset

        protected DamageValues DV;                          //Direction and DamageAmount for the Weapon
        public AudioClip[] Sounds;                          //Sounds for the weapon
        public AudioSource WeaponSound;                     //Reference for the audio Source;

        #region Properties
        public int WeaponID
        {
            get { return weaponID; }
        }

        public WeaponHolder Holder
        {
            get { return holder; }
            set { holder = value; }
        }

        public float MinDamage
        {
            get { return minDamage; }
        }

        public float MaxDamage
        {
            get { return maxDamage; }
        }

        public bool RightHand
        {
            get { return rightHand; }
        }

        public Vector3 PositionOffset
        {
            get { return positionOffset; }
        }

        public Vector3 RotationOffset
        {
            get { return rotationOffset; }
        }

        public float MinForce
        {
            get { return minForce; }
            set { minForce = value; }
        }

        public float MaxForce
        {
            get { return maxForce; }
            set { maxForce = value; }
        }


        public LayerMask HitMask
        {
            get { return hitMask; }
            set { hitMask = value; }
        }
        #endregion

        public WeaponEvent OnEquiped = new WeaponEvent();
        public WeaponEvent OnUnequiped = new WeaponEvent();

        /// <summary>
        /// Returns True if the Weapons has the same ID
        /// </summary>
        public override bool Equals(object a)
        {
            if (a is IMWeapon)
            {
                if (weaponID == (a as IMWeapon).WeaponID)
                    return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void Equiped()
        {
            OnEquiped.Invoke(this);
        }

        public virtual void Unequiped()
        {
            OnUnequiped.Invoke(this);
        }

        public virtual void InitializeWeapon()
        {
            WeaponSound = GetComponent<AudioSource>();

            if (!WeaponSound)
                WeaponSound = gameObject.AddComponent<AudioSource>(); //Create an AudioSourse if theres no Audio Source on the weapon

            WeaponSound.spatialBlend = 1;
        }

        /// CallBack from the RiderCombat Layer in the Animator to reproduce a sound on the weapon
        public virtual void PlaySound(int ID)
        {
            if (ID > Sounds.Length - 1) return;

            if (Sounds[ID] != null && WeaponSound)
            {
                WeaponSound.PlayOneShot(Sounds[ID]);   //Play Draw/ Weapon Clip
            }
        }
      
        ///Editor
       [HideInInspector] public bool ShowEventEditor;
    }
}