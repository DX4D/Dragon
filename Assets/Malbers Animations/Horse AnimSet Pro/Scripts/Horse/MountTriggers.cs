using UnityEngine;
using System.Collections;


namespace MalbersAnimations.HAP
{
    /// <summary>
    /// This Enable the mounting System
    /// </summary>
    public class MountTriggers : MonoBehaviour
    {
        public string MountAnimation = "Mount";
        [Tooltip("the Transition ID value to dismount this kind of Montura.. (is Located on the Animator)")]
        public int DismountID = 1;
        public TransformAnimation Adjustment;
        Mountable Montura;
        Rider rider;

        // Use this for initialization
        void Awake()
        {
            Montura = GetComponentInParent<Mountable>(); //Get the Mountable in the parents
        }

        void OnTriggerEnter(Collider other)
        {
            GetAnimal(other);
        }

        void OnTriggerExit(Collider other)
        {
            rider = other.GetComponentInChildren<Rider>();

            if (rider != null)
            {
                if (rider.IsRiding) return;                             //Hack when the Rider is mounting another horse and pass really close to a new horse

                if (rider.MountAnimation == MountAnimation && !Montura.Mounted)
                {
                    rider.CanMount = false;
                    rider.Montura = null;
                    rider.OnFindMount.Invoke(null);
                    rider.MountAnimation = string.Empty;
                }
                rider = null;
            }
        }

        private void GetAnimal(Collider other)
        {
            if (!Montura.Mounted && Montura.CanBeMounted)                       //If there's no other Rider on the Animal or the the Animal isn't death
            {
                rider = other.GetComponentInChildren<Rider>();
                if (rider != null)
                {
                    if (rider.IsRiding) return;

                    rider.CanMount = true;
                    rider.Montura = Montura;
                    rider.MountTrigger = this;                           //Send the side transform to mount
                    rider.MountAnimation = MountAnimation;                     //Send the side type to mount
                    rider.OnFindMount.Invoke(transform.root.gameObject);       //Invoke Found Animal
                }
            }
        }
    }
}