using UnityEngine;
using System;
using UnityEngine.EventSystems;
using MalbersAnimations.Weapons;
using MalbersAnimations.Events;
using UnityEngine.Events;
using System.Collections.Generic;

namespace MalbersAnimations.HAP
{
    /// <summary>
    /// Variables and Properties
    /// </summary>
    public partial class RiderCombat
    {
        #region Public Entries
        public LayerMask HitMask;

        public InputRow InputWeapon  = new InputRow(KeyCode.E);          //Input for Draw/Store Weapons
        public InputRow InputAttack1 = new InputRow("Fire1");            //Input for Attack1 
        public InputRow InputAttack2 = new InputRow("Fire2");            //Input for Attack2
        public InputRow InputAim     = new InputRow("Fire2");            //Input for AIM

        public bool UseInventory = true;                                //Use this if the weapons are on an Inventory
        public bool AlreadyInstantiated = true;                         //If the weapons comes from an inventory check if they are already intantiate
        public bool UseHolders = false;                                 //Use this if the weapons are on the Holders
        public bool StrafeOnTarget;                                     //If there's a target Strafe if this is set to true
        public WeaponHolder ActiveHolderSide = WeaponHolder.Back;       //Start with Back Holder as default  

        public Transform
                LeftHandEquipPoint,                                      //Equip Point for the Left Hand    
                RightHandEquipPoint,                                     //Equip Point for the Right Hand    
                HolderLeft,                                              //Transform where the Left weapons are going to be
                HolderRight,                                             //Transform where the Left weapons are going to be
                HolderBack,                                              //Transform where the Left weapons are going to be
                ActiveHolderTransform;                                   //Active Transform holder to draw weapons from

        public InputRow HBack = new InputRow(KeyCode.Alpha4);            //Input to get the Weapons in the Back  Holder if using Holders
        public InputRow HLeft = new InputRow(KeyCode.Alpha5);            //Input to get the Weapons in the Left  Holder if using Holders
        public InputRow HRight = new InputRow(KeyCode.Alpha6);           //Input to get the Weapons in the Right Holder if using Holders
        public InputRow Reload = new InputRow(KeyCode.R);                //Input to get the Weapons in the Right Holder if using Holders

        public List<RiderCombatAbility> CombatAbilities;                 //Combat Abilities availables for the rider

        public bool debug;                                               //Debug Log
        #endregion

       // [HideInInspector]
        public RiderCombatAbility ActiveAbility;
        protected Rider3rdPerson _rider;                                 //Reference for the Rider3rdPerson component
        protected Animator       _anim;                                  //Reference for the animator component


        protected GameObject activeWeapon;
        protected WeaponType _weaponType = WeaponType.None;              //Which Type of weapon is in the active weapon
        protected WeaponActions _weaponAction = WeaponActions.None;      //Which type of action is making the active weapon

        protected bool
                  lockCombat,                                            //LockCombat                      
                  isInCombatMode,                                        //If the rider is in combat mode
                  aimSent,                                               //for send just one time if is entering or leavinng aimmode (Automatic)   
                  isAiming,                                              //True if is on AIM mode (Trigger by Input)   
                  TargetSide,                                            //Check if the camera is in the right side of the Rider
                  CameraSide;

        protected bool CurrentCameraSide;                                //Current side of the Camera

        protected int Layer_RiderArmRight;                               //Right Arm Layer    
        protected int Layer_RiderArmLeft;                                //Left  Arm Layer
        protected int Layer_RiderCombat;                                 //Combat Layer

        protected Transform _cam;                                        //Reference for the Cam    
        public RectTransform AimDot;
        

        /// <summary>
        /// Transform the Rider is Aiming to
        /// </summary>
        public Transform Target;                                    

        internal Transform lastTarget = null;

        #region AIM Variables
        private float horizontalAngle;                                  //Value for the Aim Sides for the animator

        protected Vector3 aimDirection;                                 //Direction that the Rider is aiming
        protected RaycastHit aimRayHit;                                 //RayCastHit to Store all the information of the Aim Ray

        protected Quaternion
            MountStartRotation;                                         //Mount Point initial Rotation to return to when the rider is aiming
        #endregion

        #region Animator Hashs
        public readonly static int Hash_AimSide      = Animator.StringToHash("WeaponAim");
        public readonly static int Hash_WeaponType   = Animator.StringToHash("WeaponType");
        public readonly static int Hash_WeaponHolder = Animator.StringToHash("WeaponHolder");
        public readonly static int Hash_WeaponAction = Animator.StringToHash("WeaponAction");
        #endregion

