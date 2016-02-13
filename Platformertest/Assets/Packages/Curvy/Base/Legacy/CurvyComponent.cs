// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy.Utils;
using FluffyUnderware.DevTools;

namespace FluffyUnderware.Curvy
{
    /// <summary>
    /// Base Class for Curvy Controllers, working in the editor
    /// </summary>
    [ExecuteInEditMode]
    [System.Obsolete]
    public class CurvyComponent : MonoBehaviour
    {
        public delegate void CurvyComponentEvent(CurvyComponent sender);

        /// <summary>
        /// Determines when to update
        /// </summary>
        [Label(Tooltip="Determines when to update")]
        public CurvyUpdateMethod UpdateIn = CurvyUpdateMethod.Update; // when to update?
        public bool DoMove = true;
        public bool DoOrientate = true;
        

        /// <summary>
        /// Gets the (cached) transform
        /// </summary>
        public Transform Transform
        {
            get
            {
                if (!mTransform)
                    mTransform = transform;
                return mTransform;
            }
        }

        /// <summary>
        /// Gets Time.deltaTime - even in the editor!
        /// </summary>
        public float DeltaTime
        {
            get { return DTTime.deltaTime; }
        }


        Transform mTransform;

        void Update()
        {
            if (UpdateIn == CurvyUpdateMethod.Update && Application.isPlaying)
                Refresh();

        }

        void LateUpdate()
        {
            if (UpdateIn == CurvyUpdateMethod.LateUpdate)
                Refresh();
        }

        void FixedUpdate()
        {
            if (UpdateIn == CurvyUpdateMethod.FixedUpdate)
                Refresh();
        }



        /// <summary>
        /// Called about 100 times a second when the component is selected
        /// </summary>
        public virtual void EditorUpdate()
        {
            DTTime.UpdateEditorTime();
        }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        /// <returns></returns>
        public virtual bool Initialize() { return false; }

        /// <summary>
        /// Called when updating
        /// </summary>
        public virtual void Refresh()
        {
        }
    }
}
