using UnityEngine;
using System.Collections;

namespace MalbersAnimations.HAP
{
    /// <summary>
    /// Used for Linking the Reins to the hand of the Rider
    /// </summary>
    public class IKReins : MonoBehaviour
    {
        Mountable Montura;
        public Transform ReinLeftHand, ReinRightHand;
        protected Vector3 LocalStride_L, LocalStride_R;
        Transform riderHand_L, riderHand_R;

        bool freeRightHand = true;
        bool freeLeftHand = true;

        void Start()
        {
            Montura = GetComponent<Mountable>();

            if (ReinLeftHand && ReinRightHand)
            {
                LocalStride_L = ReinLeftHand.localPosition;
                LocalStride_R = ReinRightHand.localPosition;
            }
            else
            {
                Debug.LogWarning("Some of the Reins has not been set on the inspector. Please fill the values");
            }
        }

        /// <summary>
        /// Checking if the Right hand is free
        /// </summary>
        public void RightHand_is_Free(bool value)
        {
            freeRightHand = value;
            if (!value && ReinRightHand)
            {
                ReinRightHand.localPosition = LocalStride_R;
            }
        }

        /// <summary>
        /// Checking if the Left hand is free
        /// </summary>
        public void LeftHand_is_Free(bool value)
        {
            freeLeftHand = value;
            if (!value && ReinLeftHand)
            {
                ReinLeftHand.localPosition = LocalStride_L;
            }
        }

        void Update()
        {
            if (Montura && Montura.ActiveRider && ReinLeftHand && ReinRightHand)
            {
                Animator Anim = Montura.ActiveRider.Anim;  //Get the Rider Animator
                if (!Anim) return;

                riderHand_L = Anim.GetBoneTransform(HumanBodyBones.LeftHand);
                riderHand_R = Anim.GetBoneTransform(HumanBodyBones.RightHand);

                if (!Montura.CanDismount)
                {
                    if (freeLeftHand)
                    {
                        ReinLeftHand.position = Vector3.Lerp(riderHand_L.position, riderHand_L.GetChild(1).position, 0.5f);     //Put it in the middle o the left hand
                    }
                    else
                    {
                        if (freeRightHand)
                            ReinLeftHand.position = Vector3.Lerp(riderHand_R.position, riderHand_R.GetChild(1).position, 0.5f); //if the right hand is holding a weapon put the right rein to the Right hand
                    }

                    if (freeRightHand)
                    {
                        ReinRightHand.position = Vector3.Lerp(riderHand_R.position, riderHand_R.GetChild(1).position, 0.5f); //Put it in the middle o the RIGHT hand
                    }
                    else
                    {
                        if (freeLeftHand)
                            ReinRightHand.position = Vector3.Lerp(riderHand_L.position, riderHand_L.GetChild(1).position, 0.5f); //if the right hand is holding a weapon put the right rein to the Left hand
                    }
                }
                else
                {
                    ReinLeftHand.localPosition = LocalStride_L;
                    ReinRightHand.localPosition = LocalStride_R;
                }
            }
        }
    }
}
