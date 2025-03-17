using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
namespace Tests.Locomotion
{
    [Obsolete]
    public class JumpCollision : MonoBehaviour
    {
        List<CollisionContext> _obstacles;
        bool _shouldFlush;
        public ReadOnlyCollection<CollisionContext> Obstacles { get => _obstacles.AsReadOnly(); }
        public bool CollidedObstacle { get => _obstacles.Count > 0; }
        public JumpCollision()
        {
            _obstacles = new();
        }
        private void OnCollisionEnter(Collision collision)
        {
            //if (_shouldFlush)
            //{
            //    _obstacles.Clear();
            //    _shouldFlush = false;
            //}
            if (_obstacles.FindIndex(ctx => ctx.Collision == collision) > -1)
                return;
            var context = new CollisionContext() { Collision = collision, ContactPoints = new List<ContactPoint>() };
            collision.GetContacts(context.ContactPoints);
            _obstacles.Add(context);
        }
        private void OnCollisionStay(Collision collision)
        {

        }
        private void OnCollisionExit(Collision collision)
        {
            //if (_shouldFlush)
            //{
            //    _obstacles.Clear();
            //    _shouldFlush = false;
            //}
            var index = _obstacles.FindIndex(ctx => ctx.Collision == collision);
            if (index > -1)
                _obstacles.RemoveAt(index);
        }
        //private void FixedUpdate()
        //{
        //    _shouldFlush = true;
        //}
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            Gizmos.color = Color.red;
            foreach (var cc in _obstacles)
            {
                foreach (var p in cc.ContactPoints)
                    Gizmos.DrawSphere(p.point, 0.3f);
            }
        }
    }
}