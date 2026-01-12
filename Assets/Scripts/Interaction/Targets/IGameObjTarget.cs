using UnityEngine;

namespace Tests.Interaction
{
    public interface IGameObjTarget : ITarget_Obsolete
    {
        public GameObject Obj { get; }
    }
}
