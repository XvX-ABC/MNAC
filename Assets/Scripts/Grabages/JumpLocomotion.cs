using System;
using UnityEngine;
using static Assets.Scripts.Locomotion;

namespace Assets.Scripts
{
    internal class JumpLocomotion : MonoBehaviour
    {
        [SerializeField]
        float _height;
        bool _jumping;
        [SerializeField]
        LayerMask _groundMask;
        [SerializeField]
        CustomEvent<bool> _triggerEvent;
        Context _context;
        public bool Jumping
        {
            get => _jumping;
        }
        public float Height
        {
            get => _height;
        }
        public Context Context
        {
            get => _context;
            set => _context = value;
        }
        public CustomEvent<bool> TriggerEvent
        {
            get => _triggerEvent;
            set => _triggerEvent = value;
        }
        void Start()
        {
            if (_context == null)
                throw new NullReferenceException(nameof(Context));
            if(_triggerEvent==null)
                throw new NullReferenceException(nameof(TriggerEvent));
        }
        internal bool CheckIsGround(Collision collision)
        {
            var obj = collision.gameObject;
            var layer = obj.layer;
            if (layer == 0)
                return layer == _groundMask.value >> 1;
            else
                return (_groundMask.value >> 1 & layer) > 0;
        }
        internal void UpdateJumpState(Collision collision, bool isJumping)
        {
            if (CheckIsGround(collision))
                _jumping = isJumping;
        }
        void OnCollisionEnter(Collision collision)
        {
            UpdateJumpState(collision, false);
        }
        void OnCollisionExit(Collision collision)
        {
            UpdateJumpState(collision, true);
        }
        internal void OnUpdate()
        {
            if (!_jumping && _triggerEvent.Invoke())
            {
                var trans = _context.Obj.transform;
                _context.Velocity.y = (trans.up * Mathf.Sqrt(-2 * Physics.gravity.y * _height)).y;
            }
        }
    }

}
