using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    internal abstract class SwordSlashEffect : MonoBehaviour
    {
        public abstract float Duration { get; set; }
        public abstract void Play();
        public abstract void Stop();
    }
}
