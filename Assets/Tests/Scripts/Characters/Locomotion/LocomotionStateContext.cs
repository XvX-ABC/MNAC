using System;
using System.Diagnostics.CodeAnalysis;
using Tests.Input;
using Tests.TPhysics.Locomotion;
using Core = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.Characters.Locomotion
{
    internal class LocomotionStateContext
    {
        Core _core;
        IInput _input;
        Action<IInput> _inputBoundAction;
        public LocomotionStateContext([NotNull] Core core, [NotNull] IInput input)
        {
            _core = core;
            _input = input;
        }

        public Core Core { get => _core; }
        public IInput Input { get => _input; }
        public Action<IInput> InputBoundAction { get => _inputBoundAction; set => _inputBoundAction = value; }
        public void Update()
        {
            InputBoundAction?.Invoke(_input);
        }
    }
}
