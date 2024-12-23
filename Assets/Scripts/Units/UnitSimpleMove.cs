using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class JetMovement
{
    [SerializeField]
    float _duration;
    [SerializeField]
    float _interval;
    [SerializeField]
    float _power;
    float _timer;
    byte _state;
    Action _timersUpdater;
    public void Initialize()
    {
    }
    public Vector3 GetForce(Vector3 direction)
    {
        if (_state == 0)
        {
            _state = 1;
            _timersUpdater += UpdateDuration;
        }

        if (_state == 1)
            return direction * _power;

        if (_state == 2)
        {
            _timersUpdater += UpdateInterval;
            return direction;
        }

        return Vector3.zero;

        void UpdateDuration()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration)
            {
                _state = 2;
                _timersUpdater -= UpdateDuration;
            }
        }
        void UpdateInterval()
        {
            _timer += Time.deltaTime;
            if (_timer >= _interval)
            {
                _state = 0;
                _timersUpdater -= UpdateInterval;
            }
        }
    }
    public void OnUpdate()
    {
        _timersUpdater?.Invoke();
    }
}
public class UnitSimpleMove : MonoBehaviour
{
    [SerializeField]
    float _speed;
    [SerializeField]
    float _power;
    [SerializeField]
    float _jetInterval;
    [SerializeField]
    float _jetDuration;
    [SerializeField]
    float _jetIntervalTimer;
    [SerializeField]
    float _jetDurationTimer;
    [SerializeField]
    ushort _jetState;
    [SerializeField]
    float _jumpTimer;
    [SerializeField]
    float _jumpHeight;
    bool _jetMoving;
    Rigidbody _body;
    Vector3 _normal;
    [SerializeField]
    bool _onGround;
    Vector3 _oldVelocity;
    float _timer;
    void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _body.rotation = Quaternion.LookRotation(Vector3.forward);
        _body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    private void OnCollisionStay(Collision collision)
    {
        _onGround = true;
    }
    void OnCollisionExit(Collision collision)
    {
        _onGround = false;
    }
    Vector3 BoostMovement(Vector3 direction)
    {
        return direction * _speed * 8;
    }
    void FixedUpdate()
    {
        if (Physics.Raycast(new() { origin = this.transform.position, direction = -this.transform.up }, out var hitInfo, 1f))
        {
            _normal = hitInfo.normal;
        }
        else
        {
            //_normal = Vector3.positiveInfinity;
            _normal = Vector3.up;
        }
        var direction = Vector3.zero;
        var velocity = _body.velocity;
        if (Input.GetKey(KeyCode.W))
        {
            direction = GetDirectionOnPlane(this.transform.forward);
            velocity.z = GetDirectionOnPlane(this.transform.forward).z * _speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction = GetDirectionOnPlane(-this.transform.forward);
            velocity.z = GetDirectionOnPlane(-this.transform.forward).z * _speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += GetDirectionOnPlane(-this.transform.right);
            velocity.x = GetDirectionOnPlane(-this.transform.right).x * _speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction += GetDirectionOnPlane(this.transform.right);
            velocity.x = GetDirectionOnPlane(this.transform.right).x * _speed;
        }
        if (Input.GetKey(KeyCode.Space) && _onGround)
        {
            velocity.y = (this.transform.up * Mathf.Sqrt(-2 * Physics.gravity.y * _jumpHeight)).y;
        }
        if (direction != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
            velocity = BoostMovement(direction);
        _body.velocity = velocity;

        DrawDirection(velocity);
        Vector3 GetDirectionOnPlane(Vector3 originalDirection)
        {
            if (_normal == Vector3.up)
                return originalDirection;
            if (Vector3.Distance(_normal, Vector3.positiveInfinity) > 10)
                return Vector3.ProjectOnPlane(originalDirection, _normal);
            return Vector3.zero;
        }
        void DrawDirection(Vector3 direction)
        {
#if UNITY_EDITOR
            var pos = this.transform.position;
            Debug.DrawLine(pos, pos + direction * 10, Color.white);
#endif
        }
    }
}
