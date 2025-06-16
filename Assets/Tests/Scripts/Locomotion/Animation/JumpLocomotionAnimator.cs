using System;
using Assets.Scripts.Utilities;
using NUnit.Framework.Constraints;
using Tests.Environment;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion.Animation
{
    public class JumpLocomotionAnimator : MonoBehaviour, IModule
    {

        IJumpLocomotionAnimationDefinitions _definitions;
        JumpLocomotion _jumpLocomotion;
        Animator _animator;
        IGroundDetector _detector;
        float _maxHeight;
        SingleEvent<Context> _ascendingStartEvent;
        SingleEvent<Context> _jumpEndEvent;
        void Awake()
        {
            _definitions = GetComponent<ILocomotionAnimationDefinitions>()?.Jump ?? throw new NullReferenceException(nameof(_definitions));

            _animator = GetComponent<Animator>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Animator));
            _detector = GetComponent<IGroundDetector>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IGroundDetector));


            _ascendingStartEvent = new(ctx => { _animator.SetTrigger(_definitions.EnterParamName); /*_animator.SetBool(_definitions.StateHoldingParamName, true)*/; });
            //_jumpEndEvent = new(ctx => { _maxHeight = 0; _animator.SetBool(_definitions.StateHoldingParamName, false); });
        }
        void Start()
        {
            _jumpLocomotion = GetComponent<LocomotionCore>()?.jumpLocomotion ?? throw new NullReferenceException(nameof(_jumpLocomotion));
            _jumpLocomotion.AscendingStartAction += (_, _) => _ascendingStartEvent.Enabled = true;
            _jumpLocomotion.JumpEndAction += (_, _) => _jumpEndEvent.Enabled = true;

            var length = _jumpLocomotion.ascendingDurationTime;
            var clipLength = _definitions.AscendingClipLength;
            var v = length == 0 ? 0 : clipLength / length;
            _animator.SetFloat(_definitions.AscendingMultiplierName, v);




        }
        void Landing()
        {
            var groundHeight = _detector.GroundHeight;
            var currentHeight = _detector.Distance;
            var v = currentHeight / (_maxHeight - groundHeight);
            if (v > 0.1f)
                _animator.Play(_definitions.DescendingClipName, 0, Mathf.Clamp01(1 - v));
        }
        public void OnFixedUpdate(Context context)
        {
            if (!enabled)
                return;
            var state = _jumpLocomotion.CurrentState;

            if (state == JState.Descending)
                Landing();

            if (state == JState.Ascending)
                _maxHeight = Mathf.Max(_maxHeight, context.Position.y);

            _ascendingStartEvent.TryExecute(context);
            //_jumpEndEvent.TryExecute(context);
        }
    }
}