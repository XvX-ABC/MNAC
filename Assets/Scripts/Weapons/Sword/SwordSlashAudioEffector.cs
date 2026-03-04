using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.Weapons.Sword
{
    internal class SwordSlashAudioEffector : SwordEffector
    {
        AudioSource _audioSource;
        [SerializeField]
        AudioClip _swingClip;
        [SerializeField]
        AudioClip _impactClip;
        protected internal override float duration
        {
            get => base.duration;
            set
            {

                base.duration = value;
                UpdateAudioPitch(value);
            }
        }
        protected override SwordActionType actionType => SwordActionType.Slash;
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            _audioSource = sword.gameObject.GetComponent<AudioSource>() ?? throw new ComponentCantFindException(sword.gameObject, typeof(AudioSource));
            UpdateAudioPitch(duration);
        }
        void UpdateAudioPitch(float duration)
        {
            if (duration == 0)
                return;
            var length = _swingClip.length;
            var v = length / duration;
        }
        protected override void WhenHitTarget(GameObject obj)
        {
            base.WhenHitTarget(obj);
            _audioSource.PlayOneShot(_impactClip);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            _audioSource.PlayOneShot(_swingClip);
        }
    }
}
