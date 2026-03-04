using MNAC.Utilities.MTrees;
using UnityEngine.Playables;

namespace MNAC.Animations
{
    public class AnimationPlayablePartTree : MTree
    {
        public AnimationPlayablePartTree(PlayableGraph graph)
        {
            root = new AnimationPlayableNode(graph);
            enumerator = new(root);
        }
    }
}
