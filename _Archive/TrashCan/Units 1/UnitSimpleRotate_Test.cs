using System;
using UnityEngine;

public class UnitSimpleRotate_Test : MonoBehaviour
{
    [SerializeField]
    Camera _playerCamera;
    [SerializeField]
    GameObject _target;
    Quaternion _targetRotation;
    //Rigidbody _rb;
    void Awake()
    {
        //_rb = GetComponent<Rigidbody>();
    }
    void Update()
    {

        var p0 = _playerCamera.ScreenToViewportPoint(Input.mousePosition);
        var p1 = _playerCamera.WorldToViewportPoint(this.transform.position);
        var towards = (p0 - p1);
        var ft = Quaternion.Inverse(this.transform.rotation) * new Vector3(towards.x, 0, towards.y).normalized;
        _targetRotation = Quaternion.LookRotation(ft, this.transform.up);
        this.transform.rotation *= _targetRotation;
        Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 4, Color.red);
        //Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(towards.x, 0, towards.y).normalized * 4, Color.red);

        //if (Input.GetKey(KeyCode.A))
        //    this.transform.position += -Vector3.right * 55 * Time.deltaTime;
        //else if (Input.GetKey(KeyCode.D))
        //    this.transform.position += Vector3.right * 55 * Time.deltaTime;
        //Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 3, Color.blue);
    }
    void OnDrawGizmos()
    {
    }
}
