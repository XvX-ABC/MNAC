using UnityEngine;

namespace Tests.Weapons.Projectiles
{
    public class MissileDefines : MonoBehaviour, IMissileDefines
    {
        [SerializeField]
        float _angularSpeed;
        [SerializeField]
        float _maxAngle;
        [SerializeField]
        float _speed;
        [SerializeField]
        float _angularAngle;

        public float AngularSpeed { get => _angularSpeed; }
        public float MaxAngle { get => _maxAngle; }
        public float Speed { get => _speed; }

        public float AngularAngle { get => _angularAngle; }
    }
}