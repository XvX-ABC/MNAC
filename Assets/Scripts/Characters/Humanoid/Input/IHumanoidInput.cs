using Tests.Behaviours.Input;
using UnityEngine;

namespace Tests.Characters.Humanoid.Input
{
    public interface IHumanoidInput
    {
        public IBaseInput BaseInput { get; }
        public Vector2 MousePosition { get => BaseInput?.MousePosition ?? Vector3.positiveInfinity; }
        public Vector2 MousePositionDelta { get => BaseInput?.MousePositionDelta ?? Vector3.positiveInfinity; }
        public Vector3 HorizontalVector { get => BaseInput?.HorizontalVector ?? Vector3.zero; }
        public bool Jump { get; }
        public bool QuickBoost { get; }
        public IArmInput LArm { get; }
        public IArmInput RArm { get; }
    }
}
