using UnityEngine;
using UnityEditor;

namespace MalbersAnimations.HAP
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Mountable), true)]
    public class MHorseEditor : Editor
    {
        Mountable M;
        private MonoScript script;
        bool CallHelp;
        bool helpExperimental;

        private void OnEnable()
        {
            M = (Mountable)target;
            script = MonoScript.FromMonoBehaviour(M);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Makes this GameObject mountable. Need Mount Triggers and IK Goals", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // M.mountType = (MountType)EditorGUILayout.EnumPopup(new GUIContent("Type"), M.mountType);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("active"), new GUIContent("Active", "If the animal can be mounted. Deactivate if the mount is death or destroyed or is not ready to be mountable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mountLayer"), new GUIContent("Layer", "The name of the Animator layer on the rider controller which holds the riding animations"));

                //if (M.instantMount)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mountIdle"), new GUIContent("Mount Idle", "Animation to Play directly when instant mount is enabled"));
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("instantMount"), new GUIContent("Instant Mount", "Ignores the Mounting Animations"));
                EditorGUILayout.EndVertical();
            

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("straightSpine"), new GUIContent("Straight Spine", "Straighten the Mount Point to fix the Rider Animation"));

                    if (M.straightSpine)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("pointOffset"), new GUIContent("Point Offset", "Extra rotation for the Rider to fit the Rider on the correct position"));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("HighLimit"), new GUIContent("High Limit", "if the mount Up Vector messing with the mount Bone holder add some space between them"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("LowLimit"), new GUIContent("Low Limit", "if the mount Up Vector messing with the mount Bone holder add some space between them"));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothSM"), new GUIContent("Smoothness", "Smooth changes between the rotation and the straight Mount"));
                    }
                }
                EditorGUILayout.EndVertical();


            
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    M.ShowLinks = EditorGUILayout.Foldout(M.ShowLinks, "Links");
                    CallHelp = GUILayout.Toggle(CallHelp, "?", EditorStyles.miniButton, GUILayout.Width(18));
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                    if (M.ShowLinks)
                    {
                        if (CallHelp) EditorGUILayout.HelpBox("'Mount Point' is obligatory, the rest are optional", MessageType.None);

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ridersLink"), new GUIContent("Mount Point", "Reference for the Mount Point"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightIK"), new GUIContent("Right Foot", "Reference for the Right Foot correct position on the mount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightKnee"), new GUIContent("Right Knee", "Reference for the Right Knee correct position on the mount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftIK"), new GUIContent("Left Foot", "Reference for the Left Foot correct position on the mount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftKnee"), new GUIContent("Left Knee", "Reference for the Left Knee correct position on the mount"));

                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    M.ShowAnimatorSpeeds = EditorGUILayout.Foldout(M.ShowAnimatorSpeeds, "Animator");
                    helpExperimental = GUILayout.Toggle(helpExperimental, "?", EditorStyles.miniButton, GUILayout.Width(18));
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                    if (M.ShowAnimatorSpeeds)
                    {

                        if (helpExperimental) EditorGUILayout.HelpBox("Changes the Speed on the Rider Animator controller to sync with the Animal Animator.\nThe Original Riding animatios are made for the Horse. Only change the Speeds for other creatures", MessageType.None);

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("syncAnimators"), new GUIContent("Sync Animators", "If both rider and animal animator should be Synced on the Locomotion state.."));
                       // EditorGUILayout.PropertyField(serializedObject.FindProperty("syncAttacks"), new GUIContent("Sync Attacks", "Sync Attack Animations"));

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Animator Speed", EditorStyles.boldLabel);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUIUtility.labelWidth = 40;
                        M.WalkASpeed = EditorGUILayout.FloatField(new GUIContent("Walk", "Riders Animator speed when is Walking"), M.WalkASpeed, GUILayout.MinWidth(10));
                        M.TrotASpeed = EditorGUILayout.FloatField(new GUIContent("Trot", "Riders Animator speed when is Trotting"), M.TrotASpeed, GUILayout.MinWidth(10));
                        M.RunASpeed  = EditorGUILayout.FloatField(new GUIContent("Run", "Riders Animator speed when is Running"), M.RunASpeed, GUILayout.MinWidth(10));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        M.SwimASpeed = EditorGUILayout.FloatField(new GUIContent("Swim", "Riders Animator speed when is Swimming"), M.SwimASpeed,GUILayout.MinWidth(10));
                        M.FlyASpeed  = EditorGUILayout.FloatField(new GUIContent("Fly", "Riders Animator speed when is Flying"), M.FlyASpeed,GUILayout.MinWidth(10));
                        EditorGUILayout.EndHorizontal();
                        EditorGUIUtility.labelWidth = 0;
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnMounted"), new GUIContent("On Mounted", "Invoked when the Montura is mounted"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnDismounted"), new GUIContent("On Dismounted", "Invoked when the Montura is dismounted"));
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();

            if (M.ridersLink == null)
            {
                EditorGUILayout.HelpBox("'Mount Point'  is empty, please set a reference", MessageType.Warning);
            }

            EditorUtility.SetDirty(target);

            serializedObject.ApplyModifiedProperties();
        }
    }
}