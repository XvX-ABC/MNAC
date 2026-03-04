using UnityEngine;

namespace MNAC.Interaction
{
    public interface ISphericalObjsTrigger : ITargetsTrigger<GameObject>
    {
        public float Radius { get; set; }

    }
}
