using System;
using System.Diagnostics.CodeAnalysis;
using MNAC.Characters.Humanoid.Input;
using MNAC.Input;
using MNAC.TPhysics.Locomotion;
using Core = MNAC.TPhysics.Locomotion.LocomotionCore;
namespace MNAC.Characters.Humanoid.Locomotion
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
