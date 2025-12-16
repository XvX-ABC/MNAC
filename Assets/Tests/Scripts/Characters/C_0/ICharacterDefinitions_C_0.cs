using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Characters.C_0
{
    [Serializable]
    public struct HealthDefinitions
    {
        [SerializeField]
        public float MaxPoint;
    }
    internal interface ICharacterDefinitions_C_0
    {
        public HealthDefinitions Health { get; }
        public LayerMask ProjectilesLayerMaskToHit { get; }
        public LayerMask SwordLayerMaskToHit { get; }
        public TeamMask TeamMask { get; }
    }
    internal class C_0Definitions : ICharacterDefinitions_C_0
    {
        [SerializeField]
        HealthDefinitions _health;
        [SerializeField]
        LayerMask _projectilesLayerMaskToHit;
        [SerializeField]
        LayerMask _swordLayerMaskToHit;
        [SerializeField]
        TeamMask _teamMask;

        public LayerMask ProjectilesLayerMaskToHit => _projectilesLayerMaskToHit;

        public TeamMask TeamMask => _teamMask;

        public LayerMask SwordLayerMaskToHit => _swordLayerMaskToHit;

        public HealthDefinitions Health => _health;
    }
}
