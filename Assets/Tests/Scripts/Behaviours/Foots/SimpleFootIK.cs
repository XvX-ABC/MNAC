using RootMotion.FinalIK;
using System;
using UnityEngine;

namespace Tests.Behaviours.Foots
{
    public class SimpleFootIK
    {
        LayerMask _layer;
        Vector3 _positionOffset;
        Vector3 _worldUpward;
        LegIK _ik;
        GameObject _footObj;

        float _weight;

        public SimpleFootIK(GameObject footObj, LegIK ik, LayerMask layer, Vector3 positionOffset, Vector3 worldUpward)
        {
            _footObj = footObj ?? throw new ArgumentNullException(nameof(footObj));
            _layer = layer;
            _positionOffset = positionOffset;
            _worldUpward = worldUpward;
            _ik = ik ?? throw new ArgumentNullException(nameof(ik));
        }

        public float Weight
        {
            get => _weight;
            set => _weight = Mathf.Clamp01(value);
        }
        public Vector3 WorldUpward { get => _worldUpward; set => _worldUpward = value; }
        public Vector3 PositionOffset { get => _positionOffset; set => _positionOffset = value; }
        public LayerMask Layer { get => _layer; set => _layer = value; }

        public void OnLateUpdate()
        {
            var pos = _footObj.transform.position;
            var ray = new Ray(pos, -_worldUpward);
            if (_weight != 0 && Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, (1 << _layer)))
            {
                _ik.solver.IKPositionWeight = _weight;
                _ik.solver.IKRotationWeight = _weight;
                _ik.solver.IKPosition = _positionOffset + hitInfo.point;
                _ik.solver.IKRotation = Quaternion.FromToRotation(_worldUpward, hitInfo.normal) * _footObj.transform.rotation;
            }
            else
            {
                _ik.solver.IKPositionWeight = 0;
                _ik.solver.IKRotationWeight = 0;
            }
        }
    }
}
