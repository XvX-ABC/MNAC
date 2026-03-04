using MNAC.Animations;
using UnityEngine.Playables;

namespace MNAC.Behaviours.Arms.Weapons.Animations
{
    internal class ArmedWeaponPlayablePart : AnimationPlayablePartBase
    {
        IArmedArmAnimationPlayablePart _animator;

        internal IArmedArmAnimationPlayablePart animator
        {
            get => _animator;
            set
            {
                _animator = value;
                if (value != null)
                {
                    this.outputSetting = animator.OutputSetting;
                    playablePart = _animator.GetPlayablePart(graph);
                }
                else
                {
                    this.outputSetting = null;
                    playablePart = Playable.Null;
                }
            }
        }
        public ArmedWeaponPlayablePart(PlayableGraph graph) : base(graph)
        {
        }

    }
}
