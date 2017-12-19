using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MalbersAnimations.Weapons
{

    public abstract class MWeaponEditor : Editor
    {
        protected MWeapon weapon;
        protected string SoundHelp;
        protected SerializedProperty Sounds;
        bool offsets = true;

        //static Vector3 InvectorOffsetPos_R = new Vector3(2,7.8f,3.3f);
        //static Vector3 InvectorOffsetRot_R = new Vector3(-1.184f, -90, 80.263f);

        //static Vector3 InvectorOffsetPos_L = new Vector3(-2.4f, 9.5f, 4.5f);
        //static Vector3 InvectorOffsetRot_L = new Vector3(0, -92.178f, -85.028f);

        protected void WeaponProperties()
        {
            weapon = (MWeapon)target;

            if (weapon.weaponID == 0)
            {
                weapon.weaponID = Random.Range(10000, 99999);
            }
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Weapon ID", EditorStyles.label, (GUILayout.MinWidth(1)));
                weapon.weaponID = EditorGUILayout.IntField(new GUIContent("", "WeaponID"), weapon.weaponID, GUILayout.MinWidth(1));
                if (GUILayout.Button("Generate", EditorStyles.miniButton, GUILayout.MinWidth(55)))
                {
                    weapon.weaponID = Random.Range(10000, 99999);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Damage Range", GUILayout.MaxWidth(116));
                weapon.minDamage = EditorGUILayout.FloatField(new GUIContent("", "Minimun Damage"), weapon.minDamage, GUILayout.MinWidth(1));
                weapon.maxDamage = EditorGUILayout.FloatField(new GUIContent("", "Maximun Damage"), weapon.maxDamage, GUILayout.MinWidth(1));

                if (weapon.maxDamage < weapon.minDamage) weapon.maxDamage = weapon.minDamage;


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Force Range", GUILayout.MaxWidth(116));
                weapon.minForce = EditorGUILayout.FloatField(new GUIContent("", "Minimun Force"), weapon.minForce, GUILayout.MinWidth(1));
                weapon.maxForce = EditorGUILayout.FloatField(new GUIContent("", "Maximun Force"), weapon.maxForce, GUILayout.MinWidth(1));

                if (weapon.maxForce < weapon.minForce) weapon.maxForce = weapon.minForce;


                EditorGUILayout.EndHorizontal();

                weapon.rightHand = EditorGUILayout.Toggle(new GUIContent(weapon.rightHand ? "Right Hand" : "Left Hand", "Which Hand the weapon uses"), weapon.rightHand);

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                weapon.holder = (WeaponHolder)EditorGUILayout.EnumPopup(new GUIContent("Holder", "The Side where the weapon Draw/Store from"), weapon.holder, GUILayout.MinWidth(10));
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUI.indentLevel++;
                offsets = EditorGUILayout.Foldout(offsets, "Offsets");
                EditorGUI.indentLevel--;
                if (offsets)
                {
                    weapon.positionOffset = EditorGUILayout.Vector3Field(new GUIContent("Position"), weapon.positionOffset);
                    weapon.rotationOffset = EditorGUILayout.Vector3Field(new GUIContent("Rotation"), weapon.rotationOffset);
                }
            }
            EditorGUILayout.EndVertical();
        }

        public virtual void UpdateSoundHelp()
        {}

        public virtual void SoundsList()
        {
            EditorGUI.indentLevel++;
            Sounds = serializedObject.FindProperty("Sounds");
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            UpdateSoundHelp();
            EditorGUILayout.PropertyField(Sounds, new GUIContent("Sounds", "Sounds Played by the weapon"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.HelpBox(SoundHelp, MessageType.None);
            EditorGUILayout.EndVertical();
        }

        protected bool EventHelp;

        public virtual void EventList()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                weapon.ShowEventEditor = EditorGUILayout.Foldout(weapon.ShowEventEditor, "Events");
                EventHelp = GUILayout.Toggle(EventHelp, "?", EditorStyles.miniButton, GUILayout.Width(18));
                EditorGUILayout.EndHorizontal();


                EditorGUI.indentLevel--;
                if (weapon.ShowEventEditor)
                {
                    if (EventHelp)
                    {
                        EditorGUILayout.HelpBox("On Equip Weapon: Invoked when the rider equip this weapon. \n\nOn Unequip Weapon: Invoked when the rider unequip this weapon" + CustomEventsHelp(), MessageType.None);
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnEquiped"), new GUIContent("On Equiped"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnUnequiped"), new GUIContent("On Unequiped"));
                    CustomEvents();
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual string CustomEventsHelp()
        {
            return "";
        }

        public virtual void CustomEvents()
        {
        }

        protected void CheckWeaponID()
        {
            if (weapon.weaponID == 0) 
            {
                EditorGUILayout.HelpBox("Weapon ID needs cant be Zero, Please Set an ID number ", MessageType.Warning);
            }
        }
    }
}