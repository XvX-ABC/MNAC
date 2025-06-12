using Tests.Input;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    public enum BehaviourState
    {
        None,
        Ready,
        Running,
        Ended,
    }
    public interface IArmBehaviour
    {
        public static bool TryBeginAllBehaviours(IArmBehaviour[] subBehaviours)
        {
            if (subBehaviours == null || subBehaviours.Length == 0)
                return false;
            var result = true;
            foreach (var b in subBehaviours)
            {
                var s = b.BStart();
                if (!s)
                {
                    var name = b.GetType().Name;
                    Debug.LogWarning($"The behaviour '{name}' to start failed.");
                    result = false;
                }
            }
            return result;
        }
        public static bool TryEndAllBehaviours(IArmBehaviour[] subBehaviours)
        {
            if (subBehaviours == null || subBehaviours.Length == 0)
                return false;
            var result = true;
            foreach (var b in subBehaviours)
            {
                var s = b.BEnd();
                if (!s)
                {
                    var name = b.GetType().Name;
                    Debug.LogWarning($"The behaviour '{name}' to end failed.");
                    result = false;
                }
            }
            return result;
        }
        public static bool AnyBehaviourIsContinuing(params IArmBehaviour[] subBehaviours)
        {
            if (subBehaviours == null || subBehaviours.Length == 0)
                return false;
            foreach (var b in subBehaviours)
            {
                if (b.Continuing)
                    return true;
            }
            return false;
        }
        public IInput Input { set; }
        public BehaviourState State { get => BehaviourState.None; }
        public bool Continuing { get; }
        public bool BStart();
        public bool BEnd();
        public void OnUpdate() { }
        public void OnAnimatorIK(int layerIndex) { }
    }
}
