using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MalbersAnimations
{

    [CustomEditor(typeof(HorseAppearance))]
    public class HorseAppearanceEditor : Editor
    {

        private SerializedObject serObj;
        private SerializedProperty
                armor, mount;

        int M_skin, M_mount, M_hair, M_armor;

        private void OnEnable()
        {
            serObj = new SerializedObject(target);
            armor = serObj.FindProperty("Armor");
            mount = serObj.FindProperty("Mount");
            M_skin = 0;
            M_mount = 0;
            M_hair = 0;
            M_armor = 0;
        }

        public override void OnInspectorGUI()
        {
            serObj.Update();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Connects the INPUTS to the Locomotion System ", MessageType.None);
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            DrawDefaultInspector();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();


            HorseAppearance MyHorseApp = (HorseAppearance)target;


            //Toogle Mount
            foreach (Transform item in MyHorseApp.MountMesh)
            {
                if (item)
                    item.gameObject.SetActive(mount.boolValue);
            }


            //Toogle Armor
            foreach (Transform item in MyHorseApp.ArmorMesh)
            {
                if (item)
                    item.gameObject.SetActive(armor.boolValue);
            }



            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Skin", "Change the skin of the horse")))
            {

                foreach (Transform item in MyHorseApp.HorseMesh)
                {
                    if (item)
                        item.GetComponent<SkinnedMeshRenderer>().material = MyHorseApp.HorseSkins[M_skin % MyHorseApp.HorseSkins.Length];
                }
                M_skin++;
            }
            if (GUILayout.Button(new GUIContent("Hair", "Change the Armor color")))
            {
                foreach (Transform item in MyHorseApp.HairMesh)
                {
                    if (item)
                        item.GetComponent<SkinnedMeshRenderer>().material = MyHorseApp.HairColor[M_hair % MyHorseApp.HairColor.Length];
                }
                M_hair++;
            }
            if (GUILayout.Button(new GUIContent("Mount", "Change the Mount color")))
            {
                foreach (Transform item in MyHorseApp.MountMesh)
                {
                    if (item)
                        item.GetComponent<SkinnedMeshRenderer>().material = MyHorseApp.MountColor[M_mount % MyHorseApp.MountColor.Length];
                }
                M_mount++;
            }

            if (GUILayout.Button(new GUIContent("Armor", "Change the Armor color")))
            {
                foreach (Transform item in MyHorseApp.ArmorMesh)
                {
                    if (item)
                        item.GetComponent<SkinnedMeshRenderer>().material = MyHorseApp.ArmorColor[M_armor % MyHorseApp.ArmorColor.Length];
                }
                M_armor++;
            }
            GUILayout.EndHorizontal();


            serObj.ApplyModifiedProperties();
        }
    }
}