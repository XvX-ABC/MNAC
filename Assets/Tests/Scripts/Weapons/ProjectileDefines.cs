using UnityEngine;

namespace Tests.Weapons
{
    public class ProjectileDefines : MonoBehaviour, IProjectlieDefines
    {
        [SerializeField]
        float _speed;

        public float Speed { get => _speed; }
    }
}