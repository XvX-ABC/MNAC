using UnityEngine;

namespace Tests.Interaction
{
    public interface IGameObjTarget : ITarget
    {
        public GameObject Obj { get; }
    }
}
