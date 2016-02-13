// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using UnityEngine;
using UnityEditor;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Utils;

namespace FluffyUnderware.CurvyEditor
{
    /// <summary>
    /// Base Editor class for components inherited from CurvyComponent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CustomEditor(typeof(CurvyComponent))]
    [System.Obsolete]
    public class CurvyComponentInspector<T> : CurvyEditorBase<T> where T:CurvyComponent
    {
        bool mRunningInEditor;
        bool mPreviewFoldout;

     

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
            StopPreview();
        }

        /// <summary>
        /// Start editor preview
        /// </summary>
        public virtual void StartPreview()
        {
            if (mRunningInEditor)
                Target.Initialize();
            else
            {
                EditorApplication.update -= Target.EditorUpdate;
                EditorApplication.update += Target.EditorUpdate;
                mRunningInEditor = true;
            }
        }

        /// <summary>
        /// Stop editor preview
        /// </summary>
        public virtual void StopPreview()
        {
            EditorApplication.update -= Target.EditorUpdate;
            mRunningInEditor = false;
            Target.Initialize();
        }

        protected void IterateProperties()
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.name != "m_Script" && iterator.name != "InspectorFoldout")
                    EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
            }
        }

        /// <summary>
        /// Show the preview buttons
        /// </summary>
        protected void ShowPreviewButtons()
        {
            if (CurvyGUI.Foldout(ref mPreviewFoldout, "Preview"))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(mRunningInEditor, new GUIContent("Play/Replay in Editor"), GUI.skin.button) != mRunningInEditor)
                    StartPreview();
                if (GUILayout.Button(new GUIContent("Stop")))
                    StopPreview();
                GUILayout.EndHorizontal();
            }
        }


        protected override void OnCustomInspectorGUI()
        {
            if (CurvyGUI.Foldout(ref mPreviewFoldout, "Preview"))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(mRunningInEditor, new GUIContent("Play/Replay in Editor"), GUI.skin.button) != mRunningInEditor)
                    StartPreview();
                if (GUILayout.Button(new GUIContent("Stop")))
                    StopPreview();
                GUILayout.EndHorizontal();
            }
        }
        /*
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfDirtyOrScript();
            IterateProperties();
            serializedObject.ApplyModifiedProperties();
            ShowPreviewButtons();
        }*/
    }

}
