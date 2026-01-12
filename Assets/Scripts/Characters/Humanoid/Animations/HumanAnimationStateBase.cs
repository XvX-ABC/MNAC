using Tests.States;
namespace Tests.Characters.Humanoid.Animations
{
    internal class HumanAnimationStateBase : WithCallbackPlayableState<object>
    {
        public HumanAnimationStateBase(string name, float duration = 0, bool enabled = true) : base(name == null ? "humanoid_animation_state" : $"humanoid_animation_state_{name}", duration, enabled)
        {
        }
    }
}
