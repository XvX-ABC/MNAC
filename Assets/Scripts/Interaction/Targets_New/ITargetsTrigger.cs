using UnityEngine;

namespace MNAC.Interaction
{
    public interface ITargetsTrigger<T>:ICatcher<T>
    {
        LayerMask ExcludeLayerMask { get; set; }
        LayerMask IncludeLayerMask { get; set; }

        void OnTriggerEnter(Collider collider);
        void OnTriggerExit(Collider collider);
    }
}