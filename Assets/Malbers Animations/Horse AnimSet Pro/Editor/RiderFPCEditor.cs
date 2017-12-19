using UnityEngine;
using System.Collections;
using UnityEditor;
using MalbersAnimations.Events;
using UnityEditor.Animations;

namespace MalbersAnimations.HAP
{
    [CustomEditor(typeof(RiderFPC))]
    public class RiderFPCEditor : RiderEditor
    {
        protected virtual void OnEnable()
        {
            SetEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SetOnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }

        protected override void SetEnable()
        {
            MyRider = (RiderFPC)target;
        }

        protected override string CustomEventsHelp()
        {
            return "On Mounting: Invoked when the rider Mount the Animal. \n\nOn Dismount: Invoked when the rider finish the mount animation." ;
        }

        protected override void DrawCustomEvents()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnMount"), new GUIContent("On Mount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnDismount"), new GUIContent("On Dismount"));
        }
    }
}
