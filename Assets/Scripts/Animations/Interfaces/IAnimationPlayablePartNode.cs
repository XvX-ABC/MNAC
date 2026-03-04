using MNAC.Utilities.MTrees;
using UnityEngine.Playables;

namespace MNAC.Animations
{
    public interface IAnimationPlayablePartNode : IMTContainerNode<IAnimationPlayablePart>
    {
        public PlayableGraph Graph { get; set; }
    }
}
