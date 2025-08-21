using Tests.Utilities.MTrees;
using UnityEngine.Playables;

namespace Tests.Behaviours.Animations
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
