using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MalbersAnimations.Weapons
{
    [CustomEditor(typeof(MBow))]
    public class MBowEditor : MWeaponEditor
    {
        string[] axis = { "+X", "-X", "+Y", "-Y", "+Z", "-Z" };
        SerializedProperty
              UpperBn, LowerBn;

        MBow myBow;

        private void OnEnable()
        {
            myBow = (MBow)target;
            UpperBn = serializedObject.FindProperty("UpperBn");
            LowerBn = serializedObject.FindProperty("LowerBn");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Bow Weapons Properties", MessageType.None);
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                WeaponProperties();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                myBow.BonesFoldout = EditorGUILayout.Foldout(myBow.BonesFoldout, new GUIContent("Bow Joints", "AllReferences for the Bow Bones"));

                if (myBow.BonesFoldout)
                {
                    myBow.knot = (Transform) EditorGUILayout.ObjectField("Knot", myBow.knot,typeof(Transform),true);
                    myBow.arrowPoint = (Transform)EditorGUILayout.ObjectField("Arrow Point", myBow.arrowPoint, typeof(Transform), true);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(UpperBn, new GUIContent("Upper Chain", "Upper bone chain of the bow"), true);
                    EditorGUILayout.PropertyField(LowerBn, new GUIContent("Lower Chain", "Lower bone chain of the bow"), true);

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        myBow.InitializeBow();
                        EditorUtility.SetDirty(myBow);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                myBow.UpperIndex = EditorGUILayout.Popup("Upper Rot Axis", myBow.UpperIndex, axis);
                myBow.LowerIndex = EditorGUILayout.Popup("Lower Rot Axis", myBow.LowerIndex, axis);
                EditorGUILayout.EndVertical();

                myBow.RotUpperDir = Axis(myBow.UpperIndex);
                myBow.RotLowerDir = Axis(myBow.LowerIndex);

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                myBow.MaxTension = EditorGUILayout.FloatField(new GUIContent("Max Tension", "Max Angle that the Bow can Bent"), myBow.MaxTension);
                myBow.holdTime = EditorGUILayout.FloatField(new GUIContent("Hold Time", "Time to stretch the string to the Max Tension"), myBow.holdTime);
                EditorGUILayout.EndVertical();
                if (myBow.BowisSet)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    myBow.BowTension = EditorGUILayout.Slider(new GUIContent("Bow Tension", "Bow Tension Normalized"), myBow.BowTension, 0, 1);
                    if (myBow.BowTension > 0)
                    {
                        EditorGUILayout.HelpBox("This is for visual purpose only, please return the Bow Tension to 0", MessageType.Warning);
                    }
                    EditorGUILayout.EndVertical();
                }

                if (EditorGUI.EndChangeCheck())
                {
                   
                    if (myBow.MaxTension<0)
                    {
                        myBow.MaxTension = 0;
                    }

                    if (myBow.BowisSet)
                        myBow.BendBow(myBow.BowTension);

                 

                    EditorUtility.SetDirty(myBow);
                }
               

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                   myBow.arrow =(GameObject)  EditorGUILayout.ObjectField(new GUIContent("Arrow", "Arrow Prefab"), myBow.arrow,typeof(GameObject),false );
                }
                EditorGUILayout.EndVertical();

                SoundsList();

                EventList();
                CheckWeaponID();
            }
            EditorGUILayout.EndVertical();

            EditorUtility.SetDirty(target);

            serializedObject.ApplyModifiedProperties();
        }

        public override void UpdateSoundHelp()
        {
            SoundHelp = "0:Draw   1:Store   2:Hold   3:Release";
        }

        public override void CustomEvents()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnLoadArrow"), new GUIContent("On Load Arrow"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnHold"), new GUIContent("On Hold Arrow"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnReleaseArrow"), new GUIContent("On Release Arrow"));
        }

        protected override string CustomEventsHelp()
        {
            return "\n\nOn Load Arrow: Invoked when the arrow is instantiated.\n (GameObject) the instance of the Arrow. \n\nOnHold: Invoked when the bow is being bent (0 to 1)\n\nOn Release Arrow: Invoked when the Arrow is released.\n (GameObject) the instance of the Arrow.";
        }
        Vector3 Axis(int Index)
        {
            switch (Index)
            {
                case 0: return Vector3.right;
                case 1: return -Vector3.right;
                case 2: return Vector3.up;
                case 3: return -Vector3.up;
                case 4: return Vector3.forward;
                case 5: return -Vector3.forward;
                default: return Vector3.zero;
            }
        }
    }
}