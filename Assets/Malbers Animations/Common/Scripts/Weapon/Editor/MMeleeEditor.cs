using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MalbersAnimations.Weapons
{
    
    [CustomEditor(typeof(MMelee))]
    public class MMeleeEditor : MWeaponEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Melee Weapons Properties", MessageType.None);
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                WeaponProperties();
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("meleeCollider"), new GUIContent("Melee Collider", "Gets the reference of where is the Melee Collider of this weapon (Not Always is in the same gameobject level)"));
                EditorGUILayout.EndVertical();

                SoundsList();

                EventList();

                CheckWeaponID();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        public override void UpdateSoundHelp()
        {
            SoundHelp = "0:Draw   1:Store   2:Swing   3:Hit \n (Leave 3 Empty, add SoundByMaterial and Invoke 'PlayMaterialSound' for custom Hit sounds)";
        }

        public override void CustomEvents()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnCauseDamage"), new GUIContent("On Cause Damage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnHit"), new GUIContent("On Hit Something"));
        }

    }
}