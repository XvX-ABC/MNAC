using Tests.Animations;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arms.Weapons.Animations
{
    internal class ArmedWeaponPlayablePart : AnimationPlayablePartBase
    {
        IArmedWeaponArmAnimationPlayablePart _animator;

        internal IArmedWeaponArmAnimationPlayablePart animator
        {
            get => _animator;
            set
            {
                _animator = value;
                if (value != null)
                {
                    this.outputSetting = animator.OutputSetting as OutputSetting;
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
