using UnityEngine;

namespace Tests.Weapons
{
    public class ProjectileDefinitions : MonoBehaviour, IProjectileDefinitions
    {
        [SerializeField]
        float _speed;

        public float Speed { get => _speed; }
    }
}