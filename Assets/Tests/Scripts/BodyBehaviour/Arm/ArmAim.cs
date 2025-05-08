using System;
using System.Diagnostics;
using Tests.Input;
using Tests.Weapons;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Tests.BodyBehaviour.Arm
{
    [Serializable]
    internal class ArmAim
    {

        [SerializeField]
        float _pointDistance;
        [SerializeField]
        GameObject _upperArmObj;
        [SerializeField]
        GameObject _lowerArmObj;
        [SerializeField]
        GameObject _handObj;
        [SerializeField]
        GameObject _elbowObj;

        [Range(0f, 1f)]
        [SerializeField]
        float _weight;
        ITarget _target;

        [SerializeField]
        Animator _animator;
        [SerializeField]
        AvatarIKGoal _goal;
        [SerializeField]
        AvatarIKHint _hint;


        Vector3 _handIKPos;

        bool _enabled;
        public float Weight { get => _weight; set => _weight = value; }
        public bool Enabled { get => _enabled; set => _enabled = value; }
        public ITarget Target
        {
            get => _target;
            set
            {
                if (value == null)
                    _enabled = false;
                else
                    _enabled = true;
                _target = value;
            }
        }
        public bool Continuing => _enabled;


        internal void OnAwake()
        {
            if (_goal != AvatarIKGoal.LeftHand && _goal != AvatarIKGoal.RightHand)
            {
                throw new Exception("The ik goal must be a part of hands");
            }
            if (_hint != AvatarIKHint.LeftElbow && _hint != AvatarIKHint.RightElbow)
                throw new Exception("The ik hint must be a part of elbows");
        }
        public void OnUpdate()
        {
            if (!_enabled)
                return;
            var tpos = _target.Position;
            var upos = _upperArmObj.transform.position;
            var lpos = _lowerArmObj.transform.position;
            var hpos = _handObj.transform.position;

            var vuh = hpos - upos;
            var vht = tpos - hpos;
            var vhl = hpos - lpos;

            var r = Quaternion.FromToRotation(vhl, vht);
            var lv = r * vuh.normalized * _pointDistance;
            _handIKPos = upos + lv;

        }
        public void OnAnimatorIK(int layerIndex)
        {
            if (!_enabled)
                return;
            _animator.SetIKHintPosition(_hint, _elbowObj.transform.position);
            _animator.SetIKHintPositionWeight(_hint, _weight);

            _animator.SetIKPosition(_goal, _handIKPos);
            _animator.SetIKPositionWeight(_goal, _weight);
        }
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying || !_enabled)
                return;
            var pos = _lowerArmObj.transform.position;
            var v = _handObj.transform.position - _lowerArmObj.transform.position;
            Gizmos.DrawLine(pos, pos + v.normalized * 10);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_target.Position, 2);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_handIKPos, 1);
        }

        public bool Begin()
        {
            _enabled = true;
            return true;
        }

        public bool End()
        {
            _enabled = false;
            return true;
        }
    }
}
