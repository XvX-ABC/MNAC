using Tests.States;

namespace Tests.Behaviours.Animations
{
    public interface IAnimationPlayableState : IWithCallbackPlayableState<object>
    {
        public IAnimationPlayablePartNode Node { get; }
    }
}
