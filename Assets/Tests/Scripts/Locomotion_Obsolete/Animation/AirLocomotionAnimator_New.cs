using System;
using Tests.Environment;
using UnityEngine;
using static Tests.Locomotion_Obsolete.AirLocomotion;
namespace Tests.Locomotion_Obsolete.Animation
{
    [Obsolete]
    public class AirLocomotionAnimator_New : MonoBehaviour, IModule
    {
        IAirLocomotionAnimationDefinitions _definitions;
        Animator _animator;
        AirLocomotion _locomotion;
        float _maxHeight;
        IGroundDetector _detector;
        void Awake()
        {
            _definitions = GetComponent<ILocomotionAnimationDefinitions>().Air ?? throw new NullReferenceException(nameof(_definitions));
            _animator = GetComponent<Animator>();
            _detector = GetComponent<IGroundDetector>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IGroundDetector));
        }
        void Start()
        {
            var controlBase = GetComponent<LocomotionCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(LocomotionCore));
            _locomotion = controlBase.airLocomotion ?? throw new NullReferenceException(nameof(_locomotion));
            _locomotion.PostureChangedAction += (oldStatus, newStatus, ctx) =>
            {
                _animator.SetBool(_definitions.EnterParamName, newStatus == PostureState.Ascending);
                _animator.SetBool(_definitions.DescendingEntryParamName, newStatus == PostureState.Descending);
            };
            //_locomotion.DescendingAction += _ => Descend();
        }
        void Descend()
        {
            var groundHeight = _detector.GroundHeight;
            var currentHeight = _detector.Distance;
            var max = _maxHeight - groundHeight;
            if (max == 0)
                return;
            var v = currentHeight / max;
            _animator.SetBool(_definitions.DescendingEntryParamName, true);
            //if (v > _definitions.V0)
            //{
            //    _animator.Play(_definitions.DescentClipName, 0, Mathf.Clamp01(1 - v));
            //}
            //else
            //{
            //    _animator.CrossFade(_definitions.NextStateClipName, _definitions.V0, 0, 1 - _definitions.V0, 0);
            //}
        }
        void UpdateVelocity(Context context)
        {
            var rotation = context.OriginalLocomotion.Rotation;
            var velocity = context.Velocity;
            var v = Quaternion.Inverse(rotation) * velocity * 0.05f;
            _animator.SetFloat(_definitions.XParamName, v.x);
            _animator.SetFloat(_definitions.YParamName, v.z);
        }
        public void OnFixedUpdate(Context context)
        {
            var ground = context.Ground;
            if (ground == null)
            {
                _maxHeight = Mathf.Max(_maxHeight, context.Position.y);
            }
            else
            {
                _maxHeight = 0;
            }
            UpdateVelocity(context);
        }
    }
}