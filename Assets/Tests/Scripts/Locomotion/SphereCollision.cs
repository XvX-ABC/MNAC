using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Tests.Scripts.Locomotion
{
    internal class SphereCollision : MonoBehaviour
    {
        List<Vector3> _p;
        List<Vector3> _n;
        private void Awake()
        {
            _p = new();
            _n = new();
        }
        private void FixedUpdate()
        {
            _p.Clear();
            var r = Physics.SphereCastAll(this.transform.position, 1.5f, Vector3.right, 0f);
            foreach (var hitInfo in r)
            {
                _p.Add(hitInfo.point);
                _n.Add(hitInfo.normal);
                var obj = hitInfo.collider.gameObject;
                Debug.Log("Collided obj: " + obj.name);
            }

        }
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            Gizmos.color = Color.red;
            foreach (var p in _p)
                Gizmos.DrawSphere(p, 0.1f);
        }
    }
}
