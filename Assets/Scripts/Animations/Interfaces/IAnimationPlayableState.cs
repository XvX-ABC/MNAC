using Tests.States;

namespace Tests.Animations
{
    public interface IAnimationPlayableState : IWithCallbackPlayableState<object>
    {
        public IAnimationPlayablePartNode Node { get; }
    }
}
