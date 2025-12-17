using System;
using UnityEngine;
using static Tests.AI.AIHumanoidInput;
using static Tests.AI.NavigationModule;

namespace Tests.AI
{
    internal class AIHumanoidInputComponent : AIComponent
    {
        class Handler : IPositionChangeHandler
        {
            BInput _input;
            public Handler(BInput input)
            {
                _input = input ?? throw new ArgumentNullException(nameof(input));
            }
            public void Handle(Vector3 currentPos, Vector3 nextPos)
            {
                _input.horizontalVector = (nextPos - currentPos).normalized;
                Debug.DrawLine(currentPos, currentPos + Vector3.up * 10, Color.yellow);
                Debug.DrawLine(nextPos, nextPos + Vector3.up * 10, Color.red);
                Debug.Log($"cpos: {currentPos}, npos: {nextPos}, hv: {_input.horizontalVector}");
            }
        }

        internal AIHumanoidInput input;
        AINavigation _navigation;
        Handler _handler;
        protected override void Awake()
        {
            base.Awake();
            input = new();
            _handler = new(input.baseInput);
        }
        public override void Initialize(AIComponentContext context)
        {
            base.Initialize(context);
            _navigation = context.navigation ?? throw new NullReferenceException(nameof(context.navigation));
            _navigation.module.handlers.Add(_handler);
        }
        public override void Dispose()
        {
            _navigation.module.handlers.Remove(_handler);
            _navigation = null;
            base.Dispose();
        }
    }
}
