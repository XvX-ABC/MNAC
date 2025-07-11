using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    [RequireComponent(typeof(Animator))]
    public class ArmRotation_Obsolete : MonoBehaviour
    {
        [SerializeField]
        GameObject _pointObj;
        [SerializeField]
        GameObject _upperArmObj;
        [SerializeField]
        GameObject _lowerArmObj;
        [SerializeField]
        GameObject _handObj;


        [SerializeField]
        [Range(0, 1)]
        float _weight;
        [SerializeField]
        GameObject _target;
        [SerializeField]
        GameObject _elbowObj;

        Animator _animator;
        AvatarIKGoal _goal;


        Vector3 _v_uh;
        Vector3 _v_lh;

        Vector3 _v_lt;


        [SerializeField]
        Vector3 _v;
        [SerializeField]
        Vector3 _fv;
        [SerializeField]
        Vector3 _fpos;
        private void Awake()
        {
            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));
            _goal = AvatarIKGoal.RightHand;
            _v = (this._upperArmObj.transform.position - _pointObj.transform.position).magnitude * Vector3.forward;
            _fpos = _upperArmObj.transform.position + _v;

            _v_uh = _v;
        }
        private void Update()
        {

            if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
            {
                var a = _animator.GetBool("A");
                _animator.SetBool("A", !a);
            }
            //s1();
            s2();

        }

        void s1()
        {
            _v_uh = _handObj.transform.position - _upperArmObj.transform.position;
            _v_lt = _target.transform.position - _lowerArmObj.transform.position;
            _v_lh = _handObj.transform.position - _lowerArmObj.transform.position;

            var tr = Quaternion.FromToRotation(_v_lh, _v_lt);

            _fv = tr * _v_uh.normalized * _v.magnitude;

            _fpos = _upperArmObj.transform.position + _fv;
        }

        void s2()
        {


            _v_uh = _handObj.transform.position - _upperArmObj.transform.position;
            _v_lt = _target.transform.position - _handObj.transform.position;
            _v_lh = _handObj.transform.position - _lowerArmObj.transform.position;

            var tr = Quaternion.FromToRotation(_v_lh, _v_lt);

            _fv = tr * _v_uh.normalized * _v.magnitude;

            _fpos = _upperArmObj.transform.position + _fv;
        }

        private void OnAnimatorIK(int layerIndex)
        {




            var pos = _upperArmObj.transform.position;


            _animator.SetIKPosition(_goal, _fpos);
            _animator.SetIKPositionWeight(_goal, _weight);

            var epos = _elbowObj.transform.position;
            _animator.SetIKHintPosition(AvatarIKHint.RightElbow, epos);
            _animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);


        }
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            var pos = _upperArmObj.transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pos, pos + _fv);
            Gizmos.DrawSphere(pos + _fv, 0.1f);


            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(_upperArmObj.transform.position, _lowerArmObj.transform.position);
            Gizmos.DrawLine(_lowerArmObj.transform.position, _handObj.transform.position);
            var v = _v_lh.normalized;
            Gizmos.DrawLine(_lowerArmObj.transform.position, _lowerArmObj.transform.position + v * 100);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(_handObj.transform.position, _handObj.transform.position + _v_lt);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(_upperArmObj.transform.position, _target.transform.position);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(_upperArmObj.transform.position, _lowerArmObj.transform.position);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_lowerArmObj.transform.position, _handObj.transform.position);


        }
    }
}
