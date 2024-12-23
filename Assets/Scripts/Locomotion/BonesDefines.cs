using Locomotion.Animation;
using UnityEngine;

namespace Locomotion
{
    public class BonesDefines : MonoBehaviour, IBonesDefines
    {
        Animator _animator;
        Transform _upperLegTrans;
        Transform _lowerLegTrans;
        Transform _footTrans;
        float _legLength;
        public float LegLength => _legLength;
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _upperLegTrans = _animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            _lowerLegTrans = _animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            _footTrans = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            var v0 = _lowerLegTrans.position - _footTrans.position;
            var v1 = _upperLegTrans.position - _lowerLegTrans.position;
            _legLength = v0.magnitude + v1.magnitude;
        }
    }
}
