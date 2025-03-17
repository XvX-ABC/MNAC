using System.Collections.Generic;
using UnityEngine;
namespace Tests.Locomotion
{
    public struct CollisionContext
    {
        public Collision Collision;
        public List<ContactPoint> ContactPoints;
    }
}