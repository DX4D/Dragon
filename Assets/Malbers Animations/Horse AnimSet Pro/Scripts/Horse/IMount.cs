using UnityEngine;
using System.Collections;

namespace MalbersAnimations.HAP
{
    public interface IMount
    {
        Transform MountPoint { get; }       // Reference for the RidersLink Bone
        Transform FootLeftIK { get; }       // Reference for the LeftFoot correct position on the mount
        Transform FootRightIK { get; }      // Reference for the RightFoot correct position on the mount
        Transform KneeLeftIK { get; }       // Reference for the LeftKnee correct position on the mount
        Transform KneeRightIK { get; }      // Reference for the RightKnee correct position on the mount
        Animal Animal { get; }              //The Animal Asociated to this Mount

        /// <summary> 
        /// If is an Rider mounting this mount
        /// </summary>
        Rider ActiveRider { get; set; }     //The Rider
        Transform transform { get; }

        bool StraightSpine { get; }
        Quaternion SpineOffset { get; }
        bool Mounted { get; set; }          //If the Animal is Already Mounted
        bool CanBeMounted { get; set; }     //If the Mount can be mounted deactivate if the mount is death or destroyed
        bool CanDismount { get; }           //When the Mount is ready to be dismounted
        //MountType Mount_Type { get; }

        string MountLayer { get; set; }
        string MountIdle { get; set; }
        void EnableControls(bool value);
    }

    public interface IMountAI
    {
       bool CanBeCalled { get; }
       void CallAnimal(Transform target, bool call = true);
    }
}