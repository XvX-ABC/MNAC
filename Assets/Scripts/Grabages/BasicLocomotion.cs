using System;
using UnityEngine;
using static Assets.Scripts.Locomotion;

namespace Assets.Scripts
{
    internal class BasicLocomotion : MonoBehaviour
    {
        [SerializeField]
        float _speed;
        [SerializeField]
        CustomEvent<Context, Vector3> _triggerEvent;
        Context _context;
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }
        public Context Context
        {
            get => _context;
            set => _context = value;
        }
        public CustomEvent<Context, Vector3> TriggerEvent
        {
            get => _triggerEvent;
            set => _triggerEvent = value;
        }
        void Start()
        {
            if (_context == null)
                throw new NullReferenceException(nameof(Context));
            if (_triggerEvent == null)
                throw new NullReferenceException(nameof(TriggerEvent));
        }
        internal void OnUpdate()
        {
            var direction = _triggerEvent.Invoke(_context);
            if (direction == Vector3.zero)
                return;
            _context.Direction = direction;
            var velocity = _context.Velocity;
            if (direction.x != 0)
                velocity.x = direction.x * _speed;
            if (direction.z != 0)
                velocity.z = direction.z * _speed;

            _context.Velocity = velocity;
        }
    }
}
