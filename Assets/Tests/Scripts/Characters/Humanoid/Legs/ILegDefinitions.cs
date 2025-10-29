using UnityEngine;

namespace Tests.Characters.Humanoid.Legs
{
    public interface ILegDefinitions
    {
        public LayerMask FootIKLayer { get; }
        public Vector3 FootIKPositionOffset { get; }
    }
}
