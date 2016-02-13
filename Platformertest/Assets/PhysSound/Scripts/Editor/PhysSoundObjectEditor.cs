using UnityEngine;
using UnityEditor;

namespace PhysSound
{
    [CustomEditor(typeof(PhysSoundObject))]
    [CanEditMultipleObjects]
    public class PhysSoundObjectEditor : Editor
    {
        float dividerHeight = 2;

        SerializedProperty mat, impactAudio, autoCreate;
        PhysSoundObject obj;

        public override void OnInspectorGUI()
        {
            obj = target as PhysSoundObject;

            mat = serializedObject.FindProperty("SoundMaterial");
            impactAudio = serializedObject.FindProperty("ImpactAudio");
            autoCreate = serializedObject.FindProperty("AutoCreateSources");

            serializedObject.Update();

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("PhysSound Material:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mat, true);

            if (obj.SoundMaterial == null)
            {
                EditorGUILayout.HelpBox("No PhysSound Material is assigned!", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (obj.SoundMaterial.AudioSets.Count > 0)
            {
                //EditorGUILayout.Separator();
                GUILayout.Box("", GUILayout.MaxWidth(Screen.width - 25f), GUILayout.Height(dividerHeight));

                EditorGUILayout.LabelField("Audio Sources:", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(autoCreate);

                if (obj.AutoCreateSources)
                {
                    EditorGUILayout.PropertyField(impactAudio, new GUIContent("Template Audio"), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(impactAudio, true);
                    EditorGUILayout.Separator();

                    //Update the audio container list with new objects
                    foreach (PhysSoundAudioSet audSet in obj.SoundMaterial.AudioSets)
                    {
                        if (!obj.HasAudioContainer(audSet.Key))
                        {
                            obj.AddAudioContainer(audSet.Key);
                        }
                    }

                    //Remove any audio containers that don't match with the material.
                    for (int i = 0; i < obj.AudioContainers.Count; i++)
                    {
                        PhysSoundAudioContainer audCont = obj.AudioContainers[i];

                        if (!obj.SoundMaterial.HasAudioSet(audCont.KeyIndex))
                        {
                            obj.RemoveAudioContainer(audCont.KeyIndex);
                            i--;
                        }

                        audCont.SlideAudio = EditorGUILayout.ObjectField(PhysSoundTypeList.GetKey(audCont.KeyIndex) + " Slide Audio", audCont.SlideAudio, typeof(AudioSource), true) as AudioSource;
                    }
                }
            }

            EditorGUILayout.Separator();

            EditorUtility.SetDirty(obj);

            serializedObject.ApplyModifiedProperties();
        }
    }
}