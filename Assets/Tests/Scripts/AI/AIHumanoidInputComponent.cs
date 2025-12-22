using System;
using UnityEngine;
using static Tests.AI.AIHumanoidInput;
using static Tests.AI.NavigationModule;
namespace Tests.AI
{
    internal class AIHumanoidInputComponent : AIComponent_Mono
    {
        class Handler : INavigationHandler
        {
            BInput _input;
            public Handler(BInput input)
            {
                _input = input ?? throw new ArgumentNullException(nameof(input));
            }
            public void PositionChange(Vector3 currentPos, Vector3 nextPos)
            {
                _input.horizontalVector = (nextPos - currentPos).normalized;
                Debug.DrawLine(currentPos, currentPos + Vector3.up * 10, Color.yellow);
                Debug.DrawLine(nextPos, nextPos + Vector3.up * 10, Color.red);
            }

            public void SetDestination(Vector3 pos)
            {
            }

            public void Stop()
            {
                _input.horizontalVector = Vector3.zero;
            }
        }

        internal AIHumanoidInput input;
        AINavigation _navigation;
        Handler _handler;
        public override void Initialize(AIComponentContext context)
        {

            base.Initialize(context);
            input = new();
            _handler = new(input.baseInput);
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
