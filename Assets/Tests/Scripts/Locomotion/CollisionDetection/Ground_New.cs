using Tests.Locomotion;
using UnityEngine;

namespace Locomotion
{
    public class Ground_New : IGround
    {
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
    }
}
