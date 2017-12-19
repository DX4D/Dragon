using UnityEngine;
using System.Collections;
using UnityEditor;
using MalbersAnimations.Events;
using UnityEditor.Animations;

namespace MalbersAnimations.HAP
{
    public class RiderEditor : Editor
    {
        protected  Rider MyRider;
       
        bool EventHelp = false;
        bool CallHelp = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SetOnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }
        protected virtual void SetEnable()
        {
            MyRider = (Rider)target;
           
        }

        public virtual void SetOnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Controls the behaviour when is riding the animal ", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {

               

                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUI.indentLevel--;

                    EditorGUILayout.BeginHorizontal();
                    {
                        MyRider.StartMounted = EditorGUILayout.ToggleLeft(new GUIContent(" Start Mounted", "Set an animal to star mounted on it "), MyRider.StartMounted, GUILayout.MaxWidth(110));
                        MyRider.AnimalStored = (Mountable)EditorGUILayout.ObjectField(MyRider.AnimalStored, typeof(Mountable), true);
                    }
                    EditorGUILayout.EndHorizontal();
                    if (MyRider.StartMounted && MyRider.AnimalStored == null)
                    {
                        EditorGUILayout.HelpBox("Select a Animal with 'IMount' interface from the scene if you want to start mounted on it", MessageType.Warning);
                    }
                    EditorGUI.indentLevel++;
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MountInput"), new GUIContent("Mount", "Key or Input to Mount"), true);
                }
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel--;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    MyRider.CreateColliderMounted = EditorGUILayout.ToggleLeft("", MyRider.CreateColliderMounted, GUILayout.MaxWidth(10));
                    EditorGUILayout.LabelField(new GUIContent("Create Capsule Collider while Mounted", "This collider is for hit the Rider while mounted"));
                    EditorGUILayout.EndHorizontal();
                    if (MyRider.CreateColliderMounted)
                    {

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Center Y", GUILayout.MinWidth(40));
                        EditorGUILayout.LabelField("Height", GUILayout.MinWidth(40));
                        EditorGUILayout.LabelField("Radius", GUILayout.MinWidth(40));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        MyRider.Col_Center = EditorGUILayout.FloatField(MyRider.Col_Center);
                        MyRider.Col_height = EditorGUILayout.FloatField(MyRider.Col_height);
                        MyRider.Col_radius = EditorGUILayout.FloatField(MyRider.Col_radius);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                MyRider.DisableComponents = EditorGUILayout.ToggleLeft(new GUIContent("Disable Components", "If some of the scripts are breaking the Rider Script: disable them"), MyRider.DisableComponents);
                EditorGUI.indentLevel++;
                if (MyRider.DisableComponents)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("DisableList"), new GUIContent("Disable List", "Monobehaviours that will be disabled while mounted"), true);
                    if (MyRider.DisableList.Length == 0)
                    {
                        EditorGUILayout.HelpBox("If 'Disable List' is empty , it will disable all Monovehaviours while riding", MessageType.Info);
                    }
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                MyRider.Editor_RiderCallAnimal = EditorGUILayout.Foldout(MyRider.Editor_RiderCallAnimal, "Call Animal");
                CallHelp = GUILayout.Toggle(CallHelp, "?", EditorStyles.miniButton, GUILayout.Width(18));
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                if (MyRider.Editor_RiderCallAnimal)
                {
                    if (CallHelp)
                    {
                    EditorGUILayout.HelpBox("To call an animal, the animal needs to have MointAI(Script) and a NavMeshAgent.\nRemember to bake a NavMesh... See Poly Art Horse as an example", MessageType.None);
                    }
                    MyRider.CallAnimalA = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Call Animal", "Sound to call the Stored Animal"), MyRider.CallAnimalA, typeof(AudioClip), false);
                    MyRider.StopAnimalA = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Stop Animal", "Sound to stop calling the Stored Animal"), MyRider.StopAnimalA, typeof(AudioClip), false);
                    MyRider.RiderAudio = (AudioSource)EditorGUILayout.ObjectField(new GUIContent("Rider Audio Source", "The reference for the audio source"), MyRider.RiderAudio, typeof(AudioSource), false);

                }
                EditorGUILayout.EndVertical();



                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                MyRider.Editor_Events = EditorGUILayout.Foldout(MyRider.Editor_Events, "Events");
                EventHelp = GUILayout.Toggle(EventHelp, "?", EditorStyles.miniButton, GUILayout.Width(18));
                EditorGUI.indentLevel--;
                EditorGUILayout.EndHorizontal();
              
                if (MyRider.Editor_Events)
                {
                    if (EventHelp)
                    {
                        EditorGUILayout.HelpBox(CustomEventsHelp()+"\n\nOn Find Mount: Invoked when the rider founds something to mount.", MessageType.None);
                    }

                    DrawCustomEvents();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFindMount"), new GUIContent("On Find Mount"));

                    if (MyRider.StartMounted)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAlreadyMounted"), new GUIContent("On Already Mounted"));
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;

            }
            EditorGUILayout.EndVertical();

            EditorUtility.SetDirty(target);
          
        }

        protected virtual string CustomEventsHelp()
        {
            return "";
        } 

        protected virtual void DrawCustomEvents()
        { }
    }
}
