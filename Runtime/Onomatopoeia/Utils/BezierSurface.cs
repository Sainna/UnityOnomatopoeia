using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngineInternal;

public class BezierSurface : MonoBehaviour
{
    [SerializeField] private BezierCurve[] _Curves;

    [Range(0, 64)] public int _GizmosPoints;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (int i = 0; i < _GizmosPoints; i++)
        {
            float ti = i / (float)_GizmosPoints;
            for (int j = 0; j < _GizmosPoints; j++)
            {
                float tj = j / (float) _GizmosPoints;
                // OrientedPoint p1 = _Curves[0].GetBezierPoint(ti);
                // OrientedPoint p2 = _Curves[1].GetBezierPoint(ti);
                Vector3 point = Vector3.Lerp(_Curves[0].GetBezierPosition(ti), _Curves[1].GetBezierPosition(ti), tj);

                
                // Quaternion lolrota = Quaternion.Slerp(p1.rotation, p2.rotation, tj);
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }

    public BezierCurve TopCurve => _Curves[0];

    public Vector3 GetPoint(float tx, float ty)
    {
        return Vector3.Lerp(_Curves[0].GetBezierPosition(tx), _Curves[1].GetBezierPosition(tx), ty);
    }

    public OrientedPoint GetOrientedPoint(float tx, float ty)
    {
        OrientedPoint p1 = _Curves[0].GetBezierPoint(tx);
        OrientedPoint p2 = _Curves[1].GetBezierPoint(tx);

        return new OrientedPoint(Vector3.Lerp(p1.point, p2.point, ty), Quaternion.Slerp(p1.rotation, p2.rotation, ty));
    }
}
