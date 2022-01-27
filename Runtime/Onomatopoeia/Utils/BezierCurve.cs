using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sainna.Onomatopoeia
{
    public class BezierCurve : MonoBehaviour
    {

        [SerializeField] private Transform[] _ControlPoints = new Transform[4];


        [Range(0, 1)] [SerializeField] private float tTest = 0;


        Vector3 GetPosition(int idx) => _ControlPoints[idx].position;


        public void OnDrawGizmos()
        {
            for (int i = 0; i < _ControlPoints.Length; i++)
            {
                Gizmos.DrawSphere(GetPosition(i), 0.035f);
            }

            Handles.DrawBezier(GetPosition(0), GetPosition(3), GetPosition(1), GetPosition(2),
                Color.magenta,
                Texture2D.whiteTexture, 
                1f);

            var testPoint = GetBezierPoint(tTest);


            // Debug.Log(Quaternion.identity);
            // Debug.Log(testPoint.rotation);

            Gizmos.color = Color.green;

            Gizmos.DrawSphere(testPoint.point, 0.1f);


            Gizmos.color = Color.cyan;

            Gizmos.DrawSphere(GetBezierPoint(NormDistToT(tTest)).point, 0.1f);

            Gizmos.color = Color.white;




            // for (int i = 0; i < TToLengthLUT; i++)
            // {
            //     
            // }

        }

        public OrientedPoint GetBezierPoint(float t)
        {
            Vector3 p0 = GetPosition(0);
            Vector3 p1 = GetPosition(1);
            Vector3 p2 = GetPosition(2);
            Vector3 p3 = GetPosition(3);

            Vector3 a = Vector3.Lerp(p0, p1, t);
            Vector3 b = Vector3.Lerp(p1, p2, t);
            Vector3 c = Vector3.Lerp(p2, p3, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            Vector3 point = Vector3.Lerp(d, e, t);

            Vector3 forward = GetBezierTangeant(t);
            // Quaternion rot = Quaternion.LookRotation(forward);
            Vector3 side = Vector3.Cross(forward, transform.up);
            Vector3 up = Vector3.Cross(forward, -side);
            Quaternion rot = Quaternion.LookRotation(forward, up);

            return new OrientedPoint(point, rot);
        }

        public Vector3 GetBezierPosition(float t)
        {
            Vector3 p0 = GetPosition(0);
            Vector3 p1 = GetPosition(1);
            Vector3 p2 = GetPosition(2);
            Vector3 p3 = GetPosition(3);

            Vector3 a = Vector3.Lerp(p0, p1, t);
            Vector3 b = Vector3.Lerp(p1, p2, t);
            Vector3 c = Vector3.Lerp(p2, p3, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            return Vector3.Lerp(d, e, t);
        }

        Vector3 GetBezierTangeant(float t)
        {
            Vector3 p0 = GetPosition(0);
            Vector3 p1 = GetPosition(1);
            Vector3 p2 = GetPosition(2);
            Vector3 p3 = GetPosition(3);

            Vector3 a = Vector3.Lerp(p0, p1, t);
            Vector3 b = Vector3.Lerp(p1, p2, t);
            Vector3 c = Vector3.Lerp(p2, p3, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            return (e - d).normalized;
        }


        Quaternion GetBezierOrientation(float t)
        {

            Vector3 forward = GetBezierTangeant(t);
            // Quaternion rot = Quaternion.LookRotation(forward);
            Vector3 side = Vector3.Cross(forward, transform.up);
            Vector3 up = Vector3.Cross(forward, -side);
            Quaternion rot = Quaternion.LookRotation(forward, up);


            return rot;
        }

        public float ArcLength = -1;
        private float[] TToLengthLUT = new float[6];

        private void Awake()
        {
            ComputeArcLength();
        }

        private void LateUpdate()
        {
            bool compute = false;
            foreach (var tr in _ControlPoints)
            {
                if (tr.hasChanged)
                {
                    tr.hasChanged = false;
                    compute = true;
                }
            }

            if (compute)
                ComputeArcLength();
        }

        void ComputeArcLength(int n = 6)
        {
            if (n != TToLengthLUT.Length)
            {
                TToLengthLUT = new float[n];
            }

            float nf = n;
            Vector3 lastPos = _ControlPoints[0].position;
            TToLengthLUT[0] = (0);
            Vector3 currentPos;
            float length = 0;
            for (int i = 1; i < n; i++)
            {
                currentPos = GetBezierPosition(i / nf);
                length += Vector3.Distance(lastPos, currentPos);
                TToLengthLUT[i] = (length);
                lastPos = currentPos;
            }

            ArcLength = length;
        }


        public float NormDistToT(float dist)
        {
            return DistToT(dist * ArcLength);
        }

        float DistToT(float dist)
        {
            float n = TToLengthLUT.Length - 1;


            for (int i = 0; i < TToLengthLUT.Length - 1; i++)
            {
                if ((dist - TToLengthLUT[i]) * (TToLengthLUT[i + 1] - dist) >= 0)
                {
                    var a = i / n;
                    var b = (i + 1) / n;
                    var t1 = (dist - TToLengthLUT[i]);
                    var t2 = TToLengthLUT[i + 1] - TToLengthLUT[i];
                    var t = t1 / t2;
                    return Mathf.Lerp(a, b,
                        t);
                }
            }

            return 1;
        }
    }
}