using Tests.Behaviours.Input;
using UnityEngine;

namespace Tests.Characters.Interaction.Input
{
    public interface IHumanInput
    {
        public IBaseInput BaseInput { get; }
        public Vector2 MousePosition { get => BaseInput?.MousePosition ?? Vector3.positiveInfinity; }
        public Vector3 HorizontalVector { get => BaseInput?.HorizontalVector ?? Vector3.zero; }
        public bool Jump { get; }
        public bool QuickBoost { get; }
        public IArmInput LArm { get; }
        public IArmInput RArm { get; }
        public IArmInput LBArm { get; }
        public IArmInput RBArm { get; }
    }
}
