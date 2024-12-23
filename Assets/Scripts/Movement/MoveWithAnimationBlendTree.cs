using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEditor.PlayerSettings;
public class MoveWithAnimationBlendTree : MonoBehaviour
{
    const float BottomHeight = 0.8f;
    Rigidbody _rb;
    Animator _animator;
    GroundSampler _groundSampler;
    CapsuleCollider _capsuleCollider;
    LegLengthGetter _legLengthGetter;
    struct FootInfo
    {
        internal HumanBodyBones bone;
        internal RaycastHit hitInfo;
        internal bool isHit;
        internal Vector3 pos;
    }
    [SerializeField]
    float _speed;
    [SerializeField]
    float _accelerationSpeed;
    [SerializeField]
    float _jumpHeight;
    [SerializeField]
    float _jumpDuration;
    [SerializeField]
    float _jumpTimer;
    [SerializeField]
    bool _jumping;
    float _jumpVelocity;
    [SerializeField]
    bool _inAir;
    [SerializeField]
    float _timer;

    [SerializeField]
    float _speedInAir;
    [SerializeField]
    float _ascendingSpeed;
    [SerializeField]
    bool _hasInput;
    [SerializeField]
    bool OnGround { get => _groundSampler.OnGround; }
    Vector3 _normal;
    bool _onGround;

    [SerializeField]
    Camera _playerCamera;
    [SerializeField]
    GameObject _target;
    Quaternion _targetRotation;

    Vector3 _rbPosition;
    Vector3 _mousePosition;
    Vector3 _rbVelocity;
    FootInfo[] _footInfoArray;

