using Tests.Input;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    public interface IArmBehaviour
    {
        public static bool TryBeginAllBehaviours(IArmBehaviour[] subBehaviours)
        {
            if (subBehaviours == null || subBehaviours.Length == 0)
                return false;
            var result = true;
            foreach (var b in subBehaviours)
            {
                var s = b.Begin();
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
                var s = b.Begin();
                if (!s)
                {
                    var name = b.GetType().Name;
                    Debug.LogWarning($"The behaviour '{name}' to start failed.");
                    result = false;
                }
            }
            return result;
        }
        public static bool AnyBehaviourIsContinuing(IArmBehaviour[] subBehaviours)
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
        public bool Continuing { get; }
        public bool Begin();
        public bool End();
        public void Update() { }
        public void OnAnimatorIK(int layerIndex);
    }
}
