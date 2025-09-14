using UnityEngine;

namespace Tests.Interaction
{
    public class GameObjTarget : IGameObjTarget
    {
        internal GameObject obj;
        internal GameObjTarget() { }
        public GameObjTarget(GameObject obj)
        {
            this.obj = obj;
        }

        public GameObject Obj => obj;

        public Vector3 Position => obj == null ? ITarget.InvalidPosition : obj.transform.position;
    }
}
