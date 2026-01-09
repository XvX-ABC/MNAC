using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GroundSampler : MonoBehaviour
{
    [SerializeField]
    LayerMask _groundMask;
    [SerializeField]
    float _length;
    [SerializeField]
    Vector3 _relativeDirection;
    [SerializeField]
    bool _onGround;
    Action<RaycastHit> _sampledAction;
    Vector3 _center;
    [SerializeField]
    Vector3 _origin;
    Rigidbody _rb;
    [SerializeField]
    float _originOffsetLength;
    public bool OnGround
    {
        get => _onGround;
    }
    public Action<RaycastHit> SampledAction { get => _sampledAction; set => _sampledAction += value; }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();


    }
    void Start()
    {
        var collider = GetComponent<CapsuleCollider>();
        _center = collider.center;

    }
    public void OnUpdate()
    {
        var velocity = _rb.velocity;
        velocity.y = 0;
        _origin = this.transform.position + _center;
        //_origin = this.transform.position + _center + velocity.normalized * _originOffsetLength;
       
        //_origin = this.transform.position + _center;
    }
    void FixedUpdate()
    {
        if (Physics.Raycast(new() { origin = _origin, direction = this.transform.rotation * _relativeDirection }, out var hitInfo, _length, _groundMask))
        {
            _onGround = true;
            _sampledAction?.Invoke(hitInfo);
        }
        else
            _onGround = false;
    }
    void OnDrawGizmos()
    {
        var pos = _origin;
        var rotation = this.transform.rotation;
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(pos, pos + (rotation * _relativeDirection) * _length);
        Gizmos.color = Color.white;
    }
}
