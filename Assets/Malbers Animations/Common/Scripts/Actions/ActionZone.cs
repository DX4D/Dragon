using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MalbersAnimations.Events;
using UnityEngine.Events;
using System;
using MalbersAnimations.Utilities;

namespace MalbersAnimations
{
    [RequireComponent(typeof(BoxCollider))]
    public class ActionZone : MonoBehaviour
    {
        static Keyframe[] K = { new Keyframe(0, 0), new Keyframe(1, 1) };

        public Actions actionsToUse;

        public bool automatic;                          //Set the Action Zone to Automatic
        public int ID;                                  //ID of the Action Zone (Value)
        public int index;                               //Index of the Action Zone (List index)
        public float AutomaticDisabled = 10f;            //is Automatic is set to true this will be the time to disable temporarly the Trigger
        public bool HeadOnly;                           //Use the Trigger for heads only
        public bool ActiveOnJump = false;


        public bool Align;                              //Align the Animal entering to the Aling Point
        public Transform AlingPoint;
        public float AlignTime = 0.5f;
        public AnimationCurve AlignCurve = new AnimationCurve(K);

        public bool AlignPos = true, AlignRot = true, AlignLookAt = false;

        protected List<Collider> _colliders;
        protected Animal animal;

        public AnimalEvent OnEnter = new AnimalEvent();
        public AnimalEvent OnExit = new AnimalEvent();
        public AnimalEvent OnAction = new AnimalEvent();


        public static List<ActionZone> ActionZones;

        //───────AI──────────────────────────
        public float stoppingDistance = 0.5f;
        public Transform NextTarget;


        Collider _collider;

        void OnEnable()
        {
            if (ActionZones == null) ActionZones = new List<ActionZone>();

            _collider = GetComponent<Collider>();                                   //Get the reference for the collider

            ActionZones.Add(this);                                                  //Save the the Action Zones on the global Action Zone list
        }

        void OnDisable()
        {
            ActionZones.Remove(this);                                              //Remove the the Action Zones on the global Action Zone list
            if (animal) animal.OnAction.RemoveListener(OnActionListener);
        }


        void OnTriggerEnter(Collider other)
        {
            Animal animal = other.GetComponentInParent<Animal>();               //Get the animal on the entering collider

            if (other.gameObject.layer != 20) return;                           //Just use the Colliders with the Animal Layer on it
                
            if (!animal) return;                                                //If there's no animal script found do nothing

            if (_colliders == null)
                _colliders = new List<Collider>();                              //Check all the colliders that enters the Action Zone Trigger

            if (HeadOnly && !other.name.ToLower().Contains("head")) return;     //If is Head Only and no head was found Skip

            if (_colliders.Find(item => item == other) == null)                 //if the entering collider is not already on the list add it
            {
                _colliders.Add(other);
            }

            if (animal == this.animal) return;                                  //if the animal is the same do nothing (when entering two animals on the same Zone)
            else
            {
                this.animal = animal;
            }

            animal.OnAction.AddListener(OnActionListener);                      //Listen when the animal activate the Action Input

            OnEnter.Invoke(animal);
            animal.ActionID = ID;

            if (automatic)       //Just activate when is on the Locomotion State if this is automatic
            {
                if (animal.CurrentAnimState.tagHash == Hash.Tag_Jump && !ActiveOnJump) return;   //Dont start an automatic action if is jumping and active on jump is disabled
               
                animal.SetAction(ID);
                StartCoroutine(EnableAction());
                StartCoroutine(ReEnable(animal));
            }
        }

        /// <summary>
        /// Keep Enabling the Action until enters the action animation (Malbers Input overrides the Action input by seting it to false again)
        /// </summary>
        IEnumerator EnableAction()
        {
            while (animal && animal.CurrentAnimState.tagHash != Hash.Action)
            {
                animal.Action = true;
                animal.ActionID = ID;
                yield return null;
            }
            animal.Action = false;
        }

        void OnTriggerExit(Collider other)
        {
            Animal O_animal = other.GetComponentInParent<Animal>();
            if (!O_animal) return;                                                    //If there's no animal script found skip all

            if (O_animal != animal) return;                                      //If is another animal exiting the zone SKIP

            if (HeadOnly && !other.name.ToLower().Contains("head")) return;         //if is only set to head and there's no head SKIP

            if (_colliders.Find(item => item == other))                             //Remove the collider from the list that is exiting the zone.
            {
                _colliders.Remove(other);
            }

            if (_colliders.Count == 0)                                              //When all the collides are removed from the list..
            {
                OnExit.Invoke(animal);                                              //Invoke On Exit when all animal's colliders has exited the Zone
                animal.OnAction.RemoveListener(OnActionListener);                   //Remove the Method fron the Action Listener

                if (animal.ActionID == ID) animal.ActionID = -1;                    //Reset the Action ID if we have the same

                animal = null;
            }
        }

        /// <summary>
        /// This will disable the Collider on the action zone
        /// </summary>
        IEnumerator ReEnable(Animal animal) //For Automatic only 
        {
            if (AutomaticDisabled > 0)
            {
                _collider.enabled = false;
                yield return null;
                yield return null;
                animal.ActionID = -1;
                yield return new WaitForSeconds(AutomaticDisabled);
                _collider.enabled = true;
            }
            this.animal = null;     //Reset animal
            _colliders = null;      //Reset Colliders
            yield return null;
        }

        public virtual void _DestroyActionZone(float time)
        {
            Destroy(gameObject, time);
        }

        /// <summary>
        /// Used for checking if the animal enables the action
        /// </summary>
        private void OnActionListener()
        {
            if (!animal) return;                            //Skip if there's no animal

            OnAction.Invoke(animal);                        //Invole the Event OnAction

            if (Align && AlingPoint)
            {
                IEnumerator ICo = null;

                if (AlignLookAt)
                {
                    ICo = MalbersTools.AlignLookAtTransform(animal.transform, AlingPoint, AlignTime, AlignCurve);     //Align Look At the Zone
                }
                else
                {
                    ICo = MalbersTools.AlignTransformsC(animal.transform, AlingPoint, AlignTime, AlignPos, AlignRot, AlignCurve); //Aling Transform to another transform
                }

                StartCoroutine(ICo);
            }

            StartCoroutine(CheckForCollidersOff());
        }

        IEnumerator CheckForCollidersOff()
        {
            yield return null;      
            yield return null;          //Wait 2 frames


            if (_colliders != null && _colliders[0] && _colliders[0].enabled == false)
            {
                animal.OnAction.RemoveListener(OnActionListener);
                animal.ActionID = -1;
                animal = null;
                _colliders = null;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (EditorAI)
            {
                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, stoppingDistance);
            }
        }
#endif

        [HideInInspector] public bool EditorShowEvents = true;
        [HideInInspector] public bool EditorAI = true;
    }
}