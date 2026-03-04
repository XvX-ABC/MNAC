using UnityEngine;

namespace MNAC.Characters.Humanoid.Locomotion
{
    public interface IMutativeDragDefinitions
    {
        Vector2 Range { get; }
        float TransitionDuration { get; }
    }
}