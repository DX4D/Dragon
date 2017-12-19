using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using UnityEditor.Animations;
using System.Collections.Generic;
using System;

namespace MalbersAnimations.HAP
{
    [CustomEditor(typeof(RiderCombat))]
    public class RiderCombatEditor : Editor
    {
        private MonoScript script;
        RiderCombat MyRiderCombat;

        private SerializedProperty
            InputWeapon,
            InputAttack1, InputAttack2,
            InputAim, Reload,
            HitMask,
            HBack,
            HLeft, CombatAbilities,
            HRight,
            HolderLeft,
            HolderRight,
            HolderBack,
            debug;

        private void OnEnable()
        {
            MyRiderCombat = (RiderCombat)target;

            script = MonoScript.FromMonoBehaviour(MyRiderCombat);


            CombatAbilities = serializedObject.FindProperty("CombatAbilities");

            InputWeapon = serializedObject.FindProperty("InputWeapon");
            HitMask = serializedObject.FindProperty("HitMask");
            Reload = serializedObject.FindProperty("Reload");

            InputAttack1 = serializedObject.FindProperty("InputAttack1");
            InputAttack2 = serializedObject.FindProperty("InputAttack2");
            InputAim = serializedObject.FindProperty("InputAim");

            HLeft = serializedObject.FindProperty("HLeft");
            HRight = serializedObject.FindProperty("HRight");
            HBack = serializedObject.FindProperty("HBack");

            HolderLeft = serializedObject.FindProperty("HolderLeft");
            HolderRight = serializedObject.FindProperty("HolderRight");
            HolderBack = serializedObject.FindProperty("HolderBack");

            debug = serializedObject.FindProperty("debug");


            //CombatAbilityList = new ReorderableList(serializedObject, serializedObject.FindProperty("CombatAbilities"), true, true, true, true);

            //CombatAbilityList.drawHeaderCallback = OnAbilityListDrawHeader;
            //CombatAbilityList.drawElementCallback = DrawElementCallback;
            //CombatAbilityList.onAddCallback = onAddCallback;
        }

        ///// <summary>
        ///// Draws all of the added abilities.
        ///// </summary>
        //private void OnAbilityListDrawHeader(Rect rect)
        //{
        //    EditorGUI.LabelField(rect, "Combat Abilities");
        //}
        ///// <summary>
        ///// Draws all of the added abilities.
        ///// </summary>
        //private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        //{
        //    var element = MyRiderCombat.CombatAbilities[index];

        //    Rect R_1 = new Rect(rect.x, rect.y, (rect.width), EditorGUIUtility.singleLineHeight);

        //    element = (RiderCombatAbility)EditorGUI.ObjectField(R_1, element, typeof(RiderCombatAbility), false);
        //}

        //private void onAddCallback(UnityEditorInternal.ReorderableList list)
        //{
        //    if (MyRiderCombat.CombatAbilities == null)
        //    {
        //        MyRiderCombat.CombatAbilities = new System.Collections.Generic.List<RiderCombatAbility>();
        //    }

        //    MyRiderCombat.CombatAbilities.Add(new RiderCombatAbility());  //No se puede crear Huecos para Scriptable Objects
        //}

        /// <summary>
        /// Draws all of the fields for the selected ability.
        /// </summary>
        private void DrawAbility(RiderCombatAbility ability)
        {
            if (ability == null) return;

            SerializedObject abilitySerializedObject;
            abilitySerializedObject = new SerializedObject(ability);
            abilitySerializedObject.Update();

            EditorGUI.BeginChangeCheck();

            var property = abilitySerializedObject.GetIterator();
            property.NextVisible(true);
            do
            {
                EditorGUILayout.PropertyField(property, true);
            } while (property.NextVisible(false));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(ability, "Ability Changed");
                abilitySerializedObject.ApplyModifiedProperties();
                if (ability != null)
                {
                    MalbersEditor.SetObjectDirty(ability);
                }
            }
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("The Combat Mode is managed here", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(HitMask, new GUIContent("Hit Mask", "What to Hit"));
                    MyRiderCombat.Target = (Transform)EditorGUILayout.ObjectField(new GUIContent("Target", "If the Rider has a Target"), MyRiderCombat.Target, typeof(Transform), true);
                    MyRiderCombat.AimDot = (RectTransform)EditorGUILayout.ObjectField(new GUIContent("Aim Dot","UI for Aiming"), MyRiderCombat.AimDot, typeof(RectTransform), true);

                    MyRiderCombat.StrafeOnTarget = EditorGUILayout.Toggle(new GUIContent("Strafe on Target", "If is there a Target change the mount Input to Camera Input "),MyRiderCombat.StrafeOnTarget);
                }
                EditorGUILayout.EndVertical();

                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUI.indentLevel++;
                    MyRiderCombat.Editor_ShowEquipPoints = EditorGUILayout.Foldout(MyRiderCombat.Editor_ShowEquipPoints, "Weapon Equip Points");

