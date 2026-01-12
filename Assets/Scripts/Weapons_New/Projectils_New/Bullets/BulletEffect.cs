using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    internal abstract class BulletEffect : MonoBehaviour
    {
        public abstract void Play();
        public abstract void Stop();
    }
}
