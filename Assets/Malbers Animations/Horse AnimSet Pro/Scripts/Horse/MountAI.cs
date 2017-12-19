using UnityEngine;
using System.Collections;
using MalbersAnimations.HAP;
using UnityEngine.AI;

namespace MalbersAnimations
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MountAI : AnimalAIControl, IMountAI
    {
        public bool canBeCalled;
        protected Mountable animalMount;               //The Animal Mount Script
        protected bool isBeingCalled;

        public bool CanBeCalled
        {
            get { return canBeCalled; }
            set { canBeCalled = value; }
        }

        void Start()
        {
            animalMount = GetComponent<Mountable>();
            StartAgent();
        }

        void Update()
        {
            Agent.nextPosition = transform.position;                      //Update the Agent Position to the Transform position
            if (!Agent.isOnNavMesh || !Agent.enabled) return;

            if (animalMount.Mounted)            //If the Animal is mounted
            {
                Agent.enabled = false;          //Disable the navmesh agent
                return;                     
            }
            UpdateAgent();
        }

        public void CallAnimal(Transform target, bool call)
        {

            if (!CanBeCalled) return;           //If the animal cannot be called ignore this
            if (!Agent) return;                 //If there's no agent ignore this

            isBeingCalled = call;
            this.target = target;


            if (isBeingCalled)
            {
                Agent.enabled = true;
                Agent.isStopped = false;
            }
            else
            {
                Agent.enabled = true;
                Agent.isStopped = true;
            }

         
          
           

          

         
        }
    }
}
