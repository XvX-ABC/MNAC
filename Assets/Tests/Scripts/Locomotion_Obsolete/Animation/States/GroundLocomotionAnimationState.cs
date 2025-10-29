using Assets.Scripts.Utilities;
using Locomotion.Animation;
using System;
using Tests.Weapons.MultiMissileLauncher.Animation;
using UnityEngine;
namespace Tests.Locomotion_Obsolete.Animation.States
{
    public class GroundLocomotionAnimationState : AnimationStateBase
    {
        const float BottomHeight = 0.8f;
        IGroundLocomotionAnimatorDefinitions _definitions;
        IBonesDefinitions _bonesDefinitions;
        public GroundLocomotionAnimationState(Animator animator, IGroundLocomotionAnimatorDefinitions definitions, IBonesDefinitions bonesDefinitions) : base("grounded", animator)
        {
            _bonesDefinitions = bonesDefinitions ?? throw new ArgumentNullException(nameof(bonesDefinitions));
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
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
            var rotation = animator.GetIKRotation(goalIK);
            var position = animator.GetIKPosition(goalIK);
            var up = context.Transform.up;
            if (Physics.Raycast(position, -up, out var hitInfo))
            {
                position = hitInfo.point + up * bottomHeight;
                animator.SetIKPosition(goalIK, position);

                var normalRotation = Quaternion.FromToRotation(up, hitInfo.normal);
                //var finalRotation = normalRotation * rotation;
                var finalRotation = Quaternion.RotateTowards(rotation, normalRotation * rotation, 5);
                animator.SetIKRotation(goalIK, finalRotation);

                animator.SetIKRotationWeight(goalIK, 1);
                animator.SetIKPositionWeight(goalIK, 1);
                return (position, finalRotation);
            }
            return default;
        }
        void UpdateFootIKOnGround()
        {
            if (context == null)
                return;
            var ground = context.Ground;
            if (ground == null)
                return;
            //if (_jumpLocomotion.CurrentState > JumpLocomotion.State.InPreparation)
            //    return;
            var legLength = _bonesDefinitions.LegLength;
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
                    upperLegPos = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
                else if (legNum == 1)
                    upperLegPos = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
                else
                    throw new InvalidLegNumException();
                var v = footNewPos - upperLegPos;
                var length = v.magnitude - legLength;
                return (v, length);
            }
            void UpdateBodyPosition(Vector3 towards, float length)
            {
                var tv = towards.normalized * length;
                var pv = Vector3.Project(tv, context.Transform.up);
                animator.bodyPosition += pv;
            }
        }

        public override void OnAnimationIK(int layerIndex)
        {
            UpdateFootIKOnGround();
        }

    }
}