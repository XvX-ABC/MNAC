using System;
using UnityEngine;

public class LegLengthGetter : MonoBehaviour
{
    Animator _animator;
    internal Transform upLegTrans;
    Transform _lowerLegTrans;
    Transform _footTrans;
    public float Length;
    public float SquareLength;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        upLegTrans = _animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        _lowerLegTrans = _animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        _footTrans = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        var v0 = _lowerLegTrans.position - _footTrans.position;
        var v1 = upLegTrans.position - _lowerLegTrans.position;
        Length = v0.magnitude + v1.magnitude;
        SquareLength = Mathf.Pow(Length, 2);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        Span<Vector3> posArr = stackalloc Vector3[]
        {
            upLegTrans.position,
            _lowerLegTrans.position,
            _footTrans.position,
        };
        Gizmos.color = Color.red;
        foreach (var pos in posArr)
            Gizmos.DrawCube(pos, Vector3.one * 0.1f);
    }
}
