using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion
{
    public interface IMutativeDragDefinitions
    {
        Vector2 Range { get; }
        float TransitionDuration { get; }
    }
}