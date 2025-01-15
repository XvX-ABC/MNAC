using System;
using Locomotion.Animation;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
namespace Tests.Locomotion.Animation
{
    public class HorizontalLocomotionAnimator : MonoBehaviour, IModule
    {
        const float BottomHeight = 0.8f;
        IHorizontalLocomotionAnimationDefines _defines;
        IBonesDefines _bonesDefines;
        Animator _animator;
        Context _context;
        void Awake()
        {
            var ldefines = GetComponent<ILocomotionAnimationDefines>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(ILocomotionAnimationDefines));
            _defines = ldefines.Horizontal ?? throw new NullReferenceException(nameof(ldefines.Horizontal));
            _bonesDefines = GetComponent<IBonesDefines>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(IBonesDefines));
            _animator = GetComponent<Animator>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(Animator));
        }
        (Vector3, Quaternion) UpdateFootIKPosAndRotation(ushort legNum)
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
            if (!ground.Touched)
                return;
            var legLength = _bonesDefines.LegLength;
            var (newPosLeft, _) = UpdateFootIKPosAndRotation(0);
            var (newPosRight, _) = UpdateFootIKPosAndRotation(1);
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
            if (!ground.Touched)
                return;
            UpdateAnimation(context);
        }
        void UpdateAnimation(Context context)
        {
            var rotation = context.Rotation;
            var velocity = context.Velocity;
            var v = Quaternion.Inverse(context.Rotation) * velocity * 0.05f;
            _animator.SetFloat(_defines.XParamName, v.x);
            _animator.SetFloat(_defines.YParamName, v.z);
        }
    }
}