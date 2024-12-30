using Assets.Scripts;
using NUnit.Framework.Constraints;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Locomotion.Animation
{
    public interface IBonesDefines
    {
        public float LegLength { get; }
    }
    [RequireComponent(typeof(Animator))]
    public class LocomotionAnimator : MonoBehaviour, ILocomotionAnimator
    {
        const float BottomHeight = 0.8f;
        class HorizontalLocomotionAnimatorDefines
        {
            public string XParamName;
            public string YParamName;
        }
        [Serializable]
        class GroundLocomotionAnimationDefines : HorizontalLocomotionAnimatorDefines
        {
        }
        [Serializable]
        class InAirLocomotionAnimatorDefines : HorizontalLocomotionAnimatorDefines
        {
            public string EnterParamName;
        }
        [Serializable]
        class JumpAnimatorDefines
        {
            public float AscendingClipLength;
            public string AscendingMultiplierName;
            public float LandingClipLength;
            public string LandingMultiplierName;
            public string LandingValueParamName;
            public string DescendingClipName;
            public float DescendingClipLength;
            public string EnterParamName;
        }
        class InvalidLegNumException : Exception
        {
            public InvalidLegNumException(string message) : base(message)
            {

            }
        }
        class JumpAnimator
        {
            public float ScaleFactor;
            LocomotionControlBase.JumpLocomotion _locomotion;
            JumpAnimatorDefines _animatorDefines;
            IJumpDefines _defines;
            IGroundSampler _sampler;
            Animator _animator;
            Transform _trans;
            float _maxHeight;


            public JumpAnimator(LocomotionControlBase.JumpLocomotion locomotion, JumpAnimatorDefines animatorDefines, IJumpDefines defines, Animator animator, Transform trans, IGroundSampler sampler)
            {
                _locomotion = locomotion;
                _animatorDefines = animatorDefines;
                _animator = animator;
                _defines = defines;
                _trans = trans;
                _sampler = sampler;
                _locomotion.RegisterStartAction(locomotion => UpdateMaxHeight());
                UpdateAscendingMultiplier();
                UpdateLandingMultiplier();
            }

            float CalculateAscendingMultiplier()
            {
                var clipLength = _animatorDefines.AscendingClipLength;
                var length = _locomotion.AscendingDuration;
                return clipLength / length;
            }
            void UpdateAscendingMultiplier(float multiplier)
            {
                _animator.SetFloat(_animatorDefines.AscendingMultiplierName, multiplier);
            }
            public void UpdateAscendingMultiplier()
            {
                UpdateAscendingMultiplier(CalculateAscendingMultiplier());
            }
            float CalculateLandingMultiplier()
            {
                var clipLength = _animatorDefines.LandingClipLength;
                var length = _defines.LandingDuration;
                return clipLength / length;
            }
            void UpdateLandingMultiplier(float multiplier)
            {
                _animator.SetFloat(_animatorDefines.LandingMultiplierName, multiplier);
            }
            public void UpdateLandingMultiplier()
            {
                UpdateLandingMultiplier(CalculateLandingMultiplier());
            }
            void UpdateMaxHeight()
            {
                var worldY = _trans.position.y;
                _maxHeight = worldY + _defines.Height;
            }
            void UpdateInLanding()
            {
                var point = _sampler.Point;
                Debug.DrawLine(point, point + Vector3.right * 3, Color.green);
                var groundWorldHeight = point.y;
                var maxWorldHeight = _maxHeight;
                var currentHeight = _sampler.CurrentHeight;
                var v = currentHeight / (maxWorldHeight - groundWorldHeight);
                //Debug.Log("dv: " + (maxWorldHeight - groundWorldHeight) + ", groundWorldHeight: " + groundWorldHeight + ", " + maxWorldHeight + ", point: " + point + ", currentheight: " + currentHeight + ", v: " + v);
                //_animator.SetFloat(_animatorDefines.LandingValueParamName, 1f - v);
                _animator.Play(_animatorDefines.DescendingClipName, 0, Mathf.Clamp01(1 - v));
            }
            public void OnFixedUpdate()
            {
                _animator.SetBool(_animatorDefines.EnterParamName, _locomotion.InJumping);
                if (_locomotion.StepNum == 3)
                {
                    UpdateInLanding();
                }
            }
        }
        Animator _animator;
        [SerializeField]
        GroundLocomotionAnimationDefines _groundLocomotionDefines;
        [SerializeField]
        InAirLocomotionAnimatorDefines _inAirLocomotionAnimatorDefines;
        [SerializeField]
        JumpAnimatorDefines _jumpAnimatorDefines;
        JumpAnimator _jumpAnimator;
        IGroundSampler _groundSampler;
        IBonesDefines _bonesDefines;
        LocomotionControlBase.JumpLocomotion _jumpLocomotion;
        protected bool _isOnGround
        {
            get => _groundSampler.IsOnGround;
        }
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _bonesDefines = GetComponent<IBonesDefines>();
            _groundSampler = GetComponent<IGroundSampler>();
        }
        void Start()
        {
            //var locomotionControl = GetComponent<LocomotionControlBase>();
            //_jumpLocomotion = locomotionControl.jumpLocomotion;
            //_jumpAnimator = new(_jumpLocomotion, _jumpAnimatorDefines, locomotionControl.locomotionDefines.Jump, _animator, this.transform, _groundSampler);


            //_jumpAnimator.UpdateAscendingMultiplier();
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
                throw new InvalidLegNumException("The leg num can only be 0 or 1");
            var rotation = _animator.GetIKRotation(goalIK);
            var position = _animator.GetIKPosition(goalIK);
            var up = transform.up;
            if (Physics.Raycast(position, -up, out var hitInfo))
            {
                position = hitInfo.point + up * bottomHeight;
                _animator.SetIKPosition(goalIK, position);

                var normalRotation = Quaternion.FromToRotation(up, hitInfo.normal);
                var finalRotation = normalRotation * rotation;
                _animator.SetIKRotation(goalIK, finalRotation);

                _animator.SetIKRotationWeight(goalIK, 1);
                _animator.SetIKPositionWeight(goalIK, 1);
                return (position, finalRotation);
            }
            return default;
        }
        void UpdateFootIKOnGround()
        {
            if (!_isOnGround)
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
                    throw new InvalidLegNumException("The leg num can only be 0 or 1");
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
        private void OnAnimatorIK(int layerIndex)
        {
            UpdateFootIKOnGround();
        }
        public void OnFixedUpdate(LocomotionContext context)
        {
            var velocity = context.Velocity;
            //if (!_isOnGround && !_jumpLocomotion.InJumping)
            if (!_isOnGround)
            {
                _animator.SetBool(_inAirLocomotionAnimatorDefines.EnterParamName, true);
                UpdateAnimationInAir(velocity);
            }
            else if (_isOnGround)
            {
                _animator.SetBool(_inAirLocomotionAnimatorDefines.EnterParamName, false);
                UpdateAnimationOnGround(velocity);
            }
            //UpdateAnimationInJump();
            //_jumpAnimator.OnFixedUpdate();
        }
        void UpdateAnimationOfHorizontalMovement(Vector3 currentVelocity, string paramName_x, string paramName_y)
        {
            var v = Quaternion.Inverse(transform.rotation) * currentVelocity * 0.05f;
            _animator.SetFloat(paramName_x, v.x);
            _animator.SetFloat(paramName_y, v.z);
        }
        public void UpdateAnimationOnGround(Vector3 currentVelocity)
        {
            UpdateAnimationOfHorizontalMovement(currentVelocity, _groundLocomotionDefines.XParamName, _groundLocomotionDefines.YParamName);
        }
        public void UpdateAnimationInAir(Vector3 currentVelocity)
        {
            UpdateAnimationOfHorizontalMovement(currentVelocity, _inAirLocomotionAnimatorDefines.XParamName, _inAirLocomotionAnimatorDefines.YParamName);
        }
        public void UpdateAnimationInJump()
        {
            var inJumping = _jumpLocomotion.InJumping;
            _animator.SetBool(_jumpAnimatorDefines.EnterParamName, inJumping);
        }
    }
}
