using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.Weapons.Launcher
{
    internal class LauncherAudioEffector : LauncherEffector
    {
        AudioSource _audioSource;
        [SerializeField]
        AudioClip _launchClip;
        [SerializeField]
        AudioClip _reloadClip;
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            _audioSource = owner.gameObject.GetComponent<AudioSource>() ?? throw new ComponentCantFindException(owner.gameObject, typeof(AudioSource));
        }
        protected override void WhenLaunch(ILauncher launcher)
        {
            base.WhenLaunch(launcher);
            _audioSource.PlayOneShot(_launchClip);
        }
        protected override void WhenReloadStart(ILauncher launcher)
        {
            base.WhenReloadStart(launcher);
            _audioSource.Stop();
            _audioSource.clip = _reloadClip;
            _audioSource.Play();
        }
        protected override void WhenReloadEnd(ILauncher launcher)
        {
            base.WhenReloadEnd(launcher);
            _audioSource.Stop();
        }
    }
}
