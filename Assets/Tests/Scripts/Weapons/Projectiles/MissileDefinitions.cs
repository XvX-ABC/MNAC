using UnityEngine;

namespace Tests.Weapons.Projectiles
{
    public class MissileDefinitions : MonoBehaviour, IMissileDefinitions
    {
        [SerializeField]
        float _angularSpeed;
        [SerializeField]
        float _maxAngle;
        [SerializeField]
        float _speed;

        public float AngularSpeed { get => _angularSpeed; }
        public float MaxAngle { get => _maxAngle; }
        public float Speed { get => _speed; }

    }
}