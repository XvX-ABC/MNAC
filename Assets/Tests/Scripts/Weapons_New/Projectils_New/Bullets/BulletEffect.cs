using UnityEngine;

namespace Tests.Weapons.Projectiles_New
{
    internal abstract class BulletEffect : MonoBehaviour
    {
        public abstract void Play();
        public abstract void Stop();
    }
}
