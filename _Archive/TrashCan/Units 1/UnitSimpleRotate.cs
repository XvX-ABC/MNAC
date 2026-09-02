using System;
using UnityEngine;

public class UnitSimpleRotate : MonoBehaviour
{
    [SerializeField]
    Camera _playerCamera;
    [SerializeField]
    GameObject _target;
    Quaternion _targetRotation;
    Rigidbody _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {

        //var p0 = _playerCamera.ScreenToViewportPoint(Input.mousePosition);
        //var p1 = _playerCamera.WorldToViewportPoint(_rb.position);
        //var towards = (p0 - p1);
        //var ft = Quaternion.Inverse(_rb.rotation) * new Vector3(towards.x, 0, towards.y).normalized;
        //Debug.DrawLine(_rb.position, _rb.position + new Vector3(towards.x, 0, towards.y).normalized * 4, Color.red);
        //_targetRotation = Quaternion.LookRotation(ft, this.transform.up);
        ////_rb.rotation *= _targetRotation;
        //_rb.MoveRotation(_rb.rotation * _targetRotation);
        //this.transform.rotation *= _targetRotation;
    }
    void DrawForward()
    {
        Debug.DrawLine(_rb.position, _rb.position + this.transform.forward * 3, Color.green);
    }
    void OnDrawGizmos()
    {
    }
}
