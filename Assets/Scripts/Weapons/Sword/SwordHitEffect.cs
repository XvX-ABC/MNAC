using UnityEngine;

namespace MNAC.Weapons.Sword
{
    [RequireComponent(typeof(ParticleSystem))]
    internal class SwordHitEffect : SwordSlashEffect
    {
        ParticleSystem _particleSystem;
        ParticleSystem.MainModule _mainModule;
        Transform _parent;
        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _mainModule = _particleSystem.main;
            _mainModule.stopAction = ParticleSystemStopAction.Callback;
        }
        public override float Duration { get => _mainModule.duration; set => _mainModule.duration = value; }
        public Transform Parent { get => _parent; set => _parent = value; }
        private void OnParticleSystemStopped()
        {
            this.transform.SetParent(_parent);
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
        }
        public override void Play()
        {
            _particleSystem.Play();
        }

        public override void Stop()
        {
            _particleSystem.Stop();
            _particleSystem.Clear();
        }
    }
}
