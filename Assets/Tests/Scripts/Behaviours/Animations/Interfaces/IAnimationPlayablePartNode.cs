using Tests.Utilities.MTrees;
using UnityEngine.Playables;

namespace Tests.Behaviours.Animations
{
    public interface IAnimationPlayablePartNode : IMTContainerNode<IAnimationPlayablePart>
    {
        public PlayableGraph Graph { get; set; }
    }
}