        #region Events
        public GameObjectEvent OnEquipWeapon = new GameObjectEvent();
        public GameObjectEvent OnUnequipWeapon = new GameObjectEvent();
        public WeaponActionEvent OnWeaponAction = new WeaponActionEvent() ;
        public WeaponEvent OnAttack = new WeaponEvent() ;
        public IntEvent OnAimSide = new IntEvent();
        public TransformEvent OnTarget = new TransformEvent();
        #endregion

        protected Transform
                 _RightShoulder,                                         //Reference for the Right Shoulder
                 _LeftShoulder,                                          //Reference for the Left Shoulder
                 _RightHand,                                             //Reference for the Right Hand
                 _LeftHand,                                              //Reference for the Left Hand
                 _Head,                                                  //Reference for the Head
                 _Chest,                                                 //Reference for the Chest
                 _transform,                                             //Reference for this Transform
                 mountPoint;                                             //Reference for the Moint Point;  

        bool DefaultMonturaInputCamBaseInput;                           //For storing the Montura Input Type

        #region Properties

        /// <summary>
        /// If is true everything will be ignored
        /// </summary>
        public bool LockCombat
        {
            get { return lockCombat; }
            set { lockCombat = value; }
        }
        public Rider3rdPerson rider
        {
            get
            {
                if (_rider == null)
                {
                    _rider = GetComponent<Rider3rdPerson>();
                }
                return _rider;
            }
        }

        /// <summary>
        /// Enable or Disable the Combat Mode (True when the rider has equipped a compatible weapon)
        /// </summary>
        public bool IsInCombatMode
        {
            get { return isInCombatMode; }
            set { isInCombatMode = value; }
        }


        /// <summary>
        /// Which Action is currently using the RiderCombat. See WeaponActions Enum for more detail
        /// </summary>
        public WeaponActions WeaponAction
        {
            get { return _weaponAction; }
            set { _weaponAction = value; }
        }

        /// <summary>
        /// Returns the IMWeapon Interface of the Active Weapon GameObject
        /// </summary>
        public IMWeapon Active_IMWeapon
        {
            get
            {
                if (ActiveWeapon)
                {
                    return ActiveWeapon.GetComponent<IMWeapon>();
                }
                return null;
            }
        }

        /// <summary>
        /// The transform the Rider is parent to
        /// </summary>
        public Transform MountPoint
        {
            get
            {
                if (mountPoint ==null)
                {
                    mountPoint = rider.Montura.MountPoint;
                }
                return mountPoint;
            }
        }

        public Animator Anim
        {
            get { return _anim; }
        }

       
        public bool IsAiming
        {
            set { isAiming = value; }
            get { return isAiming; }
        }

        /// <summary>
        /// Which side is the rider regarding the camera or the target Location (Right:true, Left:False)
        /// </summary>
        public bool IsCamRightSide
        {
            get { return CameraSide; }
        }

        /// <summary>
        /// Returns the Normalized Angle Around the Y Axis (from -1 to 1) regarding the Target position
        /// </summary>
        public float HorizontalAngle
        {
            get { return horizontalAngle; }
        }

        public Transform RightShoulder
        {
            get { return _RightShoulder; }
        }

        public Transform LeftShoulder
        {
            get { return _LeftShoulder; }
        }
        public Transform RightHand
        {
            get { return _RightHand; }
        }
        public Transform LeftHand
        {
            get { return _LeftHand; }
        }
        public Transform Head
        {
            get { return _Head; }
        }
        public Transform Chest
        {
            get { return _Chest; }
        }

        /// <summary>
        /// Direction the Rider is Aiming
        /// </summary>
        public Vector3 AimDirection
        {
            get { return aimDirection; }
        }

        /// <summary>
        /// Information stored by the Aim Mode 
        /// </summary>
        public RaycastHit AimRayCastHit
        {
            get { return aimRayHit; }
        }

        public GameObject ActiveWeapon
        {
            get {return activeWeapon;}
        }
        #endregion

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        ///Editor Variables
        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        [HideInInspector] public bool Editor_ShowHolders;
        [HideInInspector] public bool Editor_ShowInputs;
        [HideInInspector] public bool Editor_ShowHoldersInput;
        [HideInInspector] public bool Editor_ShowAdvanced;
        [HideInInspector] public bool Editor_ShowEvents;
        [HideInInspector] public bool Editor_ShowEquipPoints;
    }
}