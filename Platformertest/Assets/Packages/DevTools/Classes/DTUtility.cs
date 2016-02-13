// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using FluffyUnderware.DevTools.Extensions;


namespace FluffyUnderware.DevTools
{
    public static class DTUtility
    {
#if UNITY_EDITOR
        static MethodInfo mGetBuiltinExtraResourcesMethod;
#endif

        public static Material GetDefaultMaterial()
        {
#if UNITY_EDITOR
            if (mGetBuiltinExtraResourcesMethod == null)
            {
                BindingFlags bfs = BindingFlags.NonPublic | BindingFlags.Static;
                mGetBuiltinExtraResourcesMethod = typeof(UnityEditor.EditorGUIUtility).GetMethod("GetBuiltinExtraResource", bfs);
            }
            return (Material)mGetBuiltinExtraResourcesMethod.Invoke(null, new object[] { typeof(Material), "Default-Diffuse.mat" });
#else
            return null;
#endif
        }


        public static bool IsEditorStateChange
        {
            get
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && !UnityEditor.EditorApplication.isPlaying)
                    return true;
                else
#endif
                    return false;
            }
        }


        
        /// <summary>
        /// Much like HandleUtility.GetHandleSize(), but works for gizmos
        /// </summary>
        public static float GetHandleSize(Vector3 position)
        {
            Camera cam = Camera.current;
            position = Gizmos.matrix.MultiplyPoint(position);
            if (cam)
            {
                Transform transform = cam.transform;
                Vector3 position2 = transform.position;
                float z = Vector3.Dot(position - position2, transform.TransformDirection(new Vector3(0f, 0f, 1f)));
                Vector3 a = cam.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(0f, 0f, z)));
                Vector3 b = cam.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(1f, 0f, z)));
                float magnitude = (a - b).magnitude;
                return 80f / Mathf.Max(magnitude, 0.0001f);
            } else
                return 20f;
        }

        /// <summary>
        /// Gets all Types T that have an attribute U
        /// </summary>
        public static Dictionary<U, Type> GetTypesWithAttribute<U, T>()
        {
            var res = new Dictionary<U, Type>();
            Assembly asm = typeof(T).Assembly;
            foreach (Type t in asm.GetTypes())
            {
                if (t.IsSubclassOf(typeof(T)))
                {
                    object[] attribs = t.GetCustomAttributes(typeof(U), false);
                    if (attribs.Length > 0)
                    {
                        res.Add((U)attribs[0], t);
                    }
                }
            } 
            return res; 
        }

        /// <summary>
        /// Gets all fields of a type that have a certain attribute
        /// </summary>
        public static List<FieldInfo> GetFieldsWithAttribute(System.Type type, System.Type attributeType, BindingFlags flags = BindingFlags.Instance|BindingFlags.Public)
        {
            var res = new List<FieldInfo>();
            var fields = type.GetFields(flags);
            foreach (FieldInfo fi in fields)
            {
                var attribs = fi.GetCustomAttributes(attributeType, true);
                if (attribs.Length > 0)
                    res.Add(fi);
            }

            return res;
        }

        public static void SetPlayerPrefs<T>(string key, T value)
        {
            var tt = typeof(T);
            if (tt.IsEnum)
            {
                PlayerPrefs.SetInt(key, Convert.ToInt32(Enum.Parse(typeof(T), value.ToString()) as Enum));
            }
            else if (tt.IsArray)
            {
                throw new System.NotImplementedException();
            }
            else if (tt.Matches(typeof(int), typeof(Int32)))
                PlayerPrefs.SetInt(key, (value as int?).Value);
            else if (tt == typeof(string))
                PlayerPrefs.SetString(key, (value as string));
            else if (tt == typeof(float))
                PlayerPrefs.SetFloat(key, (value as float?).Value);
            else if (tt == typeof(bool))
                PlayerPrefs.SetInt(key, ((value as bool?).Value==true) ? 1:0);
            else if (tt == typeof(Color))
                PlayerPrefs.SetString(key, (value as Color?).Value.ToHtml());
            else
                Debug.LogError("[DevTools.SetEditorPrefs] Unsupported datatype: " + tt.Name);
        }

        public static T GetPlayerPrefs<T>(string key, T defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                var tt = typeof(T);
                try
                {
                    if (tt.IsEnum || tt.Matches(typeof(int), typeof(Int32)))
                    {
                        return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
                    }
                    else if (tt.IsArray)
                    {
                        throw new System.NotImplementedException();
                    }
                    else if (tt == typeof(string))
                        return (T)(object)PlayerPrefs.GetString(key, defaultValue.ToString());
                    else if (tt == typeof(float))
                        return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
                    else if (tt == typeof(bool))
                        return (T)(object)(PlayerPrefs.GetInt(key, ((bool)(object)defaultValue==true) ? 1 : 0)==1);
                    else if (tt == typeof(Color))
                        return (T)(object)PlayerPrefs.GetString(key, ((Color)(object)defaultValue).ToHtml()).ColorFromHtml();
                    else
                        Debug.LogError("[DevTools.SetEditorPrefs] Unsupported datatype: " + tt.Name);
                }
                catch
                {
                    return defaultValue;
                }
            }


            return defaultValue;
        }

        public static float RandomSign()
        {
            return UnityEngine.Random.Range(0, 2) * 2 - 1;
        }


        public static string GetHelpUrl(object forClass)
        {
            return (forClass==null) ? string.Empty : GetHelpUrl(forClass.GetType());
        }

        public static string GetHelpUrl(Type classType)
        {
            if (classType != null)
            {
                var attribs = classType.GetCustomAttributes(typeof(HelpURLAttribute), true);
                if (attribs.Length > 0)
                    return (((HelpURLAttribute)attribs[0]).URL);
            }   
            return string.Empty;
        }

    }

    public static class DTTime
    {
        static float _EditorDeltaTime;
        static float _EditorLastTime;

        public static double TimeSinceStartup
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return UnityEditor.EditorApplication.timeSinceStartup;
#endif
                return Time.timeSinceLevelLoad;
            }
        }

        public static float deltaTime
        {
            get
            {
                return (Application.isPlaying) ? Time.deltaTime : _EditorDeltaTime;
            }
        }

        public static void InitializeEditorTime()
        {
            _EditorLastTime = Time.realtimeSinceStartup;
            _EditorDeltaTime = 0;
        }

        public static void UpdateEditorTime()
        {
            float cur = Time.realtimeSinceStartup;
            float timeDelta = (cur - _EditorLastTime);

            _EditorDeltaTime = timeDelta;
            _EditorLastTime = cur;

            /*
            if (frameDelta > 20 || timeDelta > 1)
            {
                _EditorLastFrame = Time.frameCount;
                _EditorLastTime = cur;
                _EditorDeltaTime = 0;
            }
            else if (frameDelta > 0)
            {
                _EditorDeltaTime = timeDelta / frameDelta;
                _EditorLastTime = cur;
                _EditorLastFrame = Time.frameCount;
            }*/
        }
    }

    public class TimeMeasure : Ring<long>
    {
        public System.Diagnostics.Stopwatch mWatch = new System.Diagnostics.Stopwatch();

        public TimeMeasure(int size) : base(size) { }

        public void Start()
        {
            mWatch.Start();
        }

        public void Stop()
        {
            mWatch.Stop();
            Add(mWatch.ElapsedTicks);
            mWatch.Reset();
        }

        public void Pause()
        {
            mWatch.Stop();
        }

        public double LastTicks
        {
            get
            {
                return this[this.Count - 1];
            }
        }

        public double LastMS
        {
            get
            {
                return LastTicks / (double)System.TimeSpan.TicksPerMillisecond;
            }
        }

        public double AverageMS
        {
            get
            {
                long d = 0;
                for (int i = 0; i < this.Count; i++)
                    d += this[i];

                return DTMath.FixNaN((d / (double)System.TimeSpan.TicksPerMillisecond) / Count);
            }
        }

        public double MinimumMS
        {
            get
            {
                long d = long.MaxValue;
                for (int i = 0; i < this.Count; i++)
                    d = System.Math.Min(d, this[i]);

                return DTMath.FixNaN(d / (double)System.TimeSpan.TicksPerMillisecond);
            }
        }

        public double MaximumMS
        {
            get
            {
                long d = long.MinValue;
                for (int i = 0; i < this.Count; i++)
                    d = System.Math.Max(d, this[i]);

                return DTMath.FixNaN(d / (double)System.TimeSpan.TicksPerMillisecond);
            }
        }

        public double AverageTicks
        {
            get
            {
                long d = 0;
                for (int i = 0; i < this.Count; i++)
                    d += this[i];
                return d / (double)Count;
            }
        }

        public double MinimumTicks
        {
            get
            {
                long d = long.MaxValue;
                for (int i = 0; i < this.Count; i++)
                    d = System.Math.Min(d, this[i]);
                return d;
            }
        }

        public double MaximumTicks
        {
            get
            {
                long d = 0;
                for (int i = 0; i < this.Count; i++)
                    d = System.Math.Max(d, this[i]);
                return d;
            }
        }
    }

    public static class DTMath
    {
        /// <summary>
        /// Much like Mathf.Repeat(), but DTMath.Repeat(v,v) returns v instead of 0
        /// </summary>
        public static float Repeat(float t, float length)
        {
            return (t == length) ? t : t - Mathf.Floor(t / length) * length;
        }

        public static double FixNaN(double v)
        {
            if (double.IsNaN(v))
                v = 0;
            return v;
        }

        public static float FixNaN(float v)
        {
            if (float.IsNaN(v))
                v = 0;
            return v;
        }

        public static Vector2 FixNaN(Vector2 v)
        {
            if (float.IsNaN(v.x))
            {
                v.x = 0;
            }
            if (float.IsNaN(v.y))
            {
                v.y = 0;
            }
            return v;
        }

        /// <summary>
        /// Fixes NaN for Vector3
        /// </summary>
        /// <param name="v">a Vector3</param>
        /// <returns>the "cleaned up" vector</returns>
        public static Vector3 FixNaN(Vector3 v)
        {
            if (float.IsNaN(v.x))
            {
                v.x = 0;
            }
            if (float.IsNaN(v.y))
            {
                v.y = 0;
            }
            if (float.IsNaN(v.z))
            {
                v.z = 0;
            }
            return v;
        }

        /// <summary>
        /// Maps a value from a source range to a destination range
        /// </summary>
        /// <param name="min">min destination value</param>
        /// <param name="max">max destination value</param>
        /// <param name="value">current source value</param>
        /// <param name="vMin">min source value</param>
        /// <param name="vMax">max source value</param>
        /// <returns></returns>
        public static float MapValue(float min, float max, float value, float vMin = -1, float vMax = 1)
        {
            return min + (max - min) * (value - vMin) / (vMax - vMin);
        }

        public static float SnapPrecision(float value, int decimals)
        {
            return (decimals >= 0) ? (float)System.Math.Round(value, decimals) : value;
        }

        public static Vector2 SnapPrecision(Vector2 value, int decimals)
        {
            if (decimals < 0)
                return value;

            value.Set(SnapPrecision(value.x, decimals), SnapPrecision(value.y, decimals));
            return value;
        }

        public static Vector3 SnapPrecision(Vector3 value, int decimals)
        {
            if (decimals < 0)
                return value;
            value.Set(SnapPrecision(value.x, decimals), SnapPrecision(value.y, decimals), SnapPrecision(value.z, decimals));
            return value;
        }

        /// <summary>
        /// Gets the squared distance to the nearest point on a line
        /// </summary>
        /// <param name="l1">Line P1</param>
        /// <param name="l2">Line P2</param>
        /// <param name="p">a Point</param>
        /// <param name="frag">fragment on the line (0..1) of the nearest point</param>
        /// <returns>sqrMagnitude</returns>
        public static float LinePointDistanceSqr(Vector3 l1, Vector3 l2, Vector3 p, out float frag)
        {
            Vector3 v = l2 - l1;
            Vector3 w = p - l1;
            float c1 = Vector3.Dot(w, v);
            if (c1 <= 0)
            {
                frag = 0;
                return (p - l1).sqrMagnitude;
            }
            float c2 = Vector3.Dot(v, v);
            if (c2 <= c1)
            {
                frag = 1;
                return (p - l2).sqrMagnitude;
            }
            frag = c1 / c2;
            Vector3 pb = l1 + frag * v;
            return (p - pb).sqrMagnitude;
        }

        /// <summary>
        /// Collide a ray (point + direction) against a line segment and return the hit point
        /// </summary>
        /// <param name="r0">Ray position</param>
        /// <param name="dir">Ray direction</param>
        /// <param name="l1">Line P1</param>
        /// <param name="l2">Line P2</param>
        /// <param name="hit">Collision Point</param>
        /// <param name="frag">fragment on the line (0..1) of the collision point</param>
        /// <returns>true if collision occurs</returns>
        public static bool RayLineSegmentIntersection(Vector2 r0, Vector2 dir, Vector2 l1, Vector2 l2, out Vector2 hit, out float frag)
        {
            Vector2 s2 = l2 - l1;
            float t;
            frag = (-dir.y * (r0.x - l1.x) + dir.x * (r0.y - l1.y)) / (-s2.x * dir.y + dir.x * s2.y);
            t = (s2.x * (r0.y - l1.y) - s2.y * (r0.x - l1.x)) / (-s2.x * dir.y + dir.x * s2.y);

            if (frag >= 0 && frag <= 1 && t > 0)
            {
                hit = new Vector2(r0.x + (t * dir.x), r0.y + (t * dir.y));
                return true;
            }
            hit = Vector2.zero;
            return false;
        }

        /// <summary>
        /// Calculates the intersection line segment between 2 lines (not segments).
        /// </summary>
        /// <returns>false if no solution can be found</returns>
        public static bool ShortestIntersectionLine(Vector3 line1A, Vector3 line1B,
            Vector3 line2A, Vector3 line2B, out Vector3 resultSegmentA, out Vector3 resultSegmentB)
        {
            // Algorithm is ported from the C algorithm of 
            // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
            resultSegmentA = Vector3.zero;
            resultSegmentB = Vector3.zero;

            Vector3 p1 = line1A;
            Vector3 p2 = line1B;
            Vector3 p3 = line2A;
            Vector3 p4 = line2B;
            Vector3 p13 = p1 - p3;
            Vector3 p43 = p4 - p3;

            if (p43.sqrMagnitude < Mathf.Epsilon)
            {
                return false;
            }
            Vector3 p21 = p2 - p1;
            if (p21.sqrMagnitude < Mathf.Epsilon)
            {
                return false;
            }

            double d1343 = p13.x * (double)p43.x + (double)p13.y * p43.y + (double)p13.z * p43.z;
            double d4321 = p43.x * (double)p21.x + (double)p43.y * p21.y + (double)p43.z * p21.z;
            double d1321 = p13.x * (double)p21.x + (double)p13.y * p21.y + (double)p13.z * p21.z;
            double d4343 = p43.x * (double)p43.x + (double)p43.y * p43.y + (double)p43.z * p43.z;
            double d2121 = p21.x * (double)p21.x + (double)p21.y * p21.y + (double)p21.z * p21.z;

            double denom = d2121 * d4343 - d4321 * d4321;
            if (System.Math.Abs(denom) < double.Epsilon)
            {
                return false;
            }
            double numer = d1343 * d4321 - d1321 * d4343;

            double mua = numer / denom;
            double mub = (d1343 + d4321 * (mua)) / d4343;
            resultSegmentA = new Vector3((float)(p1.x + mua * p21.x), (float)(p1.y + mua * p21.y), (float)(p1.z + mua * p21.z));
            resultSegmentB = new Vector3((float)(p3.x + mub * p43.x), (float)(p3.y + mub * p43.y), (float)(p3.z + mub * p43.z));
            return true;
        }

        /// <summary>
        /// Calculates the intersection between two line segments
        /// </summary>
        /// <returns>false if no solution can be found</returns>
        public static bool LineLineIntersection(Vector3 line1A, Vector3 line1B, Vector3 line2A, Vector3 line2B, out Vector3 hitPoint)
        {
            Vector3 resB;
            if (ShortestIntersectionLine(line1A, line1B, line2A, line2B, out hitPoint, out resB))
            {
                if ((resB - hitPoint).sqrMagnitude <= Mathf.Epsilon * Mathf.Epsilon)
                    return true;
            }
            return false;
        }

        public static bool LineLineIntersect(Vector2 line1A, Vector2 line1B, Vector2 line2A, Vector2 line2B, out Vector2 hitPoint, bool segmentOnly = true)
        {
            hitPoint = Vector2.zero;
            // Denominator for ua and ub are the same, so store this calculation
            double d =
               (line2B.y - line2A.y) * (line1B.x - line1A.x)
               -
               (line2B.x - line2A.x) * (line1B.y - line1A.y);

            //n_a and n_b are calculated as seperate values for readability
            double n_a =
               (line2B.x - line2A.x) * (line1A.y - line2A.y)
               -
               (line2B.y - line2A.y) * (line1A.x - line2A.x);

            double n_b =
               (line1B.x - line1A.x) * (line1A.y - line2A.y)
               -
               (line1B.y - line1A.y) * (line1A.x - line2A.x);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return false;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            double ua = n_a / d;
            double ub = n_b / d;

            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (!segmentOnly || (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d))
            {
                hitPoint.Set((float)(line1A.x + (ua * (line1B.x - line1A.x))), (float)(line1A.y + (ua * (line1B.y - line1A.y))));
                return true;
            }
            return false;
        }
    }
}
