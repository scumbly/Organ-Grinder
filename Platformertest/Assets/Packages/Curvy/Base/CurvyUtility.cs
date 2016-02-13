// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy.ThirdParty.poly2tri;
using System.Reflection;
using FluffyUnderware.Curvy.Legacy;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FluffyUnderware.DevTools;


namespace FluffyUnderware.Curvy.Utils
{

    /// <summary>
    /// Curvy Utility class
    /// </summary>
    public class CurvyUtility
    {
        #region ### Clamping Methods ###

        /// <summary>
        /// Clamps relative position
        /// </summary>
        public static float ClampTF(float tf, CurvyClamping clamping)
        {
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return Mathf.Repeat(tf, 1);
                case CurvyClamping.PingPong:
                    return Mathf.PingPong(tf, 1);
                default:
                    return Mathf.Clamp01(tf);
            }
        }
        /// <summary>
        /// Clamps a float to a range
        /// </summary>
        public static float ClampValue(float tf, CurvyClamping clamping, float minTF, float maxTF)
        {
            
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    float v1 = DTMath.MapValue(0, 1, tf, minTF, maxTF);
                    return DTMath.MapValue(minTF,maxTF,Mathf.Repeat(v1, 1),0,1);
                case CurvyClamping.PingPong:
                    float v2 = DTMath.MapValue(0, 1, tf, minTF, maxTF);
                    return DTMath.MapValue(minTF,maxTF,Mathf.PingPong(v2, 1),0,1);
                default:
                    return Mathf.Clamp(tf, minTF, maxTF);
            }
        }


        /// <summary>
        /// Clamps relative position and sets new direction
        /// </summary>
        public static float ClampTF(float tf, ref int dir, CurvyClamping clamping)
        {
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return Mathf.Repeat(tf, 1);
                case CurvyClamping.PingPong:
                    if (Mathf.FloorToInt(tf) % 2 != 0)
                        dir *= -1;
                    return Mathf.PingPong(tf, 1);
                default:
                    return Mathf.Clamp01(tf);
            }
        }

        /// <summary>
        /// Clamps relative position and sets new direction
        /// </summary>
        public static float ClampTF(float tf, ref int dir, CurvyClamping clamping, float minTF, float maxTF)
        {
            minTF = Mathf.Clamp01(minTF);
            maxTF = Mathf.Clamp(maxTF, minTF, 1);

            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return minTF + Mathf.Repeat(tf, maxTF - minTF);
                case CurvyClamping.PingPong:
                    if (Mathf.FloorToInt(tf / (maxTF - minTF)) % 2 != 0)
                        dir *= -1;
                    return minTF + Mathf.PingPong(tf, maxTF - minTF);
                default:
                    return Mathf.Clamp(tf, minTF, maxTF);
            }
        }

        /// <summary>
        /// Clamps absolute position
        /// </summary>
        public static float ClampDistance(float distance, CurvyClamping clamping, float length)
        {
            if (length == 0)
                return 0;
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return Mathf.Repeat(distance, length);
                case CurvyClamping.PingPong:
                    return Mathf.PingPong(distance, length);
                default:
                    return Mathf.Clamp(distance, 0, length);
            }
        }

        /// <summary>
        /// Clamps absolute position
        /// </summary>
        public static float ClampDistance(float distance, CurvyClamping clamping, float length, float min, float max)
        {
            if (length == 0)
                return 0;
            min = Mathf.Clamp(min, 0, length);
            max = Mathf.Clamp(max, min, length);
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return min + Mathf.Repeat(distance, max - min);
                case CurvyClamping.PingPong:
                    return min + Mathf.PingPong(distance, max - min);
                default:
                    return Mathf.Clamp(distance, min, max);
            }
        }

        /// <summary>
        /// Clamps absolute position and sets new direction
        /// </summary>
        public static float ClampDistance(float distance, ref int dir, CurvyClamping clamping, float length)
        {
            if (length == 0)
                return 0;
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return Mathf.Repeat(distance, length);
                case CurvyClamping.PingPong:
                    if (Mathf.FloorToInt(distance / length) % 2 != 0)
                        dir *= -1;
                    return Mathf.PingPong(distance, length);
                default:
                    return Mathf.Clamp(distance, 0, length);
            }
        }

        /// <summary>
        /// Clamps absolute position and sets new direction
        /// </summary>
        public static float ClampDistance(float distance, ref int dir, CurvyClamping clamping, float length, float min, float max)
        {
            if (length == 0)
                return 0;
            min = Mathf.Clamp(min, 0, length);
            max = Mathf.Clamp(max, min, length);
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return min + Mathf.Repeat(distance, max - min);
                case CurvyClamping.PingPong:
                    if (Mathf.FloorToInt(distance / (max - min)) % 2 != 0)
                        dir *= -1;
                    return min + Mathf.PingPong(distance, max - min);
                default:
                    return Mathf.Clamp(distance, min, max);
            }
        }

        #endregion

        /// <summary>
        /// Gets the default material, i.e. /Packages/Curvy/Resources/CurvyDefaultMaterial
        /// </summary>
        public static Material GetDefaultMaterial()
        {
            var mat = Resources.Load("CurvyDefaultMaterial") as Material;
            if (mat == null)
            {
                mat = new Material(Shader.Find("Diffuse"));
            }
            return mat;
        }
       

        #region ### OBSELETE ###
        /*! \cond OBSOLETE */
        [System.Obsolete("Use CurvySpline.IsPlanar() instead")]
        public static bool IsPlanar(CurvySpline spline, out int ignoreAxis)
        {
            return spline.IsPlanar(out ignoreAxis);
        }

        [System.Obsolete("Use CurvySpline.IsPlanar() instead")]
        public static bool IsPlanar(CurvySpline spline, out bool xplanar, out bool yplanar, out bool zplanar)
        {
            return spline.IsPlanar(out xplanar, out yplanar, out zplanar);
        }

        [System.Obsolete("Use CurvySpline.MakePlanar() instead!")]
        public static void MakePlanar(CurvySpline spline, int axis)
        {
            spline.MakePlanar(axis);
        }

        [System.Obsolete("Use CurvySpline.SetPivot() instead!")]
        public static Vector3 SetPivot(CurvySpline spline, float xPercent = 0, float yPercent = 0, float zPercent=0, bool preview=false)
        {
            return spline.SetPivot(xPercent,yPercent,zPercent,preview);
        }

        [System.Obsolete("Use CurvySpline.SetPivot() instead!")]
        public static void CenterPivot(CurvySpline spline)
        {
            spline.SetPivot();
        }

        [System.Obsolete("Use CurvySplineSegment.SetAsFirstCP() instead!")]
        public static void SetFirstCP(CurvySplineSegment newStartCP)
        {
            if (newStartCP)
                newStartCP.SetAsFirstCP();
        }

        [System.Obsolete("Use CurvySpline.Flip() instead")]
        public static void FlipSpline(CurvySpline spline)
        {
            spline.Flip();
        }

        [System.Obsolete("Use CurvySplineSegment.SplitSpline() instead!")]
        public static CurvySpline SplitSpline(CurvySplineSegment firstCP)
        {
            return firstCP.SplitSpline();
        }

        [System.Obsolete("Use CurvySpline.MoveControlPoints() instead!")]
        public static void MoveControlPoints(CurvySpline source, int startIndex, int count, CurvySplineSegment destCP)
        {
            source.MoveControlPoints(startIndex, count, destCP);
        }

        [System.Obsolete("Use CurvySpline.JoinWith() instead!")]
        public static void JoinSpline(CurvySpline source, CurvySplineSegment destCP)
        {
            source.JoinWith(destCP);
        }

        [System.Obsolete("Use CurvySplineSegment.SetBezierHandles() instead!")] 
        public static void InterpolateBezierHandles(CurvyInterpolation interpolation, float offset, bool? freeMoveHandles, params CurvySplineSegment[] controlPoints) 
        { 
            if (controlPoints.Length == 0) return; 
            offset = Mathf.Clamp01(offset); 
            foreach (CurvySplineSegment cp in controlPoints) { 
                bool movestate = freeMoveHandles.HasValue ? freeMoveHandles.Value : cp.FreeHandles; 
                cp.FreeHandles = movestate; 
                CurvySplineSegment other = cp.PreviousSegment; 
  
                
                if (other) 
                    cp.HandleIn = other.Interpolate(1 - offset, interpolation) - cp.transform.localPosition; 
                else 
                        cp.HandleIn = cp.Interpolate(0, interpolation); 
  
                if (cp.FreeHandles) 
                {
                    if (cp.IsValidSegment)
                        cp.HandleOut = cp.Interpolate(offset, interpolation) - cp.transform.localPosition;
                    else
                        cp.HandleIn = Vector3.zero; 
  
                } 
                 
                
            } 
            controlPoints[0].Spline.Refresh();
        }

        /*! \endcond */
        #endregion



    }

    #region ### Spline2Mesh ###

    /// <summary>
    /// Class to create a Mesh from a set of splines
    /// </summary>
    public class Spline2Mesh
    {
        #region ### Public Fields & Properties ###
        /// <summary>
        /// Outline spline
        /// </summary>
        public SplinePolyLine Outline;
        /// <summary>
        /// Splines to build holes
        /// </summary>
        public List<SplinePolyLine> Holes = new List<SplinePolyLine>();
        public Vector2 UVTiling = Vector2.one;
        public Vector2 UVOffset = Vector2.zero;
        public bool SuppressUVMapping;
        /// <summary>
        /// Whether UV2 should be set
        /// </summary>
        public bool UV2;
        /// <summary>
        /// Name of the returned mesh
        /// </summary>
        public string MeshName = string.Empty;
        /// <summary>
        /// Whether only vertices of the outline spline should be created
        /// </summary>
        public bool VertexLineOnly;

        public string Error { get; private set; }

        #endregion

        #region ### Private Fields ###

        bool mUseMeshBounds;
        Vector2 mNewBounds;
        Polygon p2t;
        Mesh mMesh;

        #endregion

        #region ### Public Methods ###

        /// <summary>
        /// Create the Mesh using the current settings
        /// </summary>
        /// <param name="result">the resulting Mesh</param>
        /// <returns>true on success. If false, check the Error property!</returns>
        public bool Apply(out Mesh result)
        {
            p2t = null;
            mMesh = null;
            Error = string.Empty;
            if (triangulate())
            {
                if (buildMesh())
                {
                    if (!SuppressUVMapping && !VertexLineOnly)
                        uvmap();
                    result = mMesh;
                    return true;
                }
            }
            if (mMesh)
                mMesh.RecalculateNormals();
            result = mMesh;
            return false;
        }

        /// <summary>
        /// Sets bounds used by UV calculation
        /// </summary>
        /// <param name="useMeshBounds">whether to use mesh bounds</param>
        /// <param name="newSize">the new size</param>
        public void SetBounds(bool useMeshBounds, Vector2 newSize)
        {
            mUseMeshBounds = useMeshBounds;
            mNewBounds = newSize;
        }

        #endregion

        #region ### Privates ###
        /*! \cond PRIVATE */

        bool triangulate()
        {
            if (Outline == null || Outline.Spline == null)
            {
                Error = "Missing Outline Spline";
                return false;
            }
            if (!polyLineIsValid(Outline))
            {
                Error = Outline.Spline.name + ": Angle must be >0";
                return false;
            }

            Vector3[] outlineVerts = Outline.getVertices();
            if (outlineVerts.Length < 3)
            {
                Error = Outline.Spline.name + ": At least 3 Vertices needed!";
                return false;
            }
            p2t = new Polygon(outlineVerts);

            if (VertexLineOnly)
                return true;

            for (int i = 0; i < Holes.Count; i++)
            {
                if (Holes[i].Spline == null)
                {
                    Error = "Missing Hole Spline";
                    return false;
                }
                if (!polyLineIsValid(Holes[i]))
                {
                    Error = Holes[i].Spline.name + ": Angle must be >0";
                    return false;
                }
                Vector3[] verts = Holes[i].getVertices();
                if (verts.Length < 3)
                {
                    Error = Holes[i].Spline.name + ": At least 3 Vertices needed!";
                    return false;
                }
                p2t.AddHole(new Polygon(verts));
            }
            try
            {
                P2T.Triangulate(p2t);
                return true;
            }
            catch (System.Exception e)
            {
                Error = e.Message;
            }

            return false;
        }

        bool polyLineIsValid(SplinePolyLine pl)
        {
            return (pl != null && pl.VertexMode == SplinePolyLine.VertexCalculation.ByApproximation ||
                    !Mathf.Approximately(0, pl.Angle));
        }

        bool buildMesh()
        {
            mMesh = new Mesh();
            mMesh.name = MeshName;


            if (VertexLineOnly)
            {
                mMesh.vertices = Outline.getVertices();
            }
            else
            {
                List<Vector3> vertices = new List<Vector3>();
                List<int> triangleIndices = new List<int>();

                for (int t = 0; t < p2t.Triangles.Count; t++)
                {
                    var tri = p2t.Triangles[t];
                    for (int p = 0; p < 3; p++)
                        if (!vertices.Contains(tri.Points[p].V3))
                            vertices.Add(tri.Points[p].V3);

                    triangleIndices.Add(vertices.IndexOf(tri.Points[2].V3));
                    triangleIndices.Add(vertices.IndexOf(tri.Points[1].V3));
                    triangleIndices.Add(vertices.IndexOf(tri.Points[0].V3));
                }
                mMesh.vertices = vertices.ToArray();
                mMesh.triangles = triangleIndices.ToArray();
            }
            mMesh.RecalculateBounds();
            return true;
        }

        void uvmap()
        {
            Bounds bounds = mMesh.bounds;

            Vector2 UVBounds = bounds.size;
            if (!mUseMeshBounds)
                UVBounds = mNewBounds;

            Vector3[] vt = mMesh.vertices;

            Vector2[] uv = new Vector2[vt.Length];

            float maxU = 0;
            float maxV = 0;

            for (int i = 0; i < vt.Length; i++)
            {
                float u = UVOffset.x + (vt[i].x - bounds.min.x) / UVBounds.x;
                float v = UVOffset.y + (vt[i].y - bounds.min.y) / UVBounds.y;
                u *= UVTiling.x;
                v *= UVTiling.y;
                maxU = Mathf.Max(u, maxU);
                maxV = Mathf.Max(v, maxV);
                uv[i] = new Vector2(u, v);
            }
            mMesh.uv = uv;
            Vector2[] uv2 = new Vector2[0];
            if (UV2)
            {
                uv2 = new Vector2[uv.Length];
                for (int i = 0; i < vt.Length; i++)
                    uv2[i] = new Vector2(uv[i].x / maxU, uv[i].y / maxV);
            }
            mMesh.uv2 = uv2;
            mMesh.RecalculateNormals();
        }

        /*! \endcond */
        #endregion
    }

    /// <summary>
    /// Spline Triangulation Helper Class
    /// </summary>
    [System.Serializable]
    public class SplinePolyLine
    {
        /// <summary>
        /// How to calculate vertices
        /// </summary>
        public enum VertexCalculation
        {
            /// <summary>
            /// Use Approximation points
            /// </summary>
            ByApproximation,
            /// <summary>
            /// By curvation angle
            /// </summary>
            ByAngle
        }

        /// <summary>
        /// Base Spline
        /// </summary>
        public CurvySplineBase Spline;
        /// <summary>
        /// Vertex Calculation Mode
        /// </summary>
        public VertexCalculation VertexMode;
        /// <summary>
        /// Angle, used by VertexMode.ByAngle only
        /// </summary>
        public float Angle;
        /// <summary>
        /// Minimum distance, used by VertexMode.ByAngle only
        /// </summary>
        public float Distance;
        public Space Space;

        /// <summary>
        /// Creates a Spline2MeshCurve class using Spline2MeshCurve.VertexMode.ByApproximation
        /// </summary>
        public SplinePolyLine(CurvySplineBase spline) : this(spline, VertexCalculation.ByApproximation, 0, 0) { }
        /// <summary>
        /// Creates a Spline2MeshCurve class using Spline2MeshCurve.VertexMode.ByAngle
        /// </summary>
        public SplinePolyLine(CurvySplineBase spline, float angle, float distance) : this(spline, VertexCalculation.ByAngle, angle, distance) { }

        SplinePolyLine(CurvySplineBase spline, VertexCalculation vertexMode, float angle, float distance, Space space=Space.World)
        {
            Spline = spline;
            VertexMode = vertexMode;
            Angle = angle;
            Distance = distance;
            Space = space;
        }
        /// <summary>
        /// Gets whether the spline is closed
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return (Spline && Spline.IsClosed);
            }
        }

        /// <summary>
        /// Gets whether the spline is continuous
        /// </summary>
        public bool IsContinuous
        {
            get
            {
                return (Spline && Spline.IsContinuous);
            }
        }

        /// <summary>
        /// Get vertices calculated using the current VertexMode
        /// </summary>
        /// <returns>an array of vertices</returns>
        public Vector3[] getVertices()
        {
            Vector3[] points = new Vector3[0];
            switch (VertexMode)
            {
                case VertexCalculation.ByAngle:
                    List<float> tf;
                    List<Vector3> tan;
                    points = Spline.GetPolygon(0, 1, Angle, Distance, -1, out tf, out tan, false);
                    break;
                default:
                    points=Spline.GetApproximation();
                    break;
            }
            if (Space == Space.World)
            {
                for (int i = 0; i < points.Length; i++)
                    points[i] = Spline.transform.TransformPoint(points[i]);
            }
            return points;
        }

    }
    #endregion

   
}
