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
using FluffyUnderware.DevTools.Extensions;
using System.Reflection;

namespace FluffyUnderware.Curvy
{
    /// <summary>
    /// Controller base class
    /// </summary>
    [ExecuteInEditMode]
    public class CurvyController : DTVersionedMonoBehaviour
    {
        #region ### Enums ###
        /// <summary>
        /// Movement method options
        /// </summary>
        public enum MoveModeEnum
        {
            /// <summary>
            /// Move by Percentage or TF (SplineController only)
            /// </summary>
            Relative,
            /// <summary>
            /// Move by extrapolated distance
            /// </summary>
            AbsoluteExtrapolate,
            /// <summary>
            /// Move by calculated distance
            /// </summary>
            AbsolutePrecise
        }

        /// <summary>
        /// Orientation options
        /// </summary>
        public enum OrientationModeEnum
        {
            /// <summary>
            /// No Orientation
            /// </summary>
            None,
            /// <summary>
            /// Use Orientation/Up-Vector
            /// </summary>
            Orientation,
            /// <summary>
            /// Use Direction/Tangent
            /// </summary>
            Tangent
        }

        /// <summary>
        /// Orientation axis to use
        /// </summary>
        public enum OrientationAxisEnum
        {
            Up,
            Down,
            Forward,
            Backward,
            Left,
            Right
        }

        #endregion

        #region ### Serialized Fields ###

        [Section("General", Sort = 0, HelpURL = CurvySpline.DOCLINK + "curvycontroller_general")]
        [Label(Tooltip = "Determines when to update")]
        public CurvyUpdateMethod UpdateIn = CurvyUpdateMethod.Update; // when to update?
        [SerializeField]
        Space m_Space;
        [Section("Position", HelpURL = CurvySpline.DOCLINK + "curvycontroller_position")]
        [SerializeField]
        CurvyPositionMode m_PositionMode;
        [RangeEx(0,"maxPosition")]
        [SerializeField]
        float m_InitialPosition;

        [Section("Move", HelpURL = CurvySpline.DOCLINK + "curvycontroller_move")]
        
        [SerializeField]
        MoveModeEnum m_MoveMode = MoveModeEnum.AbsolutePrecise;
        [SerializeField]
        float m_Speed=0;
        [SerializeField]
        CurvyClamping m_Clamping = CurvyClamping.Loop;
        [SerializeField]
        bool m_PlayAutomatically = true;
        
        [SerializeField]
        bool m_AdaptOnChange = true;
        [SerializeField]
        bool m_Animate=false;
        [FieldCondition("m_Animate",true)]
        [SerializeField]
        AnimationCurve m_Animation=AnimationCurve.EaseInOut(0,0,1,1);
        [FieldCondition("m_Animate", true)]
        [SerializeField]
        float m_TimeScale = 1;
        [FieldCondition("m_Animate", true)]
        [SerializeField]
        bool m_SingleShot;
        [FieldCondition("m_Animate", true)]
        [SerializeField]
        bool m_ResetOnStop = false;
        

        [Section("Orientation",HelpURL=CurvySpline.DOCLINK+"curvycontroller_orientation")]
        [Label("Source","Source Vector")]
        [SerializeField]
        OrientationModeEnum m_OrientationMode = OrientationModeEnum.Orientation;
        [Label("Target", "Target Vector3")]
        [SerializeField]
        OrientationAxisEnum m_OrientationAxis = OrientationAxisEnum.Up;

        [Positive]
        [SerializeField]
        float m_DampingDirection;
        [Positive]
        [SerializeField]
        float m_DampingUp;
        
        
        [Tooltip("Ignore direction when moving backwards?")]
        [SerializeField]
        bool m_IgnoreDirection;

        #endregion

        #region ### Public Properties ###

        /// <summary>
        /// Gets or sets whether to use local or global space
        /// </summary>
        public Space Space
        {
            get { return m_Space; }
            set
            {
                if (m_Space != value)
                    m_Space = value;
            }
        }

        /// <summary>
        /// Gets or sets the position mode to use
        /// </summary>
        public CurvyPositionMode PositionMode
        {
            get { return m_PositionMode; }
            set
            {
                if (m_PositionMode != value)
                {
                    m_PositionMode = value;
                    if (!Application.isPlaying)
                        Prepare();
                }
            }
        }

