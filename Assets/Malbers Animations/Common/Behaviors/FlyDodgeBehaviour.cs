using UnityEngine;

namespace MalbersAnimations
{
    public class FlyDodgeBehaviour : StateMachineBehaviour
    {
        Vector3 momentum;       //To Store the velocity that the animator had before entering this animation state

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            momentum = animator.velocity;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.transform.position = Vector3.Lerp(animator.transform.position, animator.transform.position + momentum, Time.deltaTime);
        }
    }
}