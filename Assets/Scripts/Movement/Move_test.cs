using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Move_Test : MonoBehaviour
{
    [SerializeField]
    float _speed;
    [SerializeField]
    Camera _playerCamera;
    Rigidbody _rb;
    Vector3 _rbPosition;
    Vector3 _mousePosition;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        _rbPosition = _rb.position;
        _mousePosition = Input.mousePosition;
    }
    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.A))
            //this.transform.position += -Vector3.right * _speed * Time.deltaTime;
            _rb.MovePosition(_rb.position + -Vector3.right * _speed * Time.fixedDeltaTime);
        else if (Input.GetKey(KeyCode.D))
            _rb.MovePosition(_rb.position + Vector3.right * _speed * Time.fixedDeltaTime);
        Rotate();
        //this.transform.position += Vector3.right * _speed * Time.deltaTime;
    }
    void Rotate()
    {
        var pixel = _playerCamera.pixelRect;
        //var p0 = (Vector2)Input.mousePosition;
        //var p0 = new Vector2(pixel.max.x / 2, pixel.max.y);
        var p0 = (Vector2)_mousePosition;
        var p1 = (Vector2)_playerCamera.WorldToScreenPoint(_rbPosition);
        Debug.Log($"{p0}£¬{p1}, {_rb.position}");
        var dv = (p0 - p1).normalized;
        //dv.z = 0;
        var towards = new Vector3(dv.x, 0, dv.y);
        //var towards = -Vector3.forward;
        var ft = Quaternion.Inverse(_rb.rotation) * towards;
        Debug.DrawLine(_rb.position, _rb.position + towards * 4, Color.red);
        var targetRotation = Quaternion.LookRotation(ft, this.transform.up);
        _rb.MoveRotation(_rb.rotation * targetRotation);
    }
    void OnDrawGizmos()
    {
        if (_rb == null)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_rb.position, Vector3.one * 0.5f);
    }
}