        /// <summary>
        /// Gets or sets the movement mode to use
        /// </summary>
        public MoveModeEnum MoveMode
        {
            get { return m_MoveMode; }
            set
            {
                if (m_MoveMode != value)
                    m_MoveMode = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to start playing automatically
        /// </summary>
        public bool PlayAutomatically
        {
            get { return m_PlayAutomatically; }
            set
            {
                if (m_PlayAutomatically != value)
                    m_PlayAutomatically = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to keep position when the source length changes
        /// </summary>
        public virtual bool AdaptOnChange
        {
            get { return m_AdaptOnChange; }
            set
            {
                if (m_AdaptOnChange != value)
                    m_AdaptOnChange = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to animate the movement
        /// </summary>
        public bool Animate
        {
            get { return m_Animate; }
            set
            {
                if (m_Animate != value)
                    m_Animate = value;
            }
        }

        /// <summary>
        /// Gets or sets the movement animation curve to apply
        /// </summary>
        public AnimationCurve Animation
        {
            get { return m_Animation; }
            set
            {
                if (m_Animation != value)
                    m_Animation = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration the animation curve refers to
        /// </summary>
        public float TimeScale
        {
            get { return m_TimeScale; }
            set
            {
                if (m_TimeScale != value)
                    m_TimeScale = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to play the animation just once
        /// </summary>
        public bool SingleShot
        {
            get { return m_SingleShot; }
            set
            {
                if (m_SingleShot != value)
                    m_SingleShot = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to reset to the initial position on stop
        /// </summary>
        public bool ResetOnStop
        {
            get { return m_ResetOnStop; }
            set
            {
                if (m_ResetOnStop != value)
                    m_ResetOnStop = value;
            }
        }

        /// <summary>
        /// Gets or sets what to do when the source's end is reached
        /// </summary>
        public CurvyClamping Clamping
        {
            get { return m_Clamping; }
            set
            {
                if (m_Clamping != value)
                    m_Clamping = value;
            }
        }

        /// <summary>
        /// Gets or sets how to apply rotation
        /// </summary>
        public OrientationModeEnum OrientationMode
        {
            get { return m_OrientationMode; }
            set
            {
                if (m_OrientationMode != value)
                    m_OrientationMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the axis to apply the rotation to
        /// </summary>
        public OrientationAxisEnum OrientationAxis
        {
            get { return m_OrientationAxis; }
            set
            {
                if (m_OrientationAxis != value)
                    m_OrientationAxis = value;
                
            }
        }

        /// <summary>
        /// Gets or sets the time direction change is applied over
        /// </summary>
        public float DampingDirection
        {
            get { return m_DampingDirection;}
            set
            {
                float v = Mathf.Max(0, value);
                if (m_DampingDirection != v)
                    m_DampingDirection = v;
            }
        }

        /// <summary>
        /// Gets or sets the time orientation change is applied over
        /// </summary>
        public float DampingUp
        {
            get { return m_DampingUp; }
            set
            {
                float v = Mathf.Max(0, value);
                if (m_DampingUp != v)
                    m_DampingUp = v;
            }
        }

        

        /// <summary>
        /// Gets or sets whether to ignore direction when moving backwards or to rotate 180�
        /// </summary>
        public bool IgnoreDirection
        {
            get { return m_IgnoreDirection; }
            set
            {
                if (m_IgnoreDirection != value)
                    m_IgnoreDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets the initial/starting position
        /// </summary>
        public virtual float InitialPosition
        {
            get { return m_InitialPosition; }
            set
            {
                float v = m_InitialPosition;
                switch (PositionMode)
                {
                    case CurvyPositionMode.Relative:
                        v = CurvyUtility.ClampTF(value,Clamping);//Mathf.Clamp01(value);
                        break;
                    case CurvyPositionMode.WorldUnits:
                        if (IsInitialized)
                            v = CurvyUtility.ClampDistance(value,Clamping,Length);//Mathf.Clamp(value, 0, Length);
                        break;
                }

                if (m_InitialPosition != v)
                {
                    m_InitialPosition = v;
                    if (!Application.isPlaying)
                        Prepare();
                }
                mInitialVirtualPos = v;

                
            }
        }

        /// <summary>
        /// Gets or sets the speed either in world units or relative, depending on MoveMode
        /// </summary>
        public float Speed
        {
            get { return m_Speed; }
            set
            {
                if (m_Speed != value)
                    m_Speed = value;

                mDirection = (int)Mathf.Sign(m_Speed);
            }
        }

        /// <summary>
        /// Gets or sets the relative position on the source, respecting Clamping
        /// </summary>
        public float RelativePosition
        {
            get { return mTF; }
            set
            {
                if (mTF != value)
                {
                    mTF = CurvyUtility.ClampTF(value, Clamping);
                    mForceUpdate = true;
                    if (Animate && IsPlaying)
                        Stop();
                }
            }
        }

        /// <summary>
        /// Gets or sets the absolute position on the source, respecting Clamping
        /// </summary>
        public float AbsolutePosition
        {
            get
            {
                return RelativeToAbsolute(mTF);
            }
            set
            {
                float v = AbsoluteToRelative(value);
                if (mTF != v)
                {
                    
                    mTF = v;
                    mForceUpdate = true;
                    if (Animate && IsPlaying)
                        Stop();
                }
            }
        }

        /// <summary>
        /// Gets or sets the position on the source (relative or absolute, depending on MoveMode), respecting Clamping
        /// </summary>
        public float Position
        {
            get
            {
                switch (MoveMode)
                {
                    case MoveModeEnum.Relative:
                        return mTF;
                    default:
                        return RelativeToAbsolute(mTF);
                }
            }
            set
            {
                float v;
                switch (MoveMode)
                {
                    case MoveModeEnum.Relative:
                        v = CurvyUtility.ClampTF(value, Clamping);
                        break;
                    default:
                        v = AbsoluteToRelative(value);
                        break;
                }
                if (mTF != v)
                {
                    mTF = v;
                    mForceUpdate = true;
                    if (Animate && IsPlaying)
                        Stop();
                }
            }
        }
        /// <summary>
        /// Gets whether the GameObject is active (Shortcut to activeInHierarchy)
        /// </summary>
        public bool Active
        {
            get { return (mGameObject != null && mGameObject.activeInHierarchy); }
        }

        /// <summary>
        /// Gets Time.deltaTime - even in the editor!
        /// </summary>
        public float DeltaTime
        {
            get { return DTTime.deltaTime; }
        }

        /// <summary>
        /// Shortcut to Mathf.Abs(Speed)
        /// </summary>
        public float AbsSpeed
        {
            get
            {
                return Mathf.Abs(Speed);
            }
        }

        /// <summary>
        /// Gets whether the controller is playing
        /// </summary>
        public bool IsPlaying
        {
            get { return mIsPlaying; }
        }

        /// <summary>
        /// Gets or sets the movement direction
        /// </summary>
        public int Direction
        {
            get { return mDirection; }
            set
            {
                int v=(int)Mathf.Sign(value);
                if (mDirection != v)
                    mDirection = v;
            }
        }

        #endregion

        #region ### Private Fields ###

        /// <summary>
        /// Relative Position while moving
        /// </summary>
        float mTF;
        /// <summary>
        /// Direction while moving, may change for some clamping modes
        /// </summary>
        /// <remarks>Initially this is setup with Mathf.Sign(Speed)</remarks>
        int mDirection;

        bool mForceUpdate;

        float mInitialVirtualPos;
        bool mIsPlaying;
        float mShotTime;
        float mShotStartTF;
        float mShotStartDistance;
        GameObject mGameObject;
        Vector3 mDampingDirVelocity;
        Vector3 mDampingUpVelocity;

        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */
        protected virtual void OnEnable()
        {
            mGameObject = gameObject;
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void Update()
        {
            if (UpdateIn == CurvyUpdateMethod.Update && Application.isPlaying && (Speed!=0 || mForceUpdate))
                Refresh();
        }

        protected virtual void LateUpdate()
        {
            if (UpdateIn == CurvyUpdateMethod.LateUpdate && (Speed != 0 || mForceUpdate))
                Refresh();
        }

        protected virtual void FixedUpdate()
        {
            if (UpdateIn == CurvyUpdateMethod.FixedUpdate && (Speed != 0 || mForceUpdate))
                Refresh();
        }

        protected virtual void OnTransformParentChanged()
        {
            Prepare();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            PositionMode = m_PositionMode;
            InitialPosition = m_InitialPosition;
            Speed = m_Speed;
            if (!Application.isPlaying)
                Prepare();
        }
#endif

        protected virtual void Reset()
        {
            UpdateIn = CurvyUpdateMethod.Update;
            PositionMode = CurvyPositionMode.Relative;
            InitialPosition = 0;
            PlayAutomatically = true;
            MoveMode = MoveModeEnum.AbsolutePrecise;
            Speed = 0;
            Animate = false;
            Animation = AnimationCurve.Linear(0, 1, 1, 1);
            TimeScale = 1;
            SingleShot = false;
            ResetOnStop = false;
            Clamping = CurvyClamping.Loop;
            OrientationMode = OrientationModeEnum.Orientation;
            OrientationAxis = OrientationAxisEnum.Up;
            IgnoreDirection = false;
        }
        /*! \endcond */
        #endregion

        #region ### Virtual Properties & Methods every controller MAY override  ###

        /// <summary>
        /// Whether the controller is configured, i.e. all neccessary properties set
        /// </summary>
        public virtual bool IsConfigured { get { return true; } }
        /// <summary>
        /// Whether the controller is initialized, i.e. everything configured and dependencies loaded
        /// </summary>
        public virtual bool IsInitialized { get { return IsConfigured && (MoveMode==MoveModeEnum.Relative || Length>0); } }


        /// <summary>
        /// Called about 100 times a second when the component is selected
        /// </summary>
        public virtual void EditorUpdate()
        {
            Refresh();
        }
        
        /// <summary>
        /// Initialize the controller and set starting position
        /// </summary>
        public virtual void Prepare()
        {
            if (IsPlaying)
                Stop();
            if (IsConfigured)
            {
                if (IsInitialized)
                {
                    mDirection = (int)Mathf.Sign(Speed);
                    mTF = GetTF(InitialPosition);
                    applyPositionAndRotation(mTF);
                    UserAfterInit();
                    if (PlayAutomatically && Application.isPlaying)
                        Play();
                }
            }
        }

        public virtual void Play()
        {
            if (IsInitialized)
            {
                if (IsPlaying)
                    Stop();
                mShotStartTF = mTF;
                if (Animate && MoveMode != MoveModeEnum.Relative)
                    mShotStartDistance = RelativeToAbsolute(mShotStartTF);
                mShotTime = 0;
                mIsPlaying = true;
            }
        }

        public virtual void Stop()
        {
            mShotTime = 0;
            mIsPlaying = false;
            if (IsInitialized)
            {
                if (Animate)
                {
                    if (ResetOnStop)
                        mTF = mShotStartTF;
                    if (SingleShot)
                        applyPositionAndRotation(mTF);
                    else
                        Play();
                }
            }
        }


        public virtual void Warp(float delta)
        {
            int dir = (int)Mathf.Sign(delta);
            Advance(ref mTF, ref dir, MoveMode, Mathf.Abs(delta), Clamping);

            Vector3 pos;
            Vector3 tan;
            Vector3 up;

            if (OrientationMode != OrientationModeEnum.None)
            {
                GetInterpolatedSourcePosition(mTF, out pos, out tan, out up);
                ApplyTransformRotation(GetRotation(tan,up));
            }
            else
                pos = GetInterpolatedSourcePosition(mTF);

            ApplyTransformPosition(pos);
        }
        
        /// <summary>
        /// Refresh the controller and advance position
        /// </summary>
        public virtual void Refresh()
        {
            if (IsInitialized && IsPlaying)
            {
                mShotTime += DeltaTime;

                Vector3 pos;
                Vector3 tan;
                Vector3 up;

                if (Animate)
                {
                    mTF = getAnimationTF();

                    if (OrientationMode != OrientationModeEnum.None)
                    {
                        GetInterpolatedSourcePosition(mTF, out pos, out tan, out up);
                        ApplyTransformRotation(GetRotation(Vector3.SmoothDamp(transform.forward, tan, ref mDampingDirVelocity, DampingDirection), Vector3.SmoothDamp(transform.up, up, ref mDampingUpVelocity, DampingUp)));
                    }
                    else
                        pos = GetInterpolatedSourcePosition(mTF);

                    ApplyTransformPosition(pos);
                }
                else
                {
                    Advance(ref mTF, ref mDirection, MoveMode, AbsSpeed * DeltaTime / TimeScale, Clamping);

                    if (OrientationMode != OrientationModeEnum.None)
                    {
                        GetInterpolatedSourcePosition(mTF, out pos, out tan, out up);
                        ApplyTransformRotation(GetRotation(Vector3.SmoothDamp(transform.forward, tan, ref mDampingDirVelocity, DampingDirection), Vector3.SmoothDamp(transform.up, up, ref mDampingUpVelocity, DampingUp)));
                    }
                    else
                        pos=GetInterpolatedSourcePosition(mTF);
                    
                    ApplyTransformPosition(pos);
                }
                
                UserAfterUpdate();

                // Handle Playing state
                if (Animate && mShotTime >= TimeScale)
                    Stop();
            }
            mForceUpdate = false;
        }

        

        /// <summary>
        /// Called before starting to move in editor preview
        /// </summary>
        /// <remarks>Use this to store settings, e.g. the initial VirtualPosition</remarks>
        public virtual void BeginPreview()
        {
            mInitialVirtualPos = InitialPosition;
            Play();
        }
        /// <summary>
        /// Called after ending editor preview
        /// </summary>
        /// <remarks>Use this to restore settings, e.g. the initial VirtualPosition</remarks>
        public virtual void EndPreview()
        {
            mIsPlaying = false;
            InitialPosition = mInitialVirtualPos;
            Speed = m_Speed;
            Prepare();
            
        }

        #endregion

        #region ### Virtual Properties & Methods every Controller SHOULD override ###

        /// <summary>
        /// Gets the source's length
        /// </summary>
        public virtual float Length { get { return 0; } }

        /// <summary>
        /// Advance the controller and return the new position
        /// </summary>
        /// <param name="tf">the current virtual position (either TF or World Units) </param>
        /// <param name="direction">the current direction</param>
        /// <param name="mode">movement mode</param>
        /// <param name="absSpeed">speed, always positive</param>
        /// <param name="clamping">clamping mode</param>
        protected virtual void Advance(ref float tf, ref int direction, MoveModeEnum mode, float absSpeed, CurvyClamping clamping)
        {
        }

        /// <summary>
        /// Set the transform's localRotation
        /// </summary>
        /// <param name="rotation">the new rotation</param>
        protected virtual void ApplyTransformRotation(Quaternion rotation)
        {
            transform.localRotation = rotation;
        }

        /// <summary>
        /// Set the transform's position
        /// </summary>
        /// <remarks>Depending on Space, either localPosition or position needs to be set</remarks>
        /// <param name="position">the new position</param>
        protected virtual void ApplyTransformPosition(Vector3 position) 
        {
            if (Space == Space.Self)
                transform.localPosition = position;
            else
                transform.position = position;
        }

        /// <summary>
        /// Converts distance on source from absolute to relative position, respecting Clamping
        /// </summary>
        /// <param name="worldUnitDistance">distance in world units from the source start</param>
        /// <returns>relative distance (TF) in the range 0..1</returns>
        protected virtual float AbsoluteToRelative(float worldUnitDistance)
        {
            return CurvyUtility.ClampTF(worldUnitDistance / Length,Clamping);
        }

        /// <summary>
        /// Converts distance on source from relative to absolute position, respecting Clamping
        /// </summary>
        /// <param name="relativeDistance">relative distance (TF) from the source start</param>
        /// <returns>distance in world units from the source start</returns>
        protected virtual float RelativeToAbsolute(float relativeDistance)
        {
            return CurvyUtility.ClampDistance(relativeDistance * Length, Clamping, Length);
        }

        /// <summary>
        /// Retrieve the source' relative position, i.e. TF
        /// </summary>
        /// <param name="virtualPosition">either TF or World Units, depending on mode</param>
        /// <returns>a relative position 0..1</returns>
        protected float GetTF(float virtualPosition)
        {
            switch (PositionMode)
            {
                case CurvyPositionMode.WorldUnits:
                    return AbsoluteToRelative(virtualPosition);
                default:
                    return virtualPosition;
            }
        }

        /// <summary>
        /// Retrieve the source position for a given relative position (TF)
        /// </summary>
        /// <param name="tf">relative position</param>
        /// <returns>a world position</returns>
        protected virtual Vector3 GetInterpolatedSourcePosition(float tf)
        {
            return Vector3.zero;
        }

        protected virtual void GetInterpolatedSourcePosition(float tf, out Vector3 position, out Vector3 tangent, out Vector3 up)
        {
            position = Vector3.zero;
            tangent = Vector3.forward;
            up = Vector3.up;
        }

        
        /// <summary>
        /// Retrieve the source Orientation/Up-Vector for a given relative position
        /// </summary>
        /// <param name="tf">relative position</param>
        /// <returns>the Up-Vector</returns>
        protected virtual Vector3 GetOrientation(float tf)
        {
            return transform.up;
        }
        
        /// <summary>
        /// Gets the resulting rotation
        /// </summary>
        /// <param name="tangent">direction vector</param>
        /// <param name="up">up vector</param>
        /// <returns>the rotation the transform will be set to</returns>
        protected virtual Quaternion GetRotation(Vector3 tangent, Vector3 up)
        {
            if (!IgnoreDirection)
                if (mDirection < 0)
                    tangent *= -1;

            if (OrientationMode == OrientationModeEnum.Tangent)
            {
                up = tangent;
                tangent = Vector3.forward;
            }

            switch (OrientationAxis)
            {
                case OrientationAxisEnum.Up:
                    return Quaternion.LookRotation(tangent, up);
                case OrientationAxisEnum.Down:
                    return Quaternion.LookRotation(tangent, up * -1);
                case OrientationAxisEnum.Forward:
                    return Quaternion.LookRotation(up, tangent * -1);
                case OrientationAxisEnum.Backward:
                    return Quaternion.LookRotation(up, tangent);
                case OrientationAxisEnum.Left:
                    return Quaternion.LookRotation(tangent, Vector3.Cross(up, tangent));
                case OrientationAxisEnum.Right:
                    return Quaternion.LookRotation(tangent, Vector3.Cross(tangent, up));
                default:
                    return Quaternion.identity;
            }

        }
        /// <summary>
        /// Gets tangent for a given relative position
        /// </summary>
        /// <param name="tf">relative position</param>
        /// <returns></returns>
        protected virtual Vector3 GetTangent(float tf)
        {
            return Vector3.forward;
        }

        /// <summary>
        /// Binds any external events
        /// </summary>
        protected virtual void BindEvents()
        {
        }
        /// <summary>
        /// Unbinds any external events
        /// </summary>
        protected virtual void UnbindEvents()
        {
        }

        #endregion

        #region ### Virtual Methods for inherited custom controllers (Easy mode) ###

        /// <summary>
        /// Called after the controller needs to set initial position
        /// </summary>
        protected virtual void UserAfterInit() { }
        /// <summary>
        /// Called after the controller has updated it's position
        /// </summary>
        protected virtual void UserAfterUpdate() { }
        
        

        #endregion

        /// <summary>
        /// Event-friedly helper that sets a field or property value
        /// </summary>
        /// <param name="fieldAndValue">e.g. "MyValue=42"</param>
        public void SetFromString(string fieldAndValue)
        {
            string[] f = fieldAndValue.Split('=');
            if (f.Length != 2)
                return;

            var fi = this.GetType().GetFieldIncludingBaseClasses(f[0], BindingFlags.Public | BindingFlags.Instance);
            if (fi != null)
            {
                try
                {
                    if (fi.FieldType.IsEnum)
                        fi.SetValue(this, System.Enum.Parse(fi.FieldType, f[1]));
                    else
                        fi.SetValue(this, System.Convert.ChangeType(f[1], fi.FieldType));
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(this.name + ".SetFromString(): " + e.ToString());
                }
            }
            else
            {
                var pi = this.GetType().GetPropertyIncludingBaseClasses(f[0], BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    try
                    {
                        if (pi.PropertyType.IsEnum)
                            pi.SetValue(this, System.Enum.Parse(pi.PropertyType, f[1]), null);
                        else
                            pi.SetValue(this, System.Convert.ChangeType(f[1], pi.PropertyType), null);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning(this.name + ".SetFromString(): " + e.ToString());
                    }
                }
            }
        }

        #region ### Privates & Internals ###
        /*! \cond PRIVATE */
        
        void applyPositionAndRotation(float tf)
        {
            Vector3 pos;
            Vector3 tan;
            Vector3 up;
            if (OrientationMode != OrientationModeEnum.None)
            {
                GetInterpolatedSourcePosition(tf, out pos, out tan, out up);
                ApplyTransformRotation(GetRotation(tan, up));
            }
            else
                pos = GetInterpolatedSourcePosition(tf);

            ApplyTransformPosition(pos);
            
        }

        
        float getAnimationTF()
        {
            float step = AbsSpeed * Animation.Evaluate(Mathf.Clamp01(mShotTime / TimeScale));
            mDirection = (int)Mathf.Sign(step);

            return (MoveMode == MoveModeEnum.Relative) ? mShotStartTF + step : AbsoluteToRelative(mShotStartDistance + step);
        }

        float maxPosition
        {
            get {return (PositionMode==CurvyPositionMode.Relative) ? 1 : Length;}
        }
        
        /*! \endcond */
        #endregion

        #region ### OBSOLETE ###
        /*! \cond OBSOLETE */
        [System.Obsolete("Use OrientationVelocity instead!")]
        public float OrientationDamping
        {
            get;
            set;
        }
        /*! \endcond */
        #endregion
    }
}
