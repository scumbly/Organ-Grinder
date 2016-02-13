﻿using UnityEngine;
using UnityEditor;

namespace PhysSound
{
    [CustomEditor(typeof(PhysSoundMaterial))]
    [CanEditMultipleObjects]
    public class PhysSoundMaterialEditor : Editor
    {
        PhysSoundMaterial mat;
        float dividerHeight = 2;

        FoldoutList audioSetFoldout = new FoldoutList();
        FoldoutList impactsFoldout = new FoldoutList();

        SerializedProperty pitchRand, slidePitchMod, useColVel, scImpVol, relVelThr, impNormBias;

        public override void OnInspectorGUI()
        {
            mat = target as PhysSoundMaterial;
            bool dupeFound = false;
            bool nullFound = false;

            pitchRand = serializedObject.FindProperty("PitchRandomness");
            slidePitchMod = serializedObject.FindProperty("SlidePitchMod");
            useColVel = serializedObject.FindProperty("UseCollisionVelocity");
            scImpVol = serializedObject.FindProperty("ScaleImpactVolume");
            relVelThr = serializedObject.FindProperty("RelativeVelocityThreshold");
            impNormBias = serializedObject.FindProperty("ImpactNormalBias");

            serializedObject.Update();

            EditorGUILayout.LabelField("General Properties:", EditorStyles.boldLabel);

            mat.MaterialTypeKey = EditorGUILayout.Popup("Material Type", mat.MaterialTypeKey, PhysSoundTypeList.PhysSoundTypes);
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(pitchRand);
            EditorGUILayout.PropertyField(slidePitchMod);
            EditorGUILayout.PropertyField(relVelThr, true);
            EditorGUILayout.Slider(impNormBias, 0, 1, "Impact Normal Bias");

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(useColVel);
            EditorGUILayout.PropertyField(scImpVol);

            EditorGUILayout.Separator();
            GUILayout.Box("", GUILayout.MaxWidth(Screen.width - 25f), GUILayout.Height(dividerHeight));

            EditorGUILayout.LabelField("Audio Sets:", EditorStyles.boldLabel);

            if (mat.AudioSets.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.Label("The Audio Set List is Empty.", EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                mat.FallbackTypeIndex = EditorGUILayout.Popup("Fallback Audio Set", mat.FallbackTypeIndex, mat.GetFallbackAudioSets());

                if (mat.UseCollisionVelocity)
                    EditorGUILayout.HelpBox("You are using Collision Velocity. Remember that Impact clips must be ordered from least forceful to most forceful.", MessageType.Info);

                EditorGUILayout.Separator();

                audioSetFoldout.Update(mat.AudioSets.Count, false);
                impactsFoldout.Update(mat.AudioSets.Count, false);

                for (int i = 0; i < mat.AudioSets.Count; i++)
                {
                    PhysSoundAudioSet aud = mat.AudioSets[i];

                    if (hasDuplicate(aud))
                    {
                        dupeFound = true;
                        GUI.color = new Color(1, 0.5f, 0.5f);
                    }

                    if (!PhysSoundTypeList.HasKey(aud.Key))
                    {
                        nullFound = true;
                        GUI.color = new Color(1, 0.5f, 0.5f);
                    }

                    EditorGUILayout.BeginHorizontal();

                    audioSetFoldout[i] = EditorGUILayout.Foldout(audioSetFoldout[i], "Material Type:");
                    GUILayout.Space(35);
                    aud.Key = EditorGUILayout.Popup(aud.Key, PhysSoundTypeList.PhysSoundTypes);

                    GUILayout.FlexibleSpace();

                    Color c = GUI.color;
                    GUI.color = new Color(1, 1, 0.5f);

                    if (GUILayout.Button("Remove", GUILayout.MaxWidth(75), GUILayout.MaxHeight(17)))
                    {
                        mat.AudioSets.RemoveAt(i);
                        i--;
                        continue;
                    }

                    GUI.color = c;

                    EditorGUILayout.EndHorizontal();

                    GUI.color = Color.white;

                    if (audioSetFoldout[i])
                    {
                        GUILayout.BeginVertical(EditorStyles.textField);
                        GUILayout.Space(3);
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(5);
                        GUILayout.BeginVertical();

                        impactsFoldout[i] = EditorGUILayout.Foldout(impactsFoldout[i], "Impact Clips");

                        if (impactsFoldout[i])
                        {
                            AudioClip newClip = null;
                            newClip = EditorGUILayout.ObjectField("Add Clip", newClip, typeof(AudioClip), true) as AudioClip;

                            if (newClip != null)
                            {
                                aud.Impacts.Add(newClip);
                            }

                            for (int j = 0; j < aud.Impacts.Count; j++)
                            {
                                GUILayout.BeginHorizontal();
                                aud.Impacts[j] = EditorGUILayout.ObjectField(aud.Impacts[j], typeof(AudioClip), true) as AudioClip;

                                if (GUILayout.Button("X", GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
                                {
                                    aud.Impacts.RemoveAt(j);
                                    i--;
                                }

                                GUILayout.EndHorizontal();
                            }
                            EditorGUILayout.Separator();
                        }

                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        aud.Slide = EditorGUILayout.ObjectField("Slide Clip", aud.Slide, typeof(AudioClip), true) as AudioClip;

                        GUILayout.Space(3);
                        GUILayout.EndVertical();
                    }

                    GUILayout.Space(2);
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add New Audio Set", GUILayout.MaxWidth(150), GUILayout.Height(25)))
            {
                mat.AudioSets.Add(new PhysSoundAudioSet());
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (dupeFound)
                EditorGUILayout.HelpBox("You have multiple Audio Sets with for the same Material Type! Any duplicate sets will not be used during runtime.", MessageType.Error);
            if (nullFound)
                EditorGUILayout.HelpBox("You have Audio Sets with invalid material types!", MessageType.Error);

            EditorUtility.SetDirty(mat);

            serializedObject.ApplyModifiedProperties();
        }

        bool hasDuplicate(PhysSoundAudioSet aud)
        {
            foreach (PhysSoundAudioSet audSet in mat.AudioSets)
            {
                if (audSet != aud && audSet.Key == aud.Key)
                    return true;
            }

            return false;
        }
    }
}