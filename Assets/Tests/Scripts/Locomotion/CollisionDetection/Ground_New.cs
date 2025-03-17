using Tests.Locomotion;
using UnityEngine;

namespace Locomotion
{
    public class Ground_New : IGround
    {
        internal GameObject obj;
        internal Vector3 normal;
        internal float height;
        internal bool collided;
        public Vector3 Normal => normal;
        public bool Touched => obj != null;
        public GameObject Obj => obj;
    }
}
