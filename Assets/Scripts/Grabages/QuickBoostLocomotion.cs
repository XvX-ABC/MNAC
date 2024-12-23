using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Assets.Scripts.Locomotion;

namespace Assets.Scripts
{
    public class QuickBoostLocomotion : MonoBehaviour
    {
        [Serializable]
        public enum State
        {
            Ready,
            Boosting,
            InCD,
        }
        [SerializeField]
        float _cd;
        [SerializeField]
        float _duration;
        [SerializeField]
        float _power;
        [SerializeField]
        CustomEvent<Vector3, bool> _event;
        Context _context;
        [SerializeField]
        State _state;
        [SerializeField]
        float _cdTimer;
        [SerializeField]
        float _durationTimer;
        Vector3 _direction;
        BasicLocomotion _base;
        public bool InCD
        {
            get => _cd < _cdTimer;
        }
        internal Context Context
        {
            get => _context;
            set => _context = value;
        }
        internal CustomEvent<Vector3, bool> TriggerEvent
        {
            get => _event;
            set => _event = value;
        }
        internal BasicLocomotion Base
        {
            get => _base;
            set => _base = value;
        }
        void Start()
        {
            if (_context == null)
                throw new NullReferenceException("Context");
            if (_event == null)
                throw new NullReferenceException(nameof(TriggerEvent));
            if (_base == null)
                throw new NullReferenceException(nameof(Base));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Vector3 BoostMovement()
        {
            return _direction * _base.Speed * _power;
        }
        void UpdateDirectionByNormal()
        {
            var normal = _context.NormalOnGround;
            if (!Context.CheckNormalIsValid(normal))
                return;
            _direction = Vector3.ProjectOnPlane(_direction, normal);
        }
        void DrawDirection()
        {
            var pos = _context.Obj.transform.position;
            Debug.DrawLine(pos, pos + _direction * 10, Color.white);
        }
        public void OnUpdate()
        {
            if (_state == State.Ready && _event.Invoke(_context.Direction))
            {
                _state = State.Boosting;
                _direction = _context.Direction;
            }


            if (_state == State.Boosting)
            {
                if (_durationTimer < _duration)
                {
                    UpdateDirectionByNormal();
                    _context.Velocity = BoostMovement();
                    _durationTimer += Time.deltaTime;
                    DrawDirection();
                }
                else
                {
                    _state = State.InCD;
                    _cdTimer = 0;
                }
            }
            else if (_state == State.InCD)
            {
                if (_cdTimer < _cd)
                    _cdTimer += Time.deltaTime;
                else
                {
                    _state = State.Ready;
                    _durationTimer = 0;
                }
            }
        }
    }

}
