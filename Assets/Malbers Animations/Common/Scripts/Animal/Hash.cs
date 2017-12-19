using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// Faster way to work with Animator Controller Parameters
    /// </summary>
    public class Hash : MonoBehaviour
    {
        public static int Vertical = Animator.StringToHash("Vertical");
        public static int Horizontal = Animator.StringToHash("Horizontal");
        public static int UpDown = Animator.StringToHash("UpDown");

        public static int Stand = Animator.StringToHash("Stand");
        public static int Grounded = Animator.StringToHash("Grounded");

        public static int _Jump = Animator.StringToHash("_Jump");

        public static int Dodge = Animator.StringToHash("Dodge");
        public static int Fall = Animator.StringToHash("Fall");


        public static int Slope = Animator.StringToHash("Slope");

        public static int Shift = Animator.StringToHash("Shift");

        public static int Fly = Animator.StringToHash("Fly");

        public static int Attack1 = Animator.StringToHash("Attack1");
        public static int Attack2 = Animator.StringToHash("Attack2");

        public static int Death = Animator.StringToHash("Death");

        public readonly static int Damaged = Animator.StringToHash("Damaged");
        public readonly static int Stunned = Animator.StringToHash("Stunned");

        public readonly static int IDInt = Animator.StringToHash("IDInt");
        public readonly static int IDFloat = Animator.StringToHash("IDFloat");

        public readonly static int Swim = Animator.StringToHash("Swim");
        public readonly static int Underwater = Animator.StringToHash("Underwater");

        public readonly static int IDAction = Animator.StringToHash("IDAction");
        public readonly static int Action = Animator.StringToHash("Action");


        public readonly static int Null = Animator.StringToHash("Null");
        public readonly static int Empty = Animator.StringToHash("Empty");



        public readonly static int Locomotion = Animator.StringToHash("Locomotion");
        public readonly static int Tag_Idle = Animator.StringToHash("Idle");
        public readonly static int Tag_Recover = Animator.StringToHash("Recover");
        public readonly static int Tag_Sleep = Animator.StringToHash("Sleep");
        public readonly static int Tag_Attack = Animator.StringToHash("Attack");
        public readonly static int Tag_JumpEnd = Animator.StringToHash("JumpEnd");
        public readonly static int Tag_Jump = Animator.StringToHash("Jump");
        public readonly static int Tag_SwimJump = Animator.StringToHash("SwimJump");

        //---------------------------HAP-----------------------------------------

        
        public readonly static int IKLeftFoot = Animator.StringToHash("IKLeftFoot");
        public readonly static int IKRightFoot = Animator.StringToHash("IKRightFoot");

        public readonly static int Mount = Animator.StringToHash("Mount");
        public readonly static int MountSide = Animator.StringToHash("MountSide");

        public readonly static int Tag_Mounting= Animator.StringToHash("Mounting");
        public readonly static int Tag_Unmounting = Animator.StringToHash("Unmounting");

    }
}