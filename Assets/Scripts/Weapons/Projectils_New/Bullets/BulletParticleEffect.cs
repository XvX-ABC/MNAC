using UnityEngine;

namespace MNAC.Weapons.Projectiles
{
    [RequireComponent(typeof(ParticleSystem))]
    internal class BulletParticleEffect : BulletEffect
    {
        protected new ParticleSystem particleSystem;
        protected virtual void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }
        public override void Play()
        {
            if (particleSystem == null)
                particleSystem = GetComponent<ParticleSystem>();
            Stop();
            particleSystem.Play();
        }
        public override void Stop()
        {
            particleSystem.Stop();
            particleSystem.Clear();
        }
    }
}
