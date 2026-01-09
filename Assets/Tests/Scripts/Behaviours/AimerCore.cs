using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Tests.Behaviours
{

    //[Obsolete]
    //[RequireComponent(typeof(Camera))]
    //public class AimerCore : MonoBehaviour
    //{
    //    Camera _camera;
    //    [SerializeField]
    //    LayerMask _targetMasks;
    //    [SerializeField]
    //    GameObject _focusTarget;
    //    [SerializeField]
    //    [Range(0, 10f)]
    //    float _smoothTime;
    //    [SerializeField]
    //    GameObject[] _aimerObjs;



    //    Vector3 _oldPos;
    //    Vector3 _expectedPos;
    //    Vector3 _currentVelocity;
    //    IAimer_Obsolete[] _aimers;
    //    HitTarget _target;
    //    private void Awake()
    //    {
    //        _camera = GetComponent<Camera>();
    //        _target = new();
    //    }
    //    private void Start()
    //    {
    //        _oldPos = _focusTarget.transform.position;
    //        _expectedPos = transform.position;
    //        transform.LookAt(_focusTarget.transform);

    //        var list = new List<IAimer_Obsolete>();
    //        foreach (var obj in _aimerObjs)
    //        {
    //            if (obj == null)
    //                continue;
    //            if (obj.TryGetComponent<IAimer_Obsolete>(out var aimer))
    //            {
    //                aimer.Target = _target;
    //                list.Add(aimer);
    //            }
    //        }
    //        _aimers = list.ToArray();
    //    }
    //    private void Update()
    //    {

    //    }
    //    void FixedUpdate()
    //    {
    //        var ray = _camera.ScreenPointToRay(UInput.mousePosition);
    //        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _targetMasks))
    //        {
    //            _target.hitInfo = hitInfo;
    //        }
    //    }
    //    public void Register(IAimer_Obsolete aimer)
    //    {
    //        if (aimer == null)
    //            throw new ArgumentNullException(nameof(aimer));
    //        if (_aimers == null)
    //        {
    //            _aimers = new IAimer_Obsolete[] { aimer };
    //        }
    //        else
    //        {
    //            Array.Resize(ref _aimers, _aimers.Length + 1);
    //            _aimers[^1] = aimer;
    //        }
    //        aimer.Target = _target;
    //    }
    //    public void Unregister(IAimer_Obsolete aimer)
    //    {
    //        if (aimer == null)
    //            throw new ArgumentNullException(nameof(aimer));
    //        if (_aimers.Length == 1)
    //            _aimers = null;
    //        else
    //        {
    //            var index = -1;
    //            for (var i = 0; i < _aimers.Length; i++)
    //            {
    //                var a = _aimers[i];
    //                if (a == aimer)
    //                    index = i;
    //            }


    //            if (index == -1)
    //                return;


    //            if (index <= _aimers.Length - 1)
    //            {
    //                Array.Copy(_aimers, index, _aimers, index - 1, _aimers.Length - 1 - index);
    //            }
    //            Array.Resize(ref _aimers, _aimers.Length - 1);

    //        }
    //        aimer.Target = null;
    //    }
    //}
}
