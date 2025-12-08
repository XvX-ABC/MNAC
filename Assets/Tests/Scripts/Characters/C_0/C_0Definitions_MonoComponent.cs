using Tests.Interaction;
using UnityEngine;

namespace Tests.Characters.C_0
{
    internal class C_0Definitions_MonoComponent : MonoBehaviour, ICharacterDefinitions_C_0
    {
        [SerializeField]
        LayerMask _layerMaskToHit;
        [SerializeField]
        TeamMask _teamMask;
        public LayerMask LayerMaskToHit => _layerMaskToHit;

        public TeamMask TeamMask => _teamMask;
    }
}
