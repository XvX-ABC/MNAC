using UnityEngine;

namespace Tests.Characters.Legs
{
    public class LegDefinitions : MonoBehaviour, ILegDefinitions
    {
        [SerializeField]
        LayerMask _layer;
        [SerializeField]
        Vector3 _positionOffset;

        public LayerMask FootIKLayer => _layer;

        public Vector3 FootIKPositionOffset => _positionOffset;
    }
}
