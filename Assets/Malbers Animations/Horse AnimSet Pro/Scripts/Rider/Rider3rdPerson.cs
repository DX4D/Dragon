using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MalbersAnimations.Events;

namespace MalbersAnimations.HAP
{
    public class Rider3rdPerson : Rider 
    {
        #region IK VARIABLES    
        protected float L_IKFootWeight = 0f;        //IK Weight for Left Foot
        protected float R_IKFootWeight = 0f;        //IK Weight for Right Foot
        #endregion

        public UnityEvent
              OnStartMounting,
              OnEndMounting,
              OnStartDismounting,
              OnEndDismounting,
              OnAlreadyMounted;

        public int MountLayerIndex = -1;               //Mount Layer Hash

        protected AnimatorUpdateMode Initial_UpdateMode;

        public override Mountable Montura
        {
            get { return montura; }
            set
            {
                montura = value;
                MountLayerIndex = value != null ? Anim.GetLayerIndex(Montura.MountLayer) : -1;   //Gets the layer mask of the Montura that you just found
            }
        }


        /// Set all the References
        void Awake()
        {
            _transform = transform;
            if (!Anim) Anim = GetComponentInChildren<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponents<Collider>();

            MountLayerIndex = -1;                       //Resets the Mounting Layer;

        

        }

        void Start()
        {
            IsOnHorse = Mounted = false;                                    //initialize in false
            if (Anim) Initial_UpdateMode = Anim.updateMode;           //Gets the Update Mode of the Animator to restore later when dismounted.

            if (StartMounted) AlreadyMounted();                             //Set All if Started Mounted is Active
        }

        /// <summary>
        ///Set all the correct atributes and variables to Start Mounted on the next frame
        /// </summary>
        public void AlreadyMounted()
        {
            StartCoroutine(AlreadyMountedC());
        }

