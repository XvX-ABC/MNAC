using System;
using System.Diagnostics.CodeAnalysis;
using Tests.Characters.Interaction.Input;
using Tests.Input;
using Tests.TPhysics.Locomotion;
using Core = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.Characters.Humanoid.Locomotion
{
    internal class LocomotionStateContext
    {
        //TODO: 不应该绑定固定对象
        Core _core;
        IHumanInput _input;
        Action<IHumanInput> _inputBoundAction;
        public LocomotionStateContext([NotNull] Core core, [NotNull] IHumanInput input)
        {
            _core = core;
            _input = input;
        }
        public Core Core { get => _core; }
        public Action<IHumanInput> InputBoundAction { get => _inputBoundAction; set => _inputBoundAction = value; }
        public void Update()
        {
            InputBoundAction?.Invoke(_input);
        }
    }
}
