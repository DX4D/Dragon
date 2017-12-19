using UnityEngine;
using System.Collections;
using MalbersAnimations.Events;

namespace MalbersAnimations.HAP
{
    public class PullingHorses : MonoBehaviour
    {
        [Header("Horses")]
        public Animal RightHorse;
        public Animal LeftHorse;

        [Header("Turn Speed")]
        public float TurnSpeed0 = 10f;
        public float TurnSpeed1 = 25f;
        public float TurnSpeed2 = 25f;
        public float TurnSpeed3 = 35f;

        [HideInInspector]
        public float CurrentTurnSpeed = 25f;
        protected Rigidbody _rigidbody;

        [HideInInspector]
        public Vector3 PullingDirection;          //Calculation for the Animator Velocity converted to RigidBody Velocityble
        [HideInInspector]
        public bool CurrentAngleSide;             //True if is in the Right Side ... False if is in the Left Side
        [HideInInspector]
        public bool CanRotateInPlace;

        public Transform RotationPivot;

        Vector3 RHorseInitialPos;
        Vector3 LHorseInitialPos;


        // Use this for initialization
        void Start()
        {
            if (!RightHorse) return;
            if (!LeftHorse) LeftHorse = RightHorse;

            RHorseInitialPos = RightHorse.transform.localPosition;

            _rigidbody = GetComponent<Rigidbody>();

            RightHorse.transform.parent = transform;
            LeftHorse.transform.parent = transform;

            LeftHorse.GetComponent<Rigidbody>().isKinematic = true;
            RightHorse.GetComponent<Rigidbody>().isKinematic = true;

            LeftHorse.Anim.applyRootMotion = false;
            RightHorse.Anim.applyRootMotion = false;


            switch (RightHorse.StartSpeed)
            {
                case Animal.Ground.walk:
                    CurrentTurnSpeed = TurnSpeed1;
                    break;
                case Animal.Ground.trot:
                    CurrentTurnSpeed = TurnSpeed2;
                    break;
                case Animal.Ground.run:
                    CurrentTurnSpeed = TurnSpeed3;
                    break;
                default:
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!RightHorse) return;


            if (RightHorse.Speed1)
                CurrentTurnSpeed = TurnSpeed1;

            else if (RightHorse.Speed2)
                CurrentTurnSpeed = TurnSpeed2;

            else if (RightHorse.Speed3)
                CurrentTurnSpeed = TurnSpeed3;


            if (LeftHorse.Anim.applyRootMotion) LeftHorse.Anim.applyRootMotion = false;
            if (RightHorse.Anim.applyRootMotion) RightHorse.Anim.applyRootMotion = false;

          



            if (RightHorse.Speed == 0)
            {
                RightHorse.MovementAxis = new Vector3(RightHorse.MovementAxis.x * 2, RightHorse.MovementAxis.y, RightHorse.MovementAxis.z);         //Put both horses to SideWalk

                

                if (CanRotateInPlace)
                {
                    transform.RotateAround(RotationPivot.position, Vector3.up, RightHorse.MovementAxis.x * Time.deltaTime * TurnSpeed0);      //Rotation InPlace
                }
                else
                {
                    if ((CurrentAngleSide && RightHorse.MovementAxis.x < 0) || (!CurrentAngleSide && RightHorse.MovementAxis.x > 0))       //Stop the horse Animation when it can rotate anylonger
                    {
                        RightHorse.MovementAxis = LeftHorse.MovementAxis = Vector3.zero;
                    }
                }


                PullingDirection = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, Time.deltaTime * 25);

                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, Time.deltaTime * 5);
            }
            else
            {
                transform.RotateAround(RotationPivot.position, Vector3.up, RightHorse.MovementAxis.x * Time.deltaTime * CurrentTurnSpeed);          //Rotate around Speed

                PullingDirection = Vector3.Lerp(PullingDirection, transform.forward * RightHorse.Anim.velocity.magnitude * (RightHorse.Speed >= 0 ? 1 : -1), Time.deltaTime * 15);                   //Calculate the current speed of the animator root motion

               // PullingDirection = RightHorse.Anim.velocity;

                _rigidbody.velocity = PullingDirection;
            }


            //Use the centered position for the horses
            transform.position = new Vector3(transform.position.x, (RightHorse.transform.position.y + LeftHorse.transform.position.y) / 2, transform.position.z);

            if (RightHorse)
            {
                RightHorse.transform.rotation = transform.rotation; //Make sure the horse keeps  the transform rotation
                RightHorse.transform.rotation = Quaternion.FromToRotation(RightHorse.transform.up, RightHorse.SurfaceNormal) * RightHorse._RigidBody.rotation;  //Calculate the orientation to Terrain  
            }
            if (LeftHorse) //Make sure the horse keeps  the transform rotation
            {
                LeftHorse.transform.rotation = transform.rotation;
                LeftHorse.transform.rotation = Quaternion.FromToRotation(LeftHorse.transform.up, LeftHorse.SurfaceNormal) * LeftHorse._RigidBody.rotation;  //Calculate the orientation to Terrain  
            }

            if (LeftHorse && LeftHorse != RightHorse)                       //if is there a different left horse
            {
                LeftHorse.MovementAxis = RightHorse.MovementAxis;
                LeftHorse.GroundSpeed = RightHorse.GroundSpeed;
                LeftHorse.SetIntID(RightHorse.IDInt);
            }

            RightHorse.transform.localPosition = new Vector3(RHorseInitialPos.x, RightHorse.transform.localPosition.y, RHorseInitialPos.z);
        }
    }
}