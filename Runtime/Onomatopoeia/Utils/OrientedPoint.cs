using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sainna.Onomatopoeia
{
    public struct OrientedPoint
    {
        public Vector3 point;
        public Quaternion rotation;

        public OrientedPoint(Vector3 point, Quaternion rotation)
        {
            this.point = point;
            this.rotation = rotation;
        }


        public OrientedPoint(Vector3 pos, Vector3 forward)
        {
            this.point = pos;
            this.rotation = Quaternion.LookRotation(forward);
        }


        public Vector3 LocalToWorldPos(Vector3 localSpacePos)
        {
            return point + rotation * localSpacePos;
        }


        public Vector3 LocalToWorldVect(Vector3 localSpacePos)
        {
            return rotation * localSpacePos;
        }


    }
}