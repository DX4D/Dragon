using UnityEngine;
using System.Collections;
using MalbersAnimations.HAP;
using UnityEngine.Events;
using MalbersAnimations.Events;

namespace MalbersAnimations.HAP
{
    /// <summary>
    /// Logic for making any Animal Mountable
    /// </summary>
    public class Mountable : MonoBehaviour , IAnimatorListener
    {

        #region Components
        protected Rider _rider;                 //Rider's Animator to control both Sync animators from here
        protected Animal _animal;               //The animal Asociated to the mount
        #endregion

        #region General
        public bool active = true;
        public string mountLayer = "Mounted";
        public bool instantMount = false;
        public string mountIdle = "Idle01";


        protected bool mounted;                                 //If the Animal have been mounted  
        private bool isOnCoroutine;
        #endregion

        #region Straight Mount
        public bool straightSpine;                              //Activate this only for other animals but the horse 
        public Vector3 pointOffset = new Vector3(0,0,0);
        public float LowLimit = 45;
        public float HighLimit = 135;

        public float smoothSM = 0.5f;

        protected Quaternion InitialRotation;
        private Quaternion straightRotation;

        private Quaternion currentRotation;

        #endregion

        #region Animator Speeds

        /// <summary>
        /// if both rider and animal animator should be synced on the Locomotion state..
        /// </summary>
        public bool syncAnimators = true;
        //public bool syncAttacks = true;

        public float WalkASpeed = 1;
        public float TrotASpeed = 1;
        public float RunASpeed = 1;
        public float FlyASpeed = 1;
        public float SwimASpeed = 1;

        protected float AnimatorSpeed = 1f;
        #endregion

        #region Mount Points
        public Transform ridersLink;            // Reference for the RidersLink Bone
        public Transform leftIK;                // Reference for the LeftFoot correct position on the mount
        public Transform rightIK;               // Reference for the RightFoot correct position on the mount
        public Transform leftKnee;              // Reference for the LeftKnee correct position on the mount
        public Transform rightKnee;             // Reference for the RightKnee correct position on the mount
        #endregion

        #region Reins
        protected Vector3 LocalStride_L, LocalStride_R;
        protected bool freeRightHand = true;
        protected bool freeLeftHand = true;
        #endregion

        #region Properties
        public Transform MountPoint   { get { return ridersLink; } }    // Reference for the RidersLink Bone  
        public Transform FootLeftIK   { get { return leftIK;     } }    // Reference for the LeftFoot correct position on the mount
        public Transform FootRightIK  { get { return rightIK;    } }    // Reference for the RightFoot correct position on the mount
        public Transform KneeLeftIK   { get { return leftKnee;   } }    // Reference for the LeftKnee correct position on the mount
        public Transform KneeRightIK  { get { return rightKnee;  } }    // Reference for the RightKnee correct position on the mount

        public bool StraightSpine { get { return straightSpine; } }     // Reference for the RightKnee correct position on the mount

        /// <summary>
        /// point Offset vector3 converted to Quaternion
        /// </summary>
        public Quaternion PointOffset
        {
            get
            {
                return Quaternion.Euler(pointOffset);
            }
        }     

        /// <summary>
        /// Is the animal Mounted
        /// </summary>
        public bool Mounted
        {
            set
            {
                if (value != mounted)
                {
                    mounted =  value;
                    if (mounted)
                    {
                        OnMounted.Invoke();    //Invoke the Event
                    }
                    else OnDismounted.Invoke();
                }
            }

            get { return mounted; }
        }

        public virtual Animal Animal
        {
            get
            {
                if (_animal == null)
                    _animal = GetComponent<Animal>();
                return _animal;
            }
        }

        /// <summary>
        /// Dismount only when the Animal is Still on place
        /// </summary>
        public virtual bool CanDismount
        {
            get { return Animal.Stand; }
        }

        public virtual string MountLayer
        {
            get { return mountLayer; }
            set { mountLayer = value; }
        }

        public virtual string MountIdle
        {
            get { return mountIdle; }
            set { mountIdle = value; }
        }

        public virtual bool CanBeMounted
        {
            get { return active; }
            set { active = value; }
        }

        public Rider ActiveRider
        {
            get { return _rider; }
            set { _rider = value; }
        }

        bool changed;

        /// <summary>
        /// Ignore Mounting Animations
        /// </summary>
        public bool InstantMount
        {
            get { return instantMount; }
            set { instantMount = value; }
        }

        public Quaternion StraightRotation
        {
            get { return straightRotation; }
        }

        #endregion

        public UnityEvent OnMounted = new UnityEvent();
        public UnityEvent OnDismounted = new UnityEvent();

        public virtual void EnableControls(bool value)
        {
            if (Animal)
            {
                MalbersInput animalControls = Animal.GetComponent<MalbersInput>();

                if (animalControls)
                {
                    animalControls.enabled = value;
                }
                Animal.MovementAxis = Vector3.zero;
            }
        }

