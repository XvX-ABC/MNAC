using System;
using UnityEngine;

namespace Tests.TPhysics.Environment
{
    public struct Ground
    {
        internal static Vector3 CalculateNormal(Ground ground)
        {
            var contactPoints = ground._contactPoints;
            if (contactPoints == null || contactPoints.Length == 0)
                return Vector3.zero;
            var result = Vector3.zero;
            for (int i = 0; i < contactPoints.Length; i++)
            {
                var p = ground._contactPoints[i].normal;
                result += p;
            }
            result /= contactPoints.Length;
            return result;
        }
        ContactPoint[] _contactPoints;
        Vector3 _normal;
        GameObject _obj;

        internal ContactPoint[] ContactPoints { get => _contactPoints; }
        public Vector3 Normal { get => _normal; }
        public GameObject Obj { get => _obj; }

        internal Ground(Collision collisionInfo)
        {
            _contactPoints = null;
            _normal = Vector3.zero;
            _obj = null;


            _obj = collisionInfo.collider.gameObject;
            Update(collisionInfo);
        }
        internal void Update(Collision collision)
        {
            var obj = collision.gameObject;
            if (obj != _obj)
                return;
            _obj = obj;
            _contactPoints = collision.contacts;
            _normal = CalculateNormal(this);
        }
        public override bool Equals(object obj)
        {
            if (obj is Collision collision)
                return this._obj == collision.gameObject;
            else if (obj is Ground ground)
                return this._obj == ground._obj;
            else
                throw new InvalidCastException();
        }
        public override int GetHashCode()
        {
            return _obj.GetHashCode();
        }
    }
}
