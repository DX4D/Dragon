using UnityEngine;
using System.Collections;


namespace MalbersAnimations
{
    public class ChangeCameraStates : MonoBehaviour
    {
        public MFreeLookCamera mCamera;                 //References for the camera

        public float transition = 1f;                   //Transitions time from one state to another

        protected FreeLookCameraState currentState;

        public FreeLookCameraState Default;
        public FreeLookCameraState AimRight;
        public FreeLookCameraState AimLeft;
        public FreeLookCameraState Mounted;


        private FreeLookCameraState NextState;

        bool inTransition;

        void Start()
        {
            currentState = Default;
            UpdateState(currentState);
        }

        // Update is called once per frame
        void Update()
        {
            if (!inTransition)
            {
                UpdateState(currentState);
            }
        }

        public void SetMounted(FreeLookCameraState state)
        {
            Mounted = state;
            SetCameraState(Mounted);
        }

        void UpdateState(FreeLookCameraState state)
        {
            if (mCamera == null) return;
            if (state == null) return;

            mCamera.Pivot.localPosition = state.PivotPos;
            mCamera.Cam.localPosition = state.CamPos;
            mCamera.Cam.GetComponent<Camera>().fieldOfView = state.CamFOV;
        }

        public void SetAim(int ID)
        {
            if (ID == -1 && AimLeft)
            {
                SetCameraState(AimLeft);
            }
            else if (ID == 1 && AimRight)
            {
                SetCameraState(AimRight);
            }
            else
            {
                SetCameraState(Mounted ? Mounted : Default);
            }
        }

        public void SetCameraState(FreeLookCameraState state)
        {
            if (mCamera == null) return;
            if (state == null) return;

            NextState = state;

            
            if (currentState && NextState.Name == currentState.Name) return;

            StopAllCoroutines();

            StartCoroutine(StateTransition(transition));
        }

        IEnumerator StateTransition(float time)
        {
            inTransition = true;
            float elapsedTime = 0;
            currentState = NextState;
            while (elapsedTime < time)
            {
                mCamera.Pivot.localPosition = Vector3.Lerp(mCamera.Pivot.localPosition, NextState.PivotPos, Mathf.SmoothStep(0, 1, elapsedTime / time));
                mCamera.Cam.localPosition = Vector3.Lerp(mCamera.Cam.localPosition, NextState.CamPos, Mathf.SmoothStep(0, 1, elapsedTime / time));
                mCamera.Cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mCamera.Cam.GetComponent<Camera>().fieldOfView, NextState.CamFOV, Mathf.SmoothStep(0, 1, elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            UpdateState(NextState);
            inTransition = false;
        }
    }
}