using Tests.Behaviours.Animations;
using Tests.Behaviours.Arms.Weapons;
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
            }
        }
        public override bool Enabled => _animator.Enabled;
        public override IOutputSetting OutputSetting { get => _animator.OutputSetting; set => _animator.OutputSetting = value; }
        public ArmedWeaponPlayablePart(PlayableGraph graph) : base(graph)
        {
        }
        public override bool Initialize(PlayableGraph graph)
        {
            playablePart = _animator.GetPlayablePart(graph);
            return true;
        }
        public override void Dispose()
        {
        }
    }
}
