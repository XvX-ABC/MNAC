using Tests.Interaction;
using UnityEngine;

namespace Tests.Characters.C_0
{
    internal class C_0Definitions_MonoComponent : MonoBehaviour, ICharacterDefinitions_C_0
    {
        [SerializeField]
        LayerMask _projectilesLayerMaskToHit;
        [SerializeField]
        LayerMask _swordLayerMaskToHit;
        [SerializeField]
        TeamMask _teamMask;
        public LayerMask ProjectilesLayerMaskToHit => _projectilesLayerMaskToHit;

        public TeamMask TeamMask => _teamMask;

        public LayerMask SwordLayerMaskToHit => _swordLayerMaskToHit;
    }
}
