using Tests.Input;
using Tests.States;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public enum BehaviourState
    {
        None,
        Ready,
        Running,
        Ended,
    }
    public interface IArmBehaviour : IState
    {
        public static bool TryBeginAllBehaviours(IArmBehaviour[] subBehaviours)
        {
            if (subBehaviours == null || subBehaviours.Length == 0)
                return false;
            var result = true;
            foreach (var b in subBehaviours)
            {
                //var s = b.OnEnter();
                //if (!s)
                //{
                //    var name = b.GetType().Name;
                //    Debug.LogWarning($"The behaviour '{name}' to start failed.");
                //    result = false;
                //}
                b.OnEnter();
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
                //var s = b.OnExit();
                //if (!s)
                //{
                //    var name = b.GetType().Name;
                //    Debug.LogWarning($"The behaviour '{name}' to end failed.");
                //    result = false;
                //}
                b.OnEnter();
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
        public bool Continuing { get => false; }
        public void OnAnimatorIK(int layerIndex) { }
    }
}
