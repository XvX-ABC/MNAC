using UnityEngine;

namespace Tests.Interaction
{
    public interface ISphericalObjsTrigger : ITargetsTrigger<GameObject>
    {
        public float Radius { get; set; }

    }
}
