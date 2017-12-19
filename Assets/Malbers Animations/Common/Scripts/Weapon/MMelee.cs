using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MalbersAnimations.Events;
using MalbersAnimations.Utilities;
using UnityEngine.Events;

namespace MalbersAnimations.Weapons
{
    public class MMelee : MWeapon, IMelee
    {
        protected bool isOnAtackingState;                         //If the weapon is attacking
        protected bool canCauseDamage;                      //The moment in the Animation the weapon can cause Damage        

        public Collider meleeCollider;
        protected List<Transform> AlreadyHitted = new List<Transform>();

        public GameObjectEvent OnHit;
        public BoolEvent OnCauseDamage;

        
        public bool CanCauseDamage
        {
            get { return canCauseDamage; }
            set
            {
                canCauseDamage = value;
                AlreadyHitted = new List<Transform>();  //Reset the list of transform that I already Hit

                if (!meleeCollider.isTrigger)
                {
                    meleeCollider.enabled = canCauseDamage;
                }
            }
        }

        protected ColliderProxy meleeColliderProxy;

        //The Attack Momentum while the Animation is playing
        public virtual void CanDoDamage(bool value)
        {
            CanCauseDamage = value;
            OnCauseDamage.Invoke(value);
            meleeCollider.enabled = value;
        }

        void Start()
        {
            Invoke("InitializeWeapon",0.02f); //Next Frame
            //InitializeWeapon();
        }

        public override void InitializeWeapon()
        {
            base.InitializeWeapon();

            if (meleeCollider)
            {
                meleeColliderProxy = meleeCollider.gameObject.AddComponent<ColliderProxy>();            //Create a proxy to comunicate the Collision and trigger events in case the melee conllider is on another gameObject

                if (meleeCollider.isTrigger)
                {
                    meleeColliderProxy.OnTrigger_Stay.AddListener(WeaponTriggerStay);
                }
                else
                {
                    meleeColliderProxy.OnCollision_Enter.AddListener(WeaponCollisionEnter);
                }
            }
        }
        

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Send Damage
        /// </summary>
        protected virtual void WeaponCollisionEnter(Collision other)
        {
            if (other.contacts.Length == 0) return;

            SetDamageStuff(other.contacts[0].point, other.transform);
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Set Damage if you're using triggers
        /// </summary>
        protected virtual void WeaponTriggerStay(Collider other)
        {
            SetDamageStuff(other.ClosestPointOnBounds(meleeCollider.bounds.center), other.transform);
        }


        internal void SetDamageStuff(Vector3 OtherHitPoint, Transform other)
        {
            if (other.root == transform.root) return; //if Im hitting my horse

            if (!(hitMask == (hitMask | (1 << other.gameObject.layer)))) return; //Just hit what is on the HitMask Layer

            DV = new DamageValues(meleeCollider.bounds.center - OtherHitPoint, Random.Range(MinDamage, MaxDamage));

            Debug.DrawLine(OtherHitPoint, meleeCollider.bounds.center, Color.red, 3f);


            if (canCauseDamage && !AlreadyHitted.Find(item => item == other.transform.root))                        //If can cause damage and If I didnt hit the same transform twice
            {
                AlreadyHitted.Add(other.transform.root);
                other.transform.root.SendMessage("getDamaged", DV, SendMessageOptions.DontRequireReceiver);         //To everybody who has GetDamaged()


                Rigidbody OtherRB = other.transform.root.GetComponent<Rigidbody>();

                if (OtherRB && other.gameObject.layer != 20)                                                        //Apply Force if the game object has a RigidBody && if is not an Animal
                {
                    OtherRB.AddExplosionForce(MinForce * 50, OtherHitPoint, 20);
                }

                PlaySound(3);                                                                                       //Play Hit Sound when get something

                OnHit.Invoke(other.gameObject);

                if (!meleeCollider.isTrigger)
                {
                    meleeCollider.enabled = false;
                }
            }
        }


        /// <summary>
        /// Disable Listeners
        /// </summary>
        void OnDisable()
        {
            if (meleeColliderProxy)
            {
                if (meleeCollider.isTrigger)

                    meleeColliderProxy.OnTrigger_Stay.RemoveListener(WeaponTriggerStay);
                else
                    meleeColliderProxy.OnCollision_Enter.RemoveListener(WeaponCollisionEnter);
            }
        }
    }
}