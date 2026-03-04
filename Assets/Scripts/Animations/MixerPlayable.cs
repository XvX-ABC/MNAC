using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace MNAC.Animations
{
    public class MixerPlayable : AnimationPlayablePartBase
    {
        public MixerPlayable(PlayableGraph graph, int nodeCount) : base(graph)
        {
            var mixer = AnimationMixerPlayable.Create(graph, Mathf.Max(1, nodeCount));
            mixer.SetInputWeight(0, 1);
            playablePart = mixer;
        }
        public IAnimationPlayablePart GetChild(int index)
        {
            return node.GetChild(index).Value;
        }
        public float GetChildWeight(int index)
        {
            return GetChild(index).OutputSetting.Weight;
        }
        public void SetChildWeight(int index, float weight)
        {
            GetChild(index).OutputSetting.Weight = weight;
        }
    }
}
