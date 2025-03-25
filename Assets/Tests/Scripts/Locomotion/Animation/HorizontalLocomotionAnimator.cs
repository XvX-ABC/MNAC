using System;
using Locomotion.Animation;
using UnityEngine;
namespace Tests.Locomotion.Animation
{
    public class HorizontalLocomotionAnimator : MonoBehaviour, IModule
    {
        const float BottomHeight = 0.8f;
        IHorizontalLocomotionAnimationDefinitions _definition;
        IBonesDefinitions _bonesDefinition;
        Animator _animator;
        Context _context;
        void Awake()
        {
            var ldefinition = GetComponent<ILocomotionAnimationDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionAnimationDefinitions));
            _definition = ldefinition.Horizontal ?? throw new NullReferenceException(nameof(ldefinition.Horizontal));
            _bonesDefinition = GetComponent<IBonesDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IBonesDefinitions));
            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));
        }
        (Vector3, Quaternion) CalculateFootIKPosAndRotation(ushort legNum)
        {
            var goalIK = default(AvatarIKGoal);
            var bottomHeight = BottomHeight;
            if (legNum == 0)
                goalIK = AvatarIKGoal.LeftFoot;
            else if (legNum == 1)
                goalIK = AvatarIKGoal.RightFoot;
            else
                throw new InvalidLegNumException();
            var rotation = _animator.GetIKRotation(goalIK);
            var position = _animator.GetIKPosition(goalIK);
            var up = transform.up;
            if (Physics.Raycast(position, -up, out var hitInfo))
            {
                position = hitInfo.point + up * bottomHeight;
                _animator.SetIKPosition(goalIK, position);

                var normalRotation = Quaternion.FromToRotation(up, hitInfo.normal);
                //var finalRotation = normalRotation * rotation;
                var finalRotation = Quaternion.RotateTowards(rotation, normalRotation * rotation, 5);
                _animator.SetIKRotation(goalIK, finalRotation);

                _animator.SetIKRotationWeight(goalIK, 1);
                _animator.SetIKPositionWeight(goalIK, 1);
                return (position, finalRotation);
            }
            return default;
        }
        void UpdateFootIKOnGround()
        {
            if (_context == null)
                return;
            var ground = _context.Ground;
            if (ground==null)
                return;
            var legLength = _bonesDefinition.LegLength;
            var (newPosLeft, _) = CalculateFootIKPosAndRotation(0);
            var (newPosRight, _) = CalculateFootIKPosAndRotation(1);
            var (v_l, length_l) = CalculateVectorAndLength(0, newPosLeft, legLength);
            var (v_r, length_r) = CalculateVectorAndLength(1, newPosRight, legLength);

            if (length_l > 0.001f && length_r <= 0.001f && length_l < legLength / 2)
                UpdateBodyPosition(v_l, length_l);
            if (length_r > 0.001f && length_l <= 0.001f && length_r < legLength / 2)
                UpdateBodyPosition(v_r, length_r);

            (Vector3, float) CalculateVectorAndLength(int legNum, Vector3 footNewPos, float legLength)
            {
                var upperLegPos = Vector3.zero;
                if (legNum == 0)
                    upperLegPos = _animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
                else if (legNum == 1)
                    upperLegPos = _animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
                else
                    throw new InvalidLegNumException();
                var v = footNewPos - upperLegPos;
                var length = v.magnitude - legLength;
                return (v, length);
            }
            void UpdateBodyPosition(Vector3 towards, float length)
            {
                var tv = towards.normalized * length;
                var pv = Vector3.Project(tv, transform.up);
                _animator.bodyPosition += pv;
            }
        }
        void OnAnimatorIK(int layerIndex)
        {
            UpdateFootIKOnGround();
        }
        public void OnUpdate(Context context)
        {
            _context = context;
            var ground = context.Ground;
            if (ground == null)
                return;
            UpdateAnimation(context);
        }
        void UpdateAnimation(Context context)
        {
            var rotation = context.Rotation;
            var velocity = context.Velocity;
            var v = Quaternion.Inverse(context.Rotation) * velocity * 0.05f;
            _animator.SetFloat(_definition.XParamName, v.x);
            _animator.SetFloat(_definition.YParamName, v.z);
        }
    }
}