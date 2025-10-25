using UnityEngine;

namespace Tests.Environment
{
    public class Ground : IGround
    {
        internal Collision collision;
        internal GameObject obj;
        internal Vector3 normal;
        internal float height;
        bool _c;
        internal bool collided
        {
            get => _c;
            set
            {
                //Debug.Log("value: " + value);
                _c = value;
            }
        }
        public Vector3 Normal => normal;
        public bool Touched => obj != null;
        public GameObject Obj => obj;

        public Collision Collision { get => collision; }
    }
}