        IEnumerator AlreadyMountedC()
        {
            yield return null;      //Wait for the next frame

            if (AnimalStored != null && StartMounted)
            {
                Montura = AnimalStored;

                if (MountTrigger == null) Montura.transform.GetComponentInChildren<MountTriggers>(); //Save the first Mount trigger you found

                Start_Mounting();
                End_Mounting();


                if (Anim) Anim.Play(Montura.MountIdle, MountLayerIndex);               //Play Mount Idle Animation Directly

              
                toogleCall = true;                                                      //Turn Off the Call

                Montura.Mounted = Mounted = true;                                      //Send to the animalMount that mounted is active

                Anim.SetBool(Hash.Mount, mounted);    //Update Mount Parameter In the Animator
            }

            Montura.Animal.OnSyncAnimator.AddListener(SyncAnimator);                    //Add the Sync Method to sync all parameters
            OnAlreadyMounted.Invoke();
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///This is call at the Beginning of the Mount Animations
        /// </summary>
        public override void Start_Mounting()
        {
            if (Anim)
            {
                Anim.SetLayerWeight(MountLayerIndex, 1);                                //Enable Mount Layer set the weight to 1
                Anim.SetBool(Hash.Mount, Mounted);                                      //Update the Mount Parameter on the Animator
            }

            if (!MountTrigger)
                MountTrigger = Montura.transform.GetComponentInChildren<MountTriggers>();    //If null add the first mount trigger found

            if (DisableComponents)
            {
                ToggleComponents(false);                                                //Disable all Monobehaviours breaking the Mount System
            }

            base.Start_Mounting();
            OnStartMounting.Invoke();                                                   //Invoke UnityEvent for  Start Mounting

            if (!Anim) End_Dismounting();                                               //If is there no Animator  execute the End_Dismounting part
        }


        public override void End_Mounting()
        {
            base.End_Mounting();

            if (Anim) Anim.updateMode = AnimatorUpdateMode.Normal;                      //Needed to make IK Stuffs Manually like Bow, Pistol etc...

            OnEndMounting.Invoke();
        }

        public override void Start_Dismounting()
        {
            base.Start_Dismounting();

            if (Anim) Anim.updateMode = Initial_UpdateMode;                      //Restore Update mode to its original

            OnStartDismounting.Invoke();
        }
        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///This is call at the end of the Dismounting Animations States on the animator
        /// </summary>
        public override void End_Dismounting()
        {
            if (Anim && MountLayerIndex != -1) Anim.SetLayerWeight(MountLayerIndex, 0);                       //Reset the Layer Weight to 0 when end dismounting
            base.End_Dismounting();

            if (DisableComponents) ToggleComponents(true);                          //Enable all Monobehaviours breaking the Mount System
            OnEndDismounting.Invoke();                                              //Invoke UnityEvent when is off Animal
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Syncronize the Horse/Rider animations if Rider loose sync with the animal on the locomotion state
        /// </summary>
        private void Animators_ReSync()
        {
            if (!Anim) return;
            if (!Montura.syncAnimators) return;                                                                     //Don't Sync animators when that parameter is disabled

            if (Montura.Animal.Anim.GetCurrentAnimatorStateInfo(0).tagHash == Hash.Locomotion)                      //Search for syncron the locomotion state on the animal
            {
                 RiderNormalizedTime = Anim.GetCurrentAnimatorStateInfo(MountLayerIndex).normalizedTime;            //Get the normalized time from the Rider
                 HorseNormalizedTime = Montura.Animal.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;           //Get the normalized time from the Horse

                syncronize = true;

                if (Mathf.Abs(RiderNormalizedTime - HorseNormalizedTime) > 0.1f && Time.time - LastSyncTime > 1f)     //Checking if the animal and the rider are unsync by 0.2
                {
                    Anim.CrossFade(Hash.Locomotion, 0.2f, MountLayerIndex, HorseNormalizedTime);                      //Normalized with blend
                    LastSyncTime = Time.time;
                }
            }
            else
            {
                syncronize = false;
                RiderNormalizedTime = HorseNormalizedTime = 0;

            }
        }

        //Used this for Sync Animators
        private float RiderNormalizedTime;
        private float HorseNormalizedTime;
        private float LastSyncTime;
        private bool syncronize;

        void Update()
        {
            if (MountInput.GetInput)                                        //If the mount Key is pressed
            {
                SetMounting();
            }

            //───────────────────────────────────────────────────────────────────────────────────────────────────────
            if (IsRiding && Montura != null)                                          //Run Stuff While Mounted
            {
                WhileIsMounted();
            }
        }

        /// <summary>
        /// Set if the Rider can Mount
        /// </summary>
        public virtual void SetMounting()
        {
            if (CanMount)                                               //if are near an animal and we are not already on an animal
            {
                Run_Mounting();                                         //Run mounting Animations
            }
            else if (CanDismount)                                       //if we are already mounted and the animal is not moving (Mounted && IsOnHorse && Montura.CanDismount)
            {
                Run_Dismounting();                                      //Run Dismounting Animations
            }
            else if (!can_Mount && !Mounted && !IsOnHorse)              //if there is no horse near, call the animal in the slot
            {
                CallAnimal();
            }
        }

        protected virtual void WhileIsMounted()
        {
            Animators_ReSync();                                  //Check the syncronization and fix it if is offset***
        }

        protected void SyncAnimator()
        {
            float speed = Montura.Animal.Speed;

            if (Montura.Animal.Fly) speed = Mathf.Clamp(speed * 2, 0, 2); //Just set when is flying to Locomotion Trot on the Rider
         

            Anim.SetFloat(Hash.Vertical, speed);
            Anim.SetFloat(Hash.Horizontal, Montura.Animal.Direction);
            Anim.SetFloat(Hash.Slope, Montura.Animal.Slope);
            Anim.SetBool(Hash.Stand, Montura.Animal.Stand);

            Anim.SetBool(Hash._Jump, !Montura.Animal.Fly ? Montura.Animal.Jump : false); //if Fly is active don't access the fly

            Anim.SetBool(Hash.Attack1, Montura.Animal.Attack1);
            Anim.SetBool(Hash.Shift, Montura.Animal.Attack2 ? false : Montura.Animal.Shift);

            Anim.SetBool(Hash.Damaged, Montura.Animal.Damaged);

            Anim.SetBool(Hash.Stunned, Montura.Animal.Stun);
            Anim.SetBool(Hash.Action, Montura.Animal.Action);

            Anim.SetInteger(Hash.IDAction, Montura.Animal.ActionID);
            Anim.SetInteger(Hash.IDInt, Montura.Animal.IDInt);             
            Anim.SetFloat(Hash.IDFloat, Montura.Animal.IDFloat);

            if (Montura.Animal.canSwim) Anim.SetBool(Hash.Swim, Montura.Animal.Swim);
        }

        protected virtual void Run_Mounting()
        {
            if (Montura == null) return;

            Montura.Mounted =  Mounted = true;                               //Send to the animalMount that mounted is active

            Montura.Animal.OnSyncAnimator.AddListener(SyncAnimator);        //Add the Sync Method to sync all parameters

            if (Anim)
            {
                Anim.SetLayerWeight(MountLayerIndex, 1);                            //Enable the Mounting layer  
                Anim.SetBool(Hash.Mount, Mounted);                                  //Update Mount Parameter on the Animator
            }

            if (!Montura.InstantMount)
            {
                if (Anim) Anim.Play(MountTrigger.MountAnimation, MountLayerIndex);      //Play the Mounting Animations
            }
            else
            {
                Start_Mounting();
                End_Mounting();
                if (Anim) Anim.Play(Montura.MountIdle, MountLayerIndex);                //Ingore the Mounting Animations
            }
        }

        protected virtual void Run_Dismounting()
        {
            Montura.Mounted = Mounted = false;                                  //Unmount the Animal

            Montura.Animal.OnSyncAnimator.RemoveListener(SyncAnimator);        //Add the Sync Method to sync all parameters

            if (Anim)
            {
                Anim.SetBool(Hash.Mount, Mounted);                          //Update Mount Parameter In the Animator
                Anim.SetInteger(Hash.MountSide,MountTrigger.DismountID);    //Update MountSide Parameter In the Animator
            }

            if (Montura.InstantMount)                                       //Use for Instant mount
            {
                Anim.Play(Hash.Null, MountLayerIndex);

                Anim.SetInteger(Hash.MountSide,0);                          //Update MountSide Parameter In the Animator
                Start_Dismounting();
                End_Dismounting();

                _transform.position = MountTrigger.transform.position + (MountTrigger.transform.forward * -0.2f);   //Move the rider directly to the last mounting Trigger
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// IK Feet Adjustment while mounting
        /// </summary>
        void OnAnimatorIK()
        {
            if (Anim == null) return;           //If there's no animator skip
            if (Montura != null)
            {
                if (Montura.FootLeftIK == null || Montura.FootRightIK == null 
                 || Montura.KneeLeftIK == null || Montura.KneeRightIK == null) return;  //if is there missing an IK point do nothing

                //linking the weights to the animator
                if (Mounted || IsOnHorse)
                {
                    L_IKFootWeight = 1f;
                    R_IKFootWeight = 1f;

                    if (Anim.GetCurrentAnimatorStateInfo(MountLayerIndex).tagHash == Hash.Tag_Mounting 
                        || Anim.GetCurrentAnimatorStateInfo(MountLayerIndex).tagHash == Hash.Tag_Unmounting)
                    {
                        L_IKFootWeight = Anim.GetFloat(Hash.IKLeftFoot);
                        R_IKFootWeight = Anim.GetFloat(Hash.IKRightFoot);
                    }

                    //setting the weight
                    Anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, L_IKFootWeight);
                    Anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, R_IKFootWeight);

                    Anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, L_IKFootWeight);
                    Anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, R_IKFootWeight);

                    //Knees
                    Anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, L_IKFootWeight);
                    Anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, R_IKFootWeight);

                    //Set the IK Positions
                    Anim.SetIKPosition(AvatarIKGoal.LeftFoot, Montura.FootLeftIK.position);
                    Anim.SetIKPosition(AvatarIKGoal.RightFoot, Montura.FootRightIK.position);

                    //Knees
                    Anim.SetIKHintPosition(AvatarIKHint.LeftKnee, Montura.KneeLeftIK.position);    //Position
                    Anim.SetIKHintPosition(AvatarIKHint.RightKnee, Montura.KneeRightIK.position);  //Position

                    Anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, L_IKFootWeight);   //Weight
                    Anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, R_IKFootWeight);  //Weight

                    //setting the IK Rotations of the Feet
                    Anim.SetIKRotation(AvatarIKGoal.LeftFoot, Montura.FootLeftIK.rotation);
                    Anim.SetIKRotation(AvatarIKGoal.RightFoot, Montura.FootRightIK.rotation);
                }
                else
                {
                    Anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
                    Anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);

                    Anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
                    Anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
                }
            }
        }

        ///Inspector Entries
        [SerializeField] public bool Editor_Advanced;

        
        public bool debug;

        void OnDrawGizmos()
        {
            if (!debug) return;
            if (!Anim) return;
            if (syncronize)
            {
                Transform head = Anim.GetBoneTransform(HumanBodyBones.Head);

                if ((int)RiderNormalizedTime % 2 == 0)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                    Gizmos.DrawSphere((head.position - transform.root.right * 0.2f), 0.05f);

                if ((int)HorseNormalizedTime % 2 == 0)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                    Gizmos.DrawSphere((head.position + transform.root.right * 0.2f), 0.05f);

#if UNITY_EDITOR
                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.Label(head.position + transform.up * 0.5f, "Sync Status");
#endif

            }



            if (Anim)
            {
                //Gizmos.color = Color.red;
                //Gizmos.DrawSphere(RiderAnimator.pivotPosition,0.05f);
                //Gizmos.color = Color.blue;
                //Gizmos.DrawSphere(RiderAnimator.rootPosition, 0.05f);

                //if (RiderAnimator)
                //{
                //    Vector3 LeftFoot = RiderAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                //    Vector3 RightFoot = RiderAnimator.GetBoneTransform(HumanBodyBones.RightFoot).position;

                //    //Vector3 laspos = new Vector3((LeftFoot.x + RightFoot.x) / 2, (LeftFoot.y + RightFoot.y) / 2, (LeftFoot.z + RightFoot.z) / 2);

                //    Vector3 laspos = (LeftFoot + RightFoot) / 2;
                //    Gizmos.color = Color.green;
                //    Gizmos.DrawSphere(laspos, 0.05f);
                //}
            }
        }
    }
}