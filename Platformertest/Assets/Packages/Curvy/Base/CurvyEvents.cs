// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Reflection;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.Curvy.Generator;

namespace FluffyUnderware.Curvy
{
    /// <summary>
    /// Extended UnityEvent
    /// </summary>
    public class UnityEventEx<T0> : UnityEvent<T0>
    {
        object mCallerList;
        PropertyInfo mCallsCount;

        /// <summary>
        /// Removes and adds a listener, ensuring it's only bound once
        /// </summary>
        /// <param name="call"></param>
        public void AddListenerOnce(UnityAction<T0> call) 
        {
            base.RemoveListener(call);
            base.AddListener(call);
        }

        /// <summary>
        /// Whether the event has any listeners at all
        /// </summary>
        /// <returns></returns>
        public bool HasListeners()
        {
            if (mCallsCount == null)
            {
                var fi= typeof(UnityEventBase).GetField("m_Calls", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi != null) {
                    mCallerList = fi.GetValue(this);
                    if (mCallerList != null)
                        mCallsCount = mCallerList.GetType().GetProperty("Count", BindingFlags.Public | BindingFlags.Instance);
                }
            }
            int no = 0;
            if (mCallerList != null && mCallsCount != null)
                no = (int)mCallsCount.GetValue(mCallerList, null);
            no += GetPersistentEventCount();
            return (no > 0);
        }
    }

    #region ### Spline Events ###

    /// <summary>
    /// Class used by spline related events
    /// </summary>
    [System.Serializable]
    public class CurvySplineEvent : UnityEventEx<CurvySplineEventArgs> {}

    /// <summary>
    /// Class used by spline related events
    /// </summary>
    [System.Serializable]
    public class CurvyControlPointEvent : UnityEventEx<CurvyControlPointEventArgs> {}

    /// <summary>
    /// Class used by spline related events
    /// </summary>
    [System.Serializable]
    public class CurvySplineMoveEvent : UnityEventEx<CurvySplineMoveEventArgs> {}

    /// <summary>
    /// EventArgs used by CurvySplineMoveEvent events
    /// </summary>
    public class CurvySplineMoveEventArgs : CurvyControlPointEventArgs
    {
        public bool WorldUnits;
        /// <summary>
        /// Movement direction
        /// </summary>
        public int Direction;
        /// <summary>
        /// The left TF distance yet to move (either TF or world units, depending on move mode)
        /// </summary>
        public float Delta;
        /// <summary>
        /// Current TF
        /// </summary>
        public float TF;
        

        public CurvySplineMoveEventArgs(MonoBehaviour sender, CurvySpline spline, CurvySplineSegment cp, float tf, float delta, int dir, bool worldUnits=false)
            : base(sender,spline, cp)
        {
            Sender = sender;
            Direction = dir;
            Delta = delta;
            TF = tf;
            WorldUnits = worldUnits;
        }
        /// <summary>
        /// Sets the movement to continue at a new position
        /// </summary>
        /// <param name="tf">TF (0..1)</param>
        public void SetPosition(float tf)
        {
            TF = tf;
            if (Spline)
                ControlPoint = Spline.TFToSegment(tf);
        }

        /// <summary>
        /// Sets the movement to continue at a new position
        /// </summary>
        /// <param name="segment">the segment to continue movement on</param>
        /// <param name="localF">the local F within that segment</param>
        public void SetPosition(CurvySplineSegment segment, float localF = 0)
        {
            ControlPoint = segment;
            Spline = segment.Spline;
            TF = Spline.SegmentToTF(ControlPoint, localF);
        }

