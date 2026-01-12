using System;
using System.Diagnostics.CodeAnalysis;
using Tests.Characters.Humanoid.Input;
using Tests.Input;
using Tests.TPhysics.Locomotion;
using Core = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.Characters.Humanoid.Locomotion
{
    internal class LocomotionStateContext
    {
        //TODO: 不应该绑定固定对象
        Core _core;
        internal IHumanoidInput input;
        Action<IHumanoidInput> _inputBoundAction;
        public LocomotionStateContext([NotNull] Core core, [NotNull] IHumanoidInput input)
        {
            _core = core;
            this.input = input;
        }
        public Core Core { get => _core; }
        public Action<IHumanoidInput> InputBoundAction { get => _inputBoundAction; set => _inputBoundAction = value; }
        public void Update()
        {
            InputBoundAction?.Invoke(input);
        }
    }
}
