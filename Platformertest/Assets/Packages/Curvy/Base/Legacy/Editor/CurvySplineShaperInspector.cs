// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy.Legacy;

namespace FluffyUnderware.CurvyEditor.Legacy
{
    [CustomEditor(typeof(SplineShaper))]
    [System.Obsolete]
    public class CurvySplineShaperInspector : Editor
    {

        SplineShaper Target { get { return target as SplineShaper; } }

        CurvyShaperDefinition Definition;
        CurvyShaperDefinition DefinitionLoaded;
        bool LoadGeneral = true;

      
        SerializedProperty tResolution;

        SerializedProperty tRadiusBase;
        SerializedProperty tRadiusModifier;
        SerializedProperty tRadiusModifierCurve;

        SerializedProperty tZ;
        SerializedProperty tZModifier;
        SerializedProperty tZModifierCurve;

        SerializedProperty tRange;
        SerializedProperty tWeld;
        SerializedProperty tName;
        SerializedProperty tm;
        SerializedProperty tn1;
        SerializedProperty tn2;
        SerializedProperty tn3;
        SerializedProperty ta;
        SerializedProperty tb;
        SerializedProperty tAutoRefresh;
        SerializedProperty tAutoRefreshSpeed;

        void OnEnable()
        {
            tResolution = serializedObject.FindProperty("Resolution");
            tRadiusBase = serializedObject.FindProperty("Radius");
            tRadiusModifier = serializedObject.FindProperty("RadiusModifier");
            tRadiusModifierCurve = serializedObject.FindProperty("RadiusModifierCurve");

            tZ = serializedObject.FindProperty("Z");
            tZModifier = serializedObject.FindProperty("ZModifier");
            tZModifierCurve = serializedObject.FindProperty("ZModifierCurve");
            tRange = serializedObject.FindProperty("Range");
            tWeld = serializedObject.FindProperty("WeldThreshold");
            tName = serializedObject.FindProperty("Name");
            tm = serializedObject.FindProperty("m");
            tn1 = serializedObject.FindProperty("n1");
            tn2 = serializedObject.FindProperty("n2");
            tn3 = serializedObject.FindProperty("n3");
            ta = serializedObject.FindProperty("a");
            tb = serializedObject.FindProperty("b");
            tAutoRefresh = serializedObject.FindProperty("AutoRefresh");
            tAutoRefreshSpeed = serializedObject.FindProperty("AutoRefreshSpeed");

        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is obsolete! Use SplineShape instead!", MessageType.Warning);
            Definition = (CurvyShaperDefinition)EditorGUILayout.ObjectField(new GUIContent("Select Shape", "Load Shape Definition"), Definition, typeof(CurvyShaperDefinition), false);
            LoadGeneral = EditorGUILayout.Toggle(new GUIContent("Load General Params", "Check to load saved general parameters"), LoadGeneral);
            if (DefinitionLoaded != Definition)
            {
                Definition.LoadInto(Target, LoadGeneral);
                DefinitionLoaded = Definition;
                EditorUtility.SetDirty(Target);
                serializedObject.UpdateIfDirtyOrScript();
            }
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(tName, new GUIContent("Name", "Name of the effect"));
            EditorGUILayout.PropertyField(tResolution, new GUIContent("Resolution", "Max. number of Control Points"));
            EditorGUILayout.PropertyField(tRange, new GUIContent("Range", "Range in Degree"));
            EditorGUILayout.PropertyField(tRadiusBase, new GUIContent("Radius", "Base Radius"));
            EditorGUILayout.PropertyField(tRadiusModifier, new GUIContent("Radius Modifier", "How to apply Radius curve?"));
            if (tRadiusModifier.enumNames[tRadiusModifier.enumValueIndex] != "None")
                EditorGUILayout.PropertyField(tRadiusModifierCurve, new GUIContent("Radius Curve", "Radius Modifier curve"));

            EditorGUILayout.PropertyField(tZ, new GUIContent("Z", "Base Z"));
            EditorGUILayout.PropertyField(tZModifier, new GUIContent("Z Modifier", "How to apply Z curve?"));
            if (tZModifier.enumNames[tZModifier.enumValueIndex] != "None")
                EditorGUILayout.PropertyField(tZModifierCurve, new GUIContent("Z Curve", "Z Modifier curve"));
            EditorGUILayout.LabelField("Formula Parameters", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(tm);
            EditorGUILayout.PropertyField(tn1);
            EditorGUILayout.PropertyField(tn2);
            EditorGUILayout.PropertyField(tn3);
            EditorGUILayout.PropertyField(ta);
            EditorGUILayout.PropertyField(tb);
            EditorGUILayout.LabelField("Miscellaneous", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(tWeld, new GUIContent("Weld Treshold", "Remove Control Points within a certain distance"));
            EditorGUILayout.PropertyField(tAutoRefresh, new GUIContent("Auto Refresh", "Auto Refresh Shape?"));
            EditorGUILayout.PropertyField(tAutoRefreshSpeed, new GUIContent("Auto Refresh Speed", "Refresh rate in seconds"));

            if (serializedObject.targetObject && serializedObject.ApplyModifiedProperties())
            {
                Target.RefreshImmediately();
                SceneView.RepaintAll();
                Repaint();
            }


            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Refresh"), GUILayout.ExpandWidth(false)))
                Target.RefreshImmediately();
            if (GUILayout.Button(new GUIContent("Save"), GUILayout.ExpandWidth(false)))
                SaveDefinition();
            if (GUILayout.Button(new GUIContent("What are those parameters for?"), GUILayout.ExpandWidth(false)))
                Application.OpenURL("http://paulbourke.net/geometry/supershape/");
            EditorGUILayout.EndHorizontal();
        }

        void SaveDefinition()
        {
            string file = EditorUtility.SaveFilePanelInProject("Save Shape Definition", Target.Name, "asset", "Save Shaper definition as");
            if (!string.IsNullOrEmpty(file))
            {
                var def = CurvyShaperDefinition.Create(Target);
                AssetDatabase.CreateAsset(def, file);
                AssetDatabase.SaveAssets();
            }
        }


    }
}