    [SerializeField]
    float _currentSpeed;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _groundSampler = GetComponent<GroundSampler>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _legLengthGetter = GetComponent<LegLengthGetter>();
        _footInfoArray = new FootInfo[]
        {
            new(){bone=HumanBodyBones.LeftFoot},
            new(){bone=HumanBodyBones.RightFoot}
        };
        _footInfoArray = new FootInfo[] {
            new (){bone=HumanBodyBones.LeftFoot},
            new (){bone=HumanBodyBones.RightFoot }
        };
    }
    void Update()
    {
        _mousePosition = Input.mousePosition;
        _rbPosition = _rb.position;
        _rbVelocity = _rb.velocity;
        _currentSpeed = _rbVelocity.magnitude;
    }
    void LateUpdate()
    {

    }
    void SampleFootInfo()
    {
        if (!OnGround) return;
        for (int i = 0; i < 2; i++)
        {
            ref var info = ref _footInfoArray[i];
            if (Physics.Raycast(info.pos, Vector3.down, out var hitInfo))
            {
                info.isHit = true;
                info.hitInfo = hitInfo;
            }
            else
                info.isHit = false;
        }
    }
    void FixedUpdate()
    {
        //SampleFootInfo();
        //_capsuleCollider.center = _newCenter;
        UpdateAnimation();
        SimpleNormal();
        var horizontalDirection = Vector3.zero;
        var verticalDirection = Vector3.zero;
        var position = _rb.position;
        //var horizontalDirection = Vector3.zero;
        _hasInput = false;
        //Debug.Log($"StartJump: {_startJump}, JumpTimer: {_jumpTimer}, JumpDuration: {_jumpDuration}, is greather: {_jumpTimer > _jumpDuration}");
        if (_jumping == true && (_jumpDuration > 0 && _jumpTimer > _jumpDuration) && OnGround)
        {
            Debug.Log("OnGround");
            _jumping = false;
        }
        if (OnGround)
        {
            _timer = 0;
            _inAir = false;
        }
        if (Input.GetKey(KeyCode.W))
        {
            horizontalDirection = Vector3.forward;
            _hasInput |= true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            horizontalDirection = -Vector3.forward;
            _hasInput |= true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            horizontalDirection += -Vector3.right;
            _hasInput |= true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontalDirection += Vector3.right;
            _hasInput |= true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            _hasInput |= true;
            if (OnGround)
            {
                _jumping = true;
            }
        }
        //verticalDirection += Vector3.up;
        //_hasInput |= true;
        //if (position.y < 1.5f)
        //{
        //    if (Input.GetKey(KeyCode.Space))
        //    {
        //        _startJump = true;
        //    }
        //    else
        //        _inAir = true;
        // 0=v0-gt
        // v0=-gt
        // v0/t=-g
        // -t=v0/-g
        //}
        //if (!_inAir)
        //    _timer = 0;
        var velocity = _rb.velocity;

        if (_hasInput)
        {
            if (_normal != Vector3.zero)
                horizontalDirection = Vector3.ProjectOnPlane(horizontalDirection, _normal);
            var finalSpeed = _speed;
            if (OnGround)
            {
                velocity = Vector3.MoveTowards(_rb.velocity, horizontalDirection.normalized * finalSpeed, _accelerationSpeed);
                if (_jumping)
                {
                    var v = Mathf.Sqrt(-2 * Physics.gravity.y * _jumpHeight);
                    velocity.y = v;
                    _jumpVelocity = v;
                    _jumpTimer = 0;
                    _jumpDuration = v / -Physics.gravity.y;
                }
            }
            if (_inAir)
            {
                if (horizontalDirection != Vector3.zero)
                {
                    var fvelocity = Vector3.MoveTowards(_rb.velocity, horizontalDirection.normalized * finalSpeed, _accelerationSpeed);
                    velocity.x = fvelocity.x;
                    velocity.z = fvelocity.z;
                }
                Debug.Log("In air, is on ground: " + OnGround);
                if (Input.GetKey(KeyCode.Space))
                {
                    Debug.Log("Ascending");
                    velocity.y = _ascendingSpeed;
                }
                //else
                //{
                //    Debug.Log("Downing");
                //    velocity.y = _ascendingSpeed + _timer * Physics.gravity.y;
                //    _timer += Time.fixedDeltaTime;
                //}
            }
        }

        if (_inAir && !Input.GetKey(KeyCode.Space))
        {
            velocity.y = _timer * Physics.gravity.y;
            _timer += Time.fixedDeltaTime;
        }
        if (!OnGround && _jumping)
        {
            // v^2=-2gh+v0^2/t^2
            if (_jumpTimer > 0)
            {
                Debug.Log("Updated jump velocity");
                var v = _jumpVelocity + Physics.gravity.y * _jumpTimer;
                velocity.y = v;
            }
        }
        if (_jumping)
            _jumpTimer += Time.fixedDeltaTime;
        Debug.DrawLine(position, position + velocity.normalized * 10, Color.green);
        _rb.velocity = velocity;
        Rotate();


    }
    void UpdateAnimation()
    {
        if (_animator == null)
            return;
        var direction = _rb.velocity / 10;
        var finalDirection = Quaternion.Inverse(this.transform.rotation) * direction;
        _animator.SetFloat("XValue", finalDirection.x);
        _animator.SetFloat("YValue", finalDirection.z);
    }

    Vector3 CalculateFootIKPos(ushort num)
    {
        var boneID = default(HumanBodyBones);
        var goalIK = default(AvatarIKGoal);
        var bottomHeight = BottomHeight;
        var dv = Vector3.zero;
        var currentPos = Vector3.zero;
        switch (num)
        {
            case 0:
                boneID = HumanBodyBones.LeftFoot;
                goalIK = AvatarIKGoal.LeftFoot;
                break;
            case 1:
                boneID = HumanBodyBones.RightFoot;
                goalIK = AvatarIKGoal.RightFoot;
                break;
        }
        var rotation = _animator.GetIKRotation(goalIK);
        var position = _animator.GetIKPosition(goalIK);
        //var info = _footInfoArray[num];
        //if (info.isHit)
        //{
        //    var hitInfo = info.hitInfo;
        //    //Debug.DrawLine(info.pos, hitInfo.point, Color.cyan);
        //    Debug.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal * bottomHeight);
        //    position = hitInfo.point + hitInfo.normal * bottomHeight;

        //    _animator.SetIKPosition(goalIK, position);

        //    var normalRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        //    _animator.SetIKRotation(goalIK, normalRotation * rotation);

        //    _animator.SetIKRotationWeight(goalIK, 1);
        //    _animator.SetIKPositionWeight(goalIK, 1);
        //    return position;
        //}
        if (Physics.Raycast(position, Vector3.down, out var hitInfo))
        {
            Debug.DrawLine(position, hitInfo.point, Color.cyan);
            position = hitInfo.point + this.transform.up * bottomHeight;
            //position = hitInfo.point + hitInfo.normal * bottomHeight;

            _animator.SetIKPosition(goalIK, position);

            var normalRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            _animator.SetIKRotation(goalIK, normalRotation * rotation);

            _animator.SetIKRotationWeight(goalIK, 1);
            _animator.SetIKPositionWeight(goalIK, 1);
            return position;

        }
        return Vector3.zero;
    }
    //private void OnAnimatorIK(int layerIndex)
    //{

    //    var newPosLeft = FeetIK(0);
    //    var newPosRight = FeetIK(1);

    //    //_currentIKPosOfLeftFoot = _animator.GetIKPosition(AvatarIKGoal.LeftFoot);
    //    //_currentIKPosOfRightFoot = _animator.GetIKPosition(AvatarIKGoal.RightFoot);
    //    _currentIKPosOfLeftFoot = newPosLeft;
    //    _currentIKPosOfRightFoot = newPosRight;
    //    _diffValueOfLeftFoot = _currentPosOfLeftFoot - _currentIKPosOfLeftFoot;
    //    _diffValueOfRightFoot = _currentPosOfRightFoot - _currentIKPosOfRightFoot;

    //    Debug.DrawLine(_hitPointLeftFoot, _currentPosOfLeftFoot, Color.yellow);


    //    //Debug.DrawLine(_hitPointRightFoot, _currentPosOfRightFoot, Color.yellow);
    //    Debug.DrawLine(_hitPointLeftFoot, _currentIKPosOfLeftFoot, Color.blue);
    //    Debug.DrawLine(_currentIKPosOfLeftFoot, _currentIKPosOfLeftFoot + _diffValueOfLeftFoot, Color.red);

    //    var legNum = 0;
    //    if (_currentPosOfLeftFoot.y < _currentPosOfRightFoot.y)
    //        legNum = 1;
    //    else if (_currentPosOfLeftFoot.y > _currentPosOfRightFoot.y)
    //        legNum = 2;

    //    //if (legNum == 0)
    //    //    return;

    //    var dv = legNum == 1 ? _diffValueOfLeftFoot : _diffValueOfRightFoot;
    //    if (dv.y >= _threshold)
    //    {
    //        var oldPosition = _animator.bodyPosition;
    //        _animator.bodyPosition += new Vector3(0, -dv.y, 0);
    //        Debug.Log($"num: {legNum}, bodyPosition: {_animator.bodyPosition}, dv.y: {dv.y}, oldPosition: {oldPosition}, diffValue: {_animator.bodyPosition - oldPosition}");
    //    }
    //    else if (dv.y < _threshold)
    //        Debug.Log($"num: {legNum}, The diff value less than threshold");
    //}
    //private void OnAnimatorIK(int layerIndex)
    //{

    //    var newPosLeft = FeetIK(0);
    //    var newPosRight = FeetIK(1);

    //}
    void OnAnimatorIK(int layerIndex)
    {
        _groundSampler.OnUpdate();
        if (!OnGround)
            return;

        var newPosLeft = CalculateFootIKPos(0);
        var newPosRight = CalculateFootIKPos(1);
        var (v_l, length_l) = CalculateVectorAndLength(0, newPosLeft);
        var (v_r, length_r) = CalculateVectorAndLength(1, newPosRight);
        //if (length_l > 0.1f && length_r > 0.1f)
        //    throw new Exception();
        if (length_l > 0.01f && length_r <= 0.01f && length_l < _legLengthGetter.Length / 2)
        {
            UpdateBodyPosition(v_l, length_l);
        }
        if (length_r > 0.01f && length_l <= 0.01f && length_r < _legLengthGetter.Length / 2)
            UpdateBodyPosition(v_r, length_r);

        (Vector3, float) CalculateVectorAndLength(int legNum, Vector3 newFootPos)
        {
            Vector3 upperLegPos;
            if (legNum == 0)
                upperLegPos = _animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
            else
                upperLegPos = _animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
            var v = (newFootPos - upperLegPos);
            var length = v.magnitude - _legLengthGetter.Length;
            return (v, length);
        }
        void UpdateBodyPosition(Vector3 towards, float length)
        {
            var tv = towards.normalized * length;
            var pv = Vector3.Project(tv, this.transform.up);
            _animator.bodyPosition += pv;
        }

    }
    void SimpleNormal()
    {
        if (Physics.Raycast(new() { origin = this.transform.position, direction = -this.transform.up }, out var hitInfo, Mathf.Infinity, (1 << LayerMask.NameToLayer("Terrain"))))
            _normal = hitInfo.normal;
        else
            _normal = Vector3.zero;
    }

    void DrawLine(Vector3 direction, float length, Color color)
    {
        Debug.DrawLine(this.transform.position, this.transform.position + direction * length, color);
    }
    void Rotate()
    {
        var p0 = (Vector2)_mousePosition;
        var p1 = (Vector2)_playerCamera.WorldToScreenPoint(_rbPosition);
        var dv = (p0 - p1).normalized;
        var towards = new Vector3(dv.x, 0, dv.y);
        DrawLine(towards.normalized, 10, Color.red);
        var ft = Quaternion.Inverse(_rb.rotation) * towards;
        Debug.DrawLine(_rb.position, _rb.position + towards * 4, Color.red);
        _targetRotation = Quaternion.LookRotation(ft, this.transform.up);
        _rb.MoveRotation(_rb.rotation * _targetRotation);
    }
    private void OnDrawGizmos()
    {
        if (_normal == Vector3.zero)
            return;
        //var pos = _rb.position;
        //Gizmos.DrawLine(pos, pos + _normal * 1);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawCube(pos + _normal * 1, Vector3.one * 0.3f);

        //var direction = _velocity.normalized;
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(pos, pos + direction * 7);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(_p0, Vector3.one * 0.5f);
        //Gizmos.DrawLine(_p0, _p0 + _r0 * Vector3.up);

        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(_p1 - Vector3.forward, Vector3.one * 0.5f);
        //Gizmos.DrawLine(_p1 - Vector3.forward, _p1 - Vector3.forward + _r1 * Vector3.up);
    }
}
