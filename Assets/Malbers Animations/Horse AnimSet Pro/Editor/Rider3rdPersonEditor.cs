using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

namespace MalbersAnimations.HAP
{
    [CustomEditor(typeof(Rider3rdPerson))]
    public class Rider3rdPersonEditor : RiderEditor
    {
        private MonoScript script;
        protected virtual void OnEnable()
        {
            SetEnable();
            MyRider = (Rider3rdPerson)target;
            script = MonoScript.FromMonoBehaviour(MyRider);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            SetOnInspectorGUI();

            Animator anim = MyRider.GetComponent<Animator>();

            AnimatorController controller = (AnimatorController) anim.runtimeAnimatorController;

            if (controller)
            {
                List<AnimatorControllerLayer> layers = controller.layers.ToList();

                if (layers.Find(layer => layer.name == "Mounted") == null)
                //if (anim.GetLayerIndex("Mounted") == -1)
                {
                    if (GUILayout.Button(new GUIContent("Add Mounted Layer", "Used this to add the Parameters and 'Mounted' Layer from the Mounted Animator to your custom TCP animator ")))
                    {
                        AddLayerMounted(controller);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

        }
        protected override void SetEnable()
        {
            MyRider = (Rider3rdPerson)target;
        }


        protected override string CustomEventsHelp()
        {
            return "On Start Mounting: Invoked when the rider start the mount animation. \n\nOn End Mounting: Invoked when the rider finish the mount animation.\n\nOn Start Dismounting: Invoked when the rider start the dismount animation.\n\nOn End Dismounting: Invoked when the rider finish the dismount animation.";
        }

        protected override void DrawCustomEvents()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStartMounting"), new GUIContent("On Start Mounting"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnEndMounting"), new GUIContent("On End Mounting"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStartDismounting"), new GUIContent("On Start Dismounting"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnEndDismounting"), new GUIContent("On End Dismounting"));
        }


        void AddLayerMounted(AnimatorController AnimController)
        {
            AnimatorController MountedLayerFile = Resources.Load<AnimatorController>("Mounted Layer");
            AnimatorControllerLayer MountedLayer = MountedLayerFile.layers[1];

            UpdateParametersOnAnimator(AnimController);

            AnimController.AddLayer(MountedLayer);
        }


        #region Working Great!

        // Copy all parameters to the new animator
        public static void UpdateParametersOnAnimator(AnimatorController AnimController)
        {
            AnimatorControllerParameter[] parameters = AnimController.parameters;

            if (!SearchParameter(parameters, "Vertical"))
                AnimController.AddParameter("Vertical", AnimatorControllerParameterType.Float);

            if (!SearchParameter(parameters, "Horizontal"))
                AnimController.AddParameter("Horizontal", AnimatorControllerParameterType.Float);

            if (!SearchParameter(parameters, "Stand"))
                AnimController.AddParameter("Stand", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "_Jump"))
                AnimController.AddParameter("_Jump", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "Fall"))
                AnimController.AddParameter("Fall", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "Attack1"))
                AnimController.AddParameter("Attack1", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "Attack2"))
                AnimController.AddParameter("Attack2", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "Stunned"))
                AnimController.AddParameter("Stunned", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "Damaged"))
                AnimController.AddParameter("Damaged", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "Shift"))
                AnimController.AddParameter("Shift", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "Death"))
                AnimController.AddParameter("Death", AnimatorControllerParameterType.Trigger);

            if (!SearchParameter(parameters, "Swim"))
                AnimController.AddParameter("Swim", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "Action"))
                AnimController.AddParameter("Action", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "Mount"))
                AnimController.AddParameter("Mount", AnimatorControllerParameterType.Bool);

            if (!SearchParameter(parameters, "MountSide"))
                AnimController.AddParameter("MountSide", AnimatorControllerParameterType.Int);

            if (!SearchParameter(parameters, "IDAction"))
                AnimController.AddParameter("IDAction", AnimatorControllerParameterType.Int);

            if (!SearchParameter(parameters, "IDInt"))
                AnimController.AddParameter("IDInt", AnimatorControllerParameterType.Int);

            if (!SearchParameter(parameters, "IDFloat"))
                AnimController.AddParameter("IDFloat", AnimatorControllerParameterType.Float);

            if (!SearchParameter(parameters, "Slope"))
                AnimController.AddParameter("Slope", AnimatorControllerParameterType.Float);

            if (!SearchParameter(parameters, "IKLeftFoot"))
                AnimController.AddParameter("IKLeftFoot", AnimatorControllerParameterType.Float);

            if (!SearchParameter(parameters, "IKRightFoot"))
                AnimController.AddParameter("IKRightFoot", AnimatorControllerParameterType.Float);
        }

        //Search for the parameters on the AnimatorController
        public static bool SearchParameter(AnimatorControllerParameter[] parameters, string name)
        {
            foreach (AnimatorControllerParameter item in parameters)
            {
                if (item.name == name) return true;
            }
            return false;
        }
        #endregion
    }
}
