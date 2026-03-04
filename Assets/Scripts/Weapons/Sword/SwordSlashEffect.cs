using UnityEngine;

namespace MNAC.Weapons.Sword
{
    internal abstract class SwordSlashEffect : MonoBehaviour
    {
        public abstract float Duration { get; set; }
        public abstract void Play();
        public abstract void Stop();
    }
}
