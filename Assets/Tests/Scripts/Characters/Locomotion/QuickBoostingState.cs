using System.Diagnostics.CodeAnalysis;
using Tests.Input;
using Tests.States;
using Tests.TPhysics.Locomotion;
using TMPro;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Characters.Locomotion
{
    internal class QuickBoostingHelper
    {
        QuickBoostingState _state;
        ITimeline _cdTimeline;
        IInput _input;
        public QuickBoostingHelper([NotNull] IMovementDefinitions movementDefinitions, [NotNull] IQuickBoostingDefinitions definitions, bool enabled = true)
        {
            _state = new QuickBoostingState(movementDefinitions, definitions, enabled);
            _cdTimeline = new Timeline_V1(definitions.ColdDownTime);

            _state.ExitAction += () => _cdTimeline.Restart();
            _cdTimeline.SetNormalizedTime(1);
        }

        public QuickBoostingState State { get => _state; set => _state = value; }
        public bool IsColdDowned
        {
            get => _cdTimeline.NormalizedTime >= 1;
        }
        public IInput Input { get => _input; set => _input = value; }

        public bool TriggerEvent
        {
            get => _input == null ? false : _input.HorizontalVector != Vector3.zero && _input.QuickBoost && IsColdDowned;
        }
        public void Update()
        {
            _cdTimeline.OnUpdate(Time.deltaTime);
        }
    }
    //TODO: 删除定义中增量速度相关内容
    internal class QuickBoostingState : LocomotionStateBase
    {
        BoostingLocomotion locomotion;
        public QuickBoostingState(
            [NotNull] IMovementDefinitions movementDefinitions,
            [NotNull] IQuickBoostingDefinitions definitions,
            bool enabled = true) : base("quick_boosting", definitions.Duration, enabled)
        {
            var speed = movementDefinitions.MaxSpeed * Mathf.Max(1, definitions.MaxSpeedPower);
            locomotion = new BoostingLocomotion(speed, 0);
        }

        protected override ILocomotionModule module => locomotion;

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            context.Core.EnableModule(module);
            locomotion.HorizontalVector = context.Input.HorizontalVector;
        }
        public override void OnExit()
        {
            locomotion.HorizontalVector = Vector3.zero;
            base.OnExit();
        }
    }
}