        private void Start()
        {
            InitialRotation = MountPoint.localRotation;
        }

        private void Update()
        {
            SetAnimatorSpeed(Time.deltaTime * 2);                       //Change the animator Speed acording to the animal its riding ... to make it more in Sync

            SolveStraightMount();
        }

        private void SolveStraightMount()
        {
            if (!ActiveRider) return;                                               //if there's No Rider Skip
            if (!ActiveRider.IsRiding) return;                                      //if is not Riding Skip

            currentRotation = MountPoint.localRotation;                             //Store the Current Local Rotation

            MountPoint.localRotation = InitialRotation;                             //Reset the Rotation on the Mount Point

            straightRotation = Quaternion.FromToRotation(MountPoint.up, Vector3.up) * MountPoint.rotation;       //Calculate the orientation to the Up Vector  

            float angle = Vector3.Angle(Vector3.up, MountPoint.forward);            //Check limits

            if (angle < LowLimit)
            {
                straightRotation *= Quaternion.Euler(new Vector3(angle - LowLimit, 0));
            }
            else if (angle > HighLimit)
            {
                straightRotation *= Quaternion.Euler(new Vector3(angle - HighLimit, 0));
            }

            if (pointOffset != Vector3.zero) straightRotation *= PointOffset;                                                    //Add the Spine Offset
            

            MountPoint.localRotation = currentRotation;                                         //Restore the Current Local Rotation

            if (straightAim)
            {
                MountPoint.rotation = straightRotation;
                return;
            }

            if (straightSpine && !changed)
            {
                changed = true;
                StopAllCoroutines();
                StartCoroutine(I_to_StraightMount(1));
            }

            if (!straightSpine && changed)
            {
                changed = false;
                StopAllCoroutines();
                StartCoroutine(I_from_StraightMount(1));
            }

            if (!isOnCoroutine)
            {
                if (straightSpine)
                {
                    MountPoint.rotation = straightRotation;
                }
                else
                {
                    MountPoint.localRotation = InitialRotation;
                }
            }
        }

        IEnumerator I_to_StraightMount(float time)
        {
            float currentTime = 0;
            Quaternion startRotation = MountPoint.rotation;
            isOnCoroutine = true;

            while (currentTime <= time)
            {
                currentTime += Time.deltaTime;

                MountPoint.rotation = Quaternion.Slerp(startRotation, straightRotation, currentTime / time);

                yield return null;
            }
            MountPoint.rotation = straightRotation;

            isOnCoroutine = false;
        }

        IEnumerator I_from_StraightMount(float time)
        {
            float currentTime = 0;
            Quaternion startRotation = MountPoint.localRotation;
            isOnCoroutine = true;

            while (currentTime <= time)
            {
                currentTime += Time.deltaTime;

                MountPoint.localRotation = Quaternion.Slerp(startRotation, InitialRotation, currentTime / time);

                yield return null;
            }
            MountPoint.localRotation = InitialRotation;
            isOnCoroutine = false;
        }

        private void SetAnimatorSpeed(float time)
        {
            if (ActiveRider && ActiveRider.Anim)
            {
                AnimatorSpeed = 1;

                if (Animal.CurrentAnimState.tagHash == Hash.Locomotion)
                {
                    if (Animal.Speed1)
                    {
                        AnimatorSpeed = WalkASpeed * Animal.walkSpeed.animator;
                    }
                    else if (Animal.Speed2)
                    {
                        AnimatorSpeed = TrotASpeed * Animal.trotSpeed.animator;
                    }
                    else if (Animal.Speed3)
                    {
                        AnimatorSpeed = RunASpeed * Animal.runSpeed.animator;
                    }
                }

                if (Animal.canSwim && Animal.Swim)
                {
                    AnimatorSpeed = SwimASpeed * Animal.swimSpeed.animator;
                }

                if (Animal.canFly && Animal.Fly)
                {
                    AnimatorSpeed = FlyASpeed * Animal.flySpeed.animator;
                }

                ActiveRider.Anim.speed = Mathf.Lerp(ActiveRider.Anim.speed, AnimatorSpeed, time);
            }
        }

        /// <summary>
        /// Enable/Disable the StraightMount Feature
        /// </summary>
        /// <param name="value"></param>
        public virtual void StraightMount(bool value)
        {
            straightSpine = value;
        }

        bool straightAim;

        public virtual void StraightAim(bool value)
        {
            straightAim = value;
        }

        public virtual void OnAnimatorBehaviourMessage(string message, object value)
        {
            this.InvokeWithParams(message, value);
        }

        //UnityEditor
        [HideInInspector] public bool ShowLinks;
        [HideInInspector] public bool ShowAnimatorSpeeds;
    }
}