        /// <summary>
        /// Sets the movement to follow a new spline and alter movement direction to best fit
        /// </summary>
        /// <param name="controlPoint">the Control Point on the new spline to start with</param>
        /// <param name="direction">the direction to use</param>
        public void Follow(CurvySplineSegment controlPoint, ConnectionHeadingEnum direction=ConnectionHeadingEnum.Auto)
        {
            // Change Delta (TF) to have the same distance on the new spline
            if (!WorldUnits)
                Delta *= Spline.Length / controlPoint.Spline.Length;
            // Change Spline
            float tf = controlPoint.LocalFToTF(0);
            SetPosition(controlPoint, 0);

            if (tf == 0 && Direction == -1 && Spline.Closed)
                tf = 1;
            if (tf == 0)
                Direction = 1;
            else if (tf == 1)
                Direction = -1;
            else
            {
                switch (direction)
                {
                    case ConnectionHeadingEnum.Minus:
                        Direction = -1;
                        break;
                    case ConnectionHeadingEnum.Plus:
                        Direction = 1;
                        break;
                }
            }
        }
        /// <summary>
        /// Returns the angle from current position to another Control Point
        /// </summary>
        /// <param name="controlPoint">the reference Control point</param>
        /// <returns>an angle in degrees</returns>
        public float AngleTo(CurvySplineSegment controlPoint)
        {
            float a = 0;
            if (Spline && controlPoint)
            {
                Vector3 t0 = Spline.GetTangentFast(TF);
                Vector3 t1 = controlPoint.Spline.GetTangentFast(controlPoint.LocalFToTF(0));
                bool flip=!controlPoint.Spline.Closed &&
                          ((controlPoint.IsFirstVisibleControlPoint && Direction==-1) ||
                           (controlPoint.IsLastVisibleControlPoint && Direction==1));
                if (flip)
                    t1 *= -1;
                a = Vector3.Angle(t0, t1);
            }
            return a;
        }
    }

    /// <summary>
    /// EventArgs used by CurvyControlPointEvent events
    /// </summary>
    public class CurvyControlPointEventArgs : CurvySplineEventArgs
    {
        /// <summary>
        /// Event Mode
        /// </summary>
        public enum AddMode
        {
            Before,
            After,
            None
        }

        /// <summary>
        /// Determines at which state then event was raised
        /// </summary>
        public AddMode Mode;
        /// <summary>
        /// Related Control Point
        /// </summary>
        public CurvySplineSegment ControlPoint;

        public CurvyControlPointEventArgs(MonoBehaviour sender, CurvySpline spline, CurvySplineSegment cp, AddMode mode=AddMode.None, object data=null) : base(sender,spline,data)
        {
            ControlPoint = cp;
            Mode = mode;
        }

        public CurvyControlPointEventArgs(CurvySpline spline)
            : base(spline, null)
        {
            Mode = AddMode.After;
        }

        
    }

    /// <summary>
    /// EventArgs used by CurvySplineEvent events
    /// </summary>
    public class CurvySplineEventArgs : System.ComponentModel.CancelEventArgs
    {
        /// <summary>
        /// The component raising the event
        /// </summary>
        public MonoBehaviour Sender;
        /// <summary>
        /// The related spline
        /// </summary>
        public CurvySpline Spline;
        /// <summary>
        /// Custom data
        /// </summary>
        public object Data;

        public CurvySplineEventArgs(MonoBehaviour sender, CurvySpline spline = null, object data = null)
        {
            Sender = sender;
            Spline = spline;

            Data = data;
        }

    }

    #endregion

    #region ### CG Events ###

    /// <summary>
    /// Curvy Generator related event
    /// </summary>
    [System.Serializable]
    public class CurvyCGEvent : UnityEventEx<CurvyCGEventArgs> { }

    /// <summary>
    /// EventArgs for CurvyCGEvent events
    /// </summary>
    public class CurvyCGEventArgs : System.EventArgs
    {
        /// <summary>
        /// the component raising the event
        /// </summary>
        public MonoBehaviour Sender;
        /// <summary>
        /// The related CurvyGenerator
        /// </summary>
        public readonly CurvyGenerator Generator;
        /// <summary>
        /// The related CGModule
        /// </summary>
        public readonly CGModule Module;

        public CurvyCGEventArgs(CGModule module)
        {
            Sender = module;
            Generator = module.Generator;
            Module = module;
        }

        public CurvyCGEventArgs(CurvyGenerator generator)
        {
            Sender = generator;
            Generator = generator;
        }

    }

    #endregion

}
