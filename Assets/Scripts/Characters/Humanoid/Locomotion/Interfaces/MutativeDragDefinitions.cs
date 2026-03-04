using System;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Locomotion
{
    [Serializable]
    public class MutativeDragDefinitions : IMutativeDragDefinitions
    {
        [SerializeField]
        float _transitionDuration;
        [SerializeField]
        Vector2 _range;

        public Vector2 Range { get => _range; }
        public float TransitionDuration { get => _transitionDuration; }
    }
}
