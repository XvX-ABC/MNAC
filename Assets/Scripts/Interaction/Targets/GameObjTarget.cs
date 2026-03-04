using System;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;

namespace MNAC.Interaction
{
    public class GameObjTarget : IGameObjTarget
    {
        static ObjectPool<GameObjTarget> s_pool;
        static GameObject s_currentObjWhenGet;
        static GameObjTarget()
        {
            s_pool = new(CreateObj, GetObj, ReleaseObj);
        }
        static GameObjTarget CreateObj()
        {
            return new();
        }
        static void GetObj(GameObjTarget target)
        {
            target.obj = s_currentObjWhenGet;
        }
        static void ReleaseObj(GameObjTarget target)
        {
            target.obj = null;
        }
        public static GameObjTarget GetInstance(GameObject obj)
        {
            s_currentObjWhenGet = obj;
            return s_pool.Get();
        }
        public static void ReleaseInstance(GameObjTarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            s_pool.Release(target);
        }
        internal GameObject obj;
        internal GameObjTarget() { }
        public GameObjTarget(GameObject obj)
        {
            this.obj = obj;
        }

        public GameObject Obj => obj;

        public Vector3 Position => obj == null ? ITarget_Obsolete.InvalidPosition : obj.transform.position;
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine("obj: " + obj.name);
            sb.AppendLine("pos: " + Position);
            return sb.ToString();
        }
    }
}
