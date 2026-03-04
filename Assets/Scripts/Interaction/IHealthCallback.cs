using UnityEngine;

namespace MNAC.Interaction
{
    public interface IHealthCallback
    {
        public float TriggerProportion { get; }
        public bool RepetitiveExecution { get; }
        public void Execute(GameObject obj, IHealth health);
    }
}
