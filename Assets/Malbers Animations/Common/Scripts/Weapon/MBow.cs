using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using MalbersAnimations.Events;

namespace MalbersAnimations.Weapons
{
    public class MBow : MWeapon, IBow
    {
        public Transform knot;                                  //Point of the bow to put the arrow (End)
        public Transform arrowPoint;                            //Point of the bow to put the arrow (Front)

        public Transform[] UpperBn;                             //Upper Chain of the bow
        public Transform[] LowerBn;                         //Upper Chain of the bow
        public AxisDirection RotationLowerAxis;                 //Axis to rotate the bow dynamically

        public GameObject arrow;                                //Arrow to Release Prefab

        protected GameObject currentArrow;                      //Current arrow in Load Instance

        public float MaxTension;

        public float holdTime = 2f;
        [Range(0, 1)]
        public float BowTension;

        Quaternion[] UpperBnInitRotation;
        Quaternion[] LowerBnInitRotation;
        Vector3 InitPosKnot;

        public Vector3 RotUpperDir = -Vector3.forward;
        public Vector3 RotLowerDir = Vector3.forward;

        [Tooltip("Called when an arrow is instantiated")]
        public GameObjectEvent OnLoadArrow;
        public FloatEvent OnHold;
        public GameObjectEvent OnReleaseArrow;

       // [HideInInspector]
       [SerializeField] public bool BowisSet = false;

        #region Properties

        /// <summary>
        /// Every time this property  is called the Knot orients towards the ArrowPoint
        /// </summary>
        public Transform KNot
        {
            get
            {
                knot.rotation = Quaternion.LookRotation((arrowPoint.position - knot.position).normalized, -Physics.gravity);
                return knot;
            }
        }
        public Transform ArrowPoint
        {
            get { return arrowPoint; }
        }

        public float HoldTime
        {
            get { return holdTime; }
        }

        public GameObject Arrow
        {
            get { return arrow; }
            set { arrow = value; }
        }

        public GameObject ArrowInstance
        {
            get { return currentArrow; }
            set { currentArrow = value; }
        }
        #endregion

        void Start()
        {
            InitializeWeapon();
            InitializeBow();
        }

        /// <summary>
        /// Set all the main Parameters to start using a bow;
        /// </summary>
        public virtual void InitializeBow()
        {
            if (UpperBn == null || LowerBn == null)
            {
                BowisSet = false;
                return;
            }

            if (UpperBn.Length == 0 || LowerBn.Length == 0)
            {
                BowisSet = false;
                return;
            }

            BowTension = 0;

            UpperBnInitRotation = new Quaternion[UpperBn.Length];   //Get the Initial Upper ChainRotation
            LowerBnInitRotation = new Quaternion[LowerBn.Length];    //Get the Initial Lower ChainRotation

            if (knot)
                InitPosKnot = knot.localPosition;

            for (int i = 0; i < UpperBn.Length; i++)
            {
                if (UpperBn[i] == null)
                {
                    BowisSet = false;
                    return;
                }
                UpperBnInitRotation[i] = UpperBn[i].localRotation;
            }
            for (int i = 0; i < LowerBn.Length; i++)
            {
                if (LowerBn[i] == null)
                {
                    BowisSet = false;
                    return;
                }
                LowerBnInitRotation[i] = LowerBn[i].localRotation;
            }

            BowisSet = true;
        }


        /// <summary>
        /// Create an arrow ready to shooot
        /// </summary>
        public virtual void EquipArrow()
        {
            if (currentArrow != null) return;                           //If there is no arrow in the game object slot ignore

            currentArrow = (GameObject)Instantiate(Arrow, KNot);        //Instantiate the Arrow in the Knot of the Bow
            currentArrow.transform.localPosition = Vector3.zero;        //Reset Position
            currentArrow.transform.localRotation = Quaternion.identity; //Reset Rotation

            IArrow  arrow = currentArrow.GetComponent<IArrow>();        //Get the IArrow Component

            if (arrow != null)
                currentArrow.transform.Translate(0, 0, arrow.TailOffset, Space.Self);  //Translate in the offset of the arrow to put it on the hand

            OnLoadArrow.Invoke(currentArrow);
        }

        /// <summary>
        /// Destroy the Active Arrow , used when is Stored the Weapon again and it has an arrow ready
        /// </summary>
        public virtual void DestroyArrow()
        {
            if (ArrowInstance != null)
                Destroy(ArrowInstance.gameObject);

            ArrowInstance = null; //Clean the Arrow Instance
        }

        /// <summary>
        /// Rotate and modify the bow Bones to bend it from Min = 0 to Max = 1
        /// </summary>
        /// <param name="normalizedTime">0 = Relax, 1 = Stretch</param>
        public virtual void BendBow(float normalizedTime)
        {
            if (!BowisSet) return;

            BowTension = Mathf.Clamp01(normalizedTime); // Normalize time from 0 to 1
            OnHold.Invoke(BowTension);                           //Invoke on Hold Event;



            for (int i = 0; i < UpperBn.Length; i++)
            {
                if (UpperBn[i] != null)
                {
                    UpperBn[i].localRotation =
                        Quaternion.Lerp(UpperBnInitRotation[i], Quaternion.Euler(RotUpperDir * MaxTension) * UpperBnInitRotation[i], BowTension);  //Bend the Upper Chain on the Bow
                }
            }

            for (int i = 0; i < LowerBn.Length; i++)
            {
                if (LowerBn[i] != null)
                {
                    LowerBn[i].localRotation =
                        Quaternion.Lerp(LowerBnInitRotation[i], Quaternion.Euler(RotLowerDir * MaxTension) * LowerBnInitRotation[i], BowTension);  //Bend the Lower Chain of the Bow
                }
            }

            if (knot && arrowPoint)
            {
                Debug.DrawRay(KNot.position, KNot.forward, Color.red);
            }
        }
        


        public virtual void ReleaseArrow(Vector3 direction)
        {
            if (ArrowInstance == null) return;

            ArrowInstance.transform.parent = null;

            IArrow arrow = ArrowInstance.GetComponent<IArrow>();

            arrow.HitMask = HitMask; //Transfer the same Hit Mask

            arrow.ShootArrow(Mathf.Lerp(MinForce,MaxForce, BowTension), direction);
            arrow.Damage = Mathf.Lerp(MinDamage, MaxDamage, BowTension);

            OnReleaseArrow.Invoke(ArrowInstance);

            ArrowInstance = null; 
        }

        /// <summary>
        /// Is called  when the Rider is not holding the string of the bow
        /// </summary>
        public virtual void RestoreKnot()
        {
            KNot.localPosition = InitPosKnot;
            DestroyArrow();
        }


        public override void PlaySound(int ID)
        {
            if (ID > Sounds.Length - 1) return;

            if (Sounds[ID] != null && WeaponSound != null)
            {
                if (WeaponSound.isPlaying ) WeaponSound.Stop();
               
                if (ID == 2)                                    //THIS IS THE SOUND FOR BEND THE BOW
                {
                    WeaponSound.pitch = 1.03f / HoldTime;
                    StartCoroutine(BowHoldTimePlay(ID));
                }
                else
                {
                    WeaponSound.pitch = 1;
                    WeaponSound.PlayOneShot(Sounds[ID]);   //Play Draw/ Weapon Clip
                }
            }
        }

        IEnumerator BowHoldTimePlay(int ID)
        {
            while (BowTension == 0) yield return null;
            
            WeaponSound.PlayOneShot(Sounds[ID]);
        }

        //Editor variables
        [HideInInspector]
        public bool BonesFoldout,proceduralfoldout;
        [HideInInspector]
        public int LowerIndex, UpperIndex;
    }
}