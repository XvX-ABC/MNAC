using UnityEngine;

namespace Tests.Weapons.Projectiles_New
{
    internal class BulletHitParticleEffect : BulletParticleEffect
    {
        Transform _parent;

        public Transform Parent { get => _parent; set => _parent = value; }

        protected override void Awake()
        {
            base.Awake();
            var main = particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
        protected virtual void OnParticleSystemStopped()
        {
            Stop();
        }
        public override void Play()
        {
            base.Play();
            this.transform.SetParent(null);
        }
        public override void Stop()
        {
            base.Stop();
            Debug.Log($"stop parent: {_parent.name}, {_parent.gameObject.activeSelf}");
            this.transform.SetParent(_parent);
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
        }
    }
}
