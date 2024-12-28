using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tests.Arms
{
    public class ArmRotation : MonoBehaviour
    {
        [SerializeField]
        GameObject _shoulder;
        [SerializeField]
        GameObject _elbow;
        [SerializeField]
        GameObject _hand;
        [SerializeField]
        GameObject _target_0;
        [SerializeField]
        GameObject _target_1;
        GameObject _currentTarget;
        Vector3 _forward_c;
        Vector3 _forward_s;
        Vector3 _lowerArmVector;
        float _minLength;
        float _minAngle;

        void Start()
        {
            _forward_s = (_elbow.transform.position - _shoulder.transform.position).normalized;
            _forward_c = _forward_s;
            _minLength = (_shoulder.transform.position - _hand.transform.position).magnitude;
            CalculateNewForwardOfShoulder();
            //CalculateMinAngle();
            _currentTarget = _target_0;
        }

        void SwapTarget()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                _currentTarget=_currentTarget == _target_0 ? _target_1 : _target_0;
            }
        }
        void CalculateNewForwardOfShoulder()
        {
            var rotation = _elbow.transform.localRotation;
            _forward_s = rotation * _forward_c;
        }
        Quaternion CalculateRotation_Step_0(Vector3 towards)
        {
            var tv = towards.normalized;
            return Quaternion.FromToRotation(_forward_s, tv);
        }
        Quaternion CalculateRotation_Step_1(Vector3 towards)
        {
            var tv = towards.normalized;
            return Quaternion.FromToRotation(_forward_s, tv);
        }
        void Update()
        {
            SwapTarget();
            var towards = _currentTarget.transform.position - _shoulder.transform.position;
            var towards_1 = _currentTarget.transform.position - _elbow.transform.position;
            _lowerArmVector = _hand.transform.position - _elbow.transform.position;



            var rotation_1 = CalculateRotation_Step_1(towards_1);
            _shoulder.transform.rotation = rotation_1;
        }
        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            var pos = this.transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + _forward_s * 100);


            pos = _elbow.transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + _lowerArmVector.normalized * 100);
        }
    }

}
