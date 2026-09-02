using UnityEngine;

namespace MNAC.Weapons.Projectiles
{
    internal abstract class BulletEffect : MonoBehaviour
    {
        public abstract void Play();
        public abstract void Stop();
    }
}