                    if (MyRiderCombat.Editor_ShowEquipPoints)
                    {
                        MyRiderCombat.LeftHandEquipPoint = (Transform)EditorGUILayout.ObjectField("Left Hand", MyRiderCombat.LeftHandEquipPoint, typeof(Transform), true);
                        MyRiderCombat.RightHandEquipPoint = (Transform)EditorGUILayout.ObjectField("Right Hand", MyRiderCombat.RightHandEquipPoint, typeof(Transform), true);
                    }

                    Animator rideranimator = MyRiderCombat.GetComponent<Animator>();
                    if (rideranimator)
                    {
                        if (!MyRiderCombat.LeftHandEquipPoint)
                        {
                            MyRiderCombat.LeftHandEquipPoint = rideranimator.GetBoneTransform(HumanBodyBones.LeftHand);
                        }

                        if (!MyRiderCombat.RightHandEquipPoint)
                        {
                            MyRiderCombat.RightHandEquipPoint = rideranimator.GetBoneTransform(HumanBodyBones.RightHand);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();


                //INPUTS 
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUI.indentLevel++;
                    MyRiderCombat.Editor_ShowInputs = EditorGUILayout.Foldout(MyRiderCombat.Editor_ShowInputs, "Inputs");

                    if (MyRiderCombat.Editor_ShowInputs)
                    {
                       
                        EditorGUILayout.PropertyField(InputAttack1, new GUIContent("Attack1", "Attack Right Side "));
                        EditorGUILayout.PropertyField(InputAttack2, new GUIContent("Attack2", "Attack Left Side "));
                        EditorGUILayout.PropertyField(InputAim, new GUIContent("Aim Mode", "Enable Aim mode for Ranged Weapons"));
                        EditorGUILayout.PropertyField(Reload, new GUIContent("Reload", "To Reload Guns"));
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                MyRiderCombat.ActiveHolderSide = (WeaponHolder)EditorGUILayout.EnumPopup(new GUIContent("Active Holder Side", "Holder to draw weapons from, When weapons dont have specific holder"), MyRiderCombat.ActiveHolderSide);
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                {
                    MyRiderCombat.SetActiveHolder(MyRiderCombat.ActiveHolderSide);
                }
                EditorGUI.indentLevel++;

                ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
                //Inventory and Holders
                EditorGUILayout.BeginHorizontal();

                MyRiderCombat.UseInventory = GUILayout.Toggle(MyRiderCombat.UseInventory, new GUIContent("Use Inventory", "Get the Weapons from an Inventory"), EditorStyles.toolbarButton);

                if (MyRiderCombat.UseInventory)
                    MyRiderCombat.UseHolders = false;
                else MyRiderCombat.UseHolders = true;


                MyRiderCombat.UseHolders = GUILayout.Toggle(MyRiderCombat.UseHolders, new GUIContent("Use Holders", "The Weapons are child of the Holders Transform"), EditorStyles.toolbarButton);

                if (MyRiderCombat.UseHolders)
                    MyRiderCombat.UseInventory = false;
                else MyRiderCombat.UseInventory = true;

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

                if (MyRiderCombat.UseInventory)
                {

                    EditorGUILayout.BeginVertical(MalbersEditor.StyleGreen);
                    EditorGUILayout.HelpBox("The weapons gameobjects are received by the method 'SetWeaponByInventory(GameObject)'", MessageType.None);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    MyRiderCombat.AlreadyInstantiated = EditorGUILayout.ToggleLeft(new GUIContent("Already Instantiated", "The weapon is already instantiated before entering 'GetWeaponByInventory'"), MyRiderCombat.AlreadyInstantiated);
                    EditorGUILayout.EndVertical();

                    if (MyRiderCombat.GetComponent<MInventory>())
                    {
                        MyRiderCombat.GetComponent<MInventory>().enabled = true;
                    }
                }

                //Holder Stufss
                if (MyRiderCombat.UseHolders)
                {
                    EditorGUILayout.BeginVertical(MalbersEditor.StyleGreen);
                    EditorGUILayout.HelpBox("The weapons are child of the Holders", MessageType.None);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {

                        EditorGUI.indentLevel++;

                        MyRiderCombat.Editor_ShowHolders = EditorGUILayout.Foldout(MyRiderCombat.Editor_ShowHolders, "Holders");
                        if (MyRiderCombat.Editor_ShowHolders)
                        {
                            EditorGUILayout.PropertyField(HolderLeft, new GUIContent("Holder Left", "The Tranform that has the weapons on the Left  Side"));
                            EditorGUILayout.PropertyField(HolderRight, new GUIContent("Holder Right", "The Tranform that has the weapons on the Right Side"));
                            EditorGUILayout.PropertyField(HolderBack, new GUIContent("Holder Back", "The Tranform that has the weapons on the Back  Side"));

                            MyRiderCombat.Editor_ShowHoldersInput = GUILayout.Toggle(MyRiderCombat.Editor_ShowHoldersInput, "Holders Input", EditorStyles.toolbarButton);
                            {
                                if (MyRiderCombat.Editor_ShowHoldersInput)
                                {
                                    EditorGUILayout.PropertyField(InputWeapon, new GUIContent("Input Weapon", "Draw/Store the Last Weapon"));
                                    EditorGUILayout.PropertyField(HLeft, new GUIContent("Left", "Input to get Weapons from the Left Holder"));
                                    EditorGUILayout.PropertyField(HRight, new GUIContent("Right", "Input to get Weapons from the Right Holder"));
                                    EditorGUILayout.PropertyField(HBack, new GUIContent("Back", "Input to get Weapons from the Back Holder"));
                                }
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();

                    if (MyRiderCombat.GetComponent<MInventory>())
                    {
                        MyRiderCombat.GetComponent<MInventory>().enabled = false;
                    }
                }
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(CombatAbilities, new GUIContent("Rider Combat Abilities", ""), true);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            foreach (var combatAbility in MyRiderCombat.CombatAbilities)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                //EditorGUI.indentLevel++;
                //EditorGUILayout.Foldout(true, combatAbility.name);
                //EditorGUI.indentLevel--;
                DrawAbility(combatAbility);
                EditorGUILayout.EndVertical();
            }



            EditorGUI.indentLevel++;
            DrawEvents();

            EditorGUILayout.PropertyField(debug, new GUIContent("Debug", ""));

            Animator anim = MyRiderCombat.GetComponent<Animator>();

            if (anim && (anim.runtimeAnimatorController != null) && anim.gameObject.activeSelf)
            {
                if (!PrefabUtility.GetPrefabObject(anim) || PrefabUtility.GetPrefabParent(anim))
                {
                   
                    if (anim.GetLayerIndex("Mounted") == -1)
                    {
                        EditorGUILayout.HelpBox("No Mounted Layer Found, Add it the Mounted Layer using the Rider 3rd Person Script", MessageType.Warning);
                    }
                    else
                    {
                        if (anim.GetLayerIndex("Rider Combat") == -1)
                        {
                            if (GUILayout.Button(new GUIContent("Add Rider Combat Layers", "Used for adding the parameters and Layer from the Mounted Animator to your custom character controller animator ")))
                            {
                                AddLayerMounted();
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Rider Combat Change");
            }

            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
        bool EventHelp = false;

        void DrawEvents()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            MyRiderCombat.Editor_ShowEvents = EditorGUILayout.Foldout(MyRiderCombat.Editor_ShowEvents, new GUIContent("Events"));
            EventHelp = GUILayout.Toggle(EventHelp, "?", EditorStyles.miniButton, GUILayout.Width(18));
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            if (MyRiderCombat.Editor_ShowEvents)
            {
                if (EventHelp)
                {
                EditorGUILayout.HelpBox("On Equip Weapon: Invoked when the rider equip a weapon. \n\nOn Unequip Weapon: Invoked when the rider unequip a weapon.\n\nOn Weapon Action: Gets invoked when a new WeaponAction is set\n(See Weapon Actions enum for more detail). \n\nOn Attack: Invoked when the rider is about to Attack(Melee) or Fire(Range)\n\nOn AimSide: Invoked when the rider is Aiming\n 1:The camera is on the Right Side\n-1 The camera is on the Left Side\n 0:The Aim is Reseted\n\nOn Target: Invoked when the Target is changed", MessageType.None);
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnEquipWeapon"  ), new GUIContent("On Equip Weapon"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnUnequipWeapon"), new GUIContent("On Unequip Weapon"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnWeaponAction" ), new GUIContent("On Weapon Action"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAttack"), new GUIContent("On Attack"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAimSide"), new GUIContent("On Aim Side"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTarget"), new GUIContent("On Target"));
            }

            EditorGUILayout.EndVertical();
        }

        void AddLayerMounted()
        {
            UnityEditor.Animations.AnimatorController MountedLayerFile = Resources.Load<UnityEditor.Animations.AnimatorController>("Mounted Layer");

            Animator anim = MyRiderCombat.GetComponent<Animator>();                                         //Get the Animator
            UnityEditor.Animations.AnimatorController CurrentAnimator = (UnityEditor.Animations.AnimatorController)anim.runtimeAnimatorController;        //Get the current RuntimeAnimatorController

            Rider3rdPersonEditor.UpdateParametersOnAnimator(CurrentAnimator);
            UpdateParametersOnAnimator(CurrentAnimator);                                                    //Adding the Parameters Needed

            UnityEditor.Animations.AnimatorControllerLayer RiderCombatLayers = MountedLayerFile.layers[2];                         //Search For the 2nd Layer to Add
            int RiderCombatLayer_Index;

            RiderCombatLayer_Index = anim.GetLayerIndex("Rider Arm Right");
            if (RiderCombatLayer_Index == -1) CurrentAnimator.AddLayer(RiderCombatLayers);                  //Add "Rider Arm Right" Layer

            RiderCombatLayers = MountedLayerFile.layers[3];

            RiderCombatLayer_Index = anim.GetLayerIndex("Rider Arm Left");
            if (RiderCombatLayer_Index == -1) CurrentAnimator.AddLayer(RiderCombatLayers);                  //Add "Rider Arm Left"  Layer


            RiderCombatLayers = MountedLayerFile.layers[4];

            RiderCombatLayer_Index = anim.GetLayerIndex("Rider Combat");
            if (RiderCombatLayer_Index == -1) CurrentAnimator.AddLayer(RiderCombatLayers);                  //Add "Rider Combat" Layer

        }


        #region Working Great!

        // Copy all parameters to the new animator
        void UpdateParametersOnAnimator(UnityEditor.Animations.AnimatorController AnimController)
        {
            UnityEngine.AnimatorControllerParameter[] parameters = AnimController.parameters;

            //RIDER COMBAT!!!!!!!!!!

            if (!Rider3rdPersonEditor.SearchParameter(parameters, "WeaponAim"))
                AnimController.AddParameter("WeaponAim", UnityEngine.AnimatorControllerParameterType.Float);

            if (!Rider3rdPersonEditor.SearchParameter(parameters, "WeaponType"))
                AnimController.AddParameter("WeaponType", UnityEngine.AnimatorControllerParameterType.Int);

            if (!Rider3rdPersonEditor.SearchParameter(parameters, "WeaponHolder"))
                AnimController.AddParameter("WeaponHolder", UnityEngine.AnimatorControllerParameterType.Int);

            if (!Rider3rdPersonEditor.SearchParameter(parameters, "WeaponAction"))
                AnimController.AddParameter("WeaponAction", UnityEngine.AnimatorControllerParameterType.Int);

        }
        #endregion

    }
}