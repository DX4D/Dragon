using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace MalbersAnimations.HAP
{
    public class RiderFPC : Rider
    {
        public UnityEvent OnMount = new UnityEvent();
        public UnityEvent OnDismount = new UnityEvent();

        void Awake()
        {
            _transform = transform;
            _collider = GetComponents<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            if (StartMounted)  AlreadyMounted();
        }

        public void AlreadyMounted()
        {
            if (AnimalStored != null) StartCoroutine(AlreadyMountedC());
        }

        IEnumerator AlreadyMountedC()
        {
            yield return null;      //Wait for the next frame
            Mounting();
        }   

        public void Mounting()
        {
            base.Start_Mounting();

            IsOnHorse = true;


            Montura.EnableControls(true);                                                           //Enable Animal Controls

            if (CreateColliderMounted) MountingCollider(true);

            Montura.ActiveRider = this;

            Vector3 AnimalForward = Montura.transform.forward;
            AnimalForward.y = 0;
            AnimalForward.Normalize();

            transform.rotation = Quaternion.LookRotation(AnimalForward, -Physics.gravity);

            OnMount.Invoke();
        }

        public void Dismounting()
        {
            base.Start_Dismounting();
            base.End_Dismounting();

            transform.position = new Vector3(MountTrigger.transform.position.x, transform.position.y, MountTrigger.transform.position.z); //Desplazar
            if (_rigidbody)  _rigidbody.velocity = Vector3.zero;

            OnDismount.Invoke();
        }

        void Update()
        {
            if (IsRiding)
                transform.position = Montura.MountPoint.position;           //Lock the FPC position in the mount Point

            if (transform.parent != null && isOnHorse)  mounted = true;     //Double check that if is still have a parent and is still on the horse you're still mounted
  
            if (MountInput.GetInput)                                        //If the mount Key is pressed
            {
                SetMounting();
            }
        }

        public virtual void SetMounting()
        {
            if (CanMount)                                               //if are near an animal and we are not already on an animal
            {
                Mounting();                                             //Run mounting Animations
            }
            else if (CanDismount)
            {
                Dismounting();
            }
            else if (!can_Mount && !IsRiding)                           //if there is no horse near, call the animal in the slot
            {
                CallAnimal();
            }
        }
    }
}