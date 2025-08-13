using Tests.Utilities.MTrees;
using UnityEngine.Playables;

namespace Tests.Character
{
    public interface IAnimationPlayablePartNode : IMTContainerNode<IAnimationPlayablePart>
    {
        public PlayableGraph Graph { get; set; }
    }
}
