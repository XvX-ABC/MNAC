using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Characters.C_0
{
    internal interface ICharacterDefinitions_C_0
    {
        public LayerMask LayerMaskToHit { get; }
        public TeamMask TeamMask { get; }
    }
    internal class C_0Definitions : ICharacterDefinitions_C_0
    {
        [SerializeField]
        LayerMask _layerMask;
        [SerializeField]
        TeamMask _teamMask;

        public LayerMask LayerMaskToHit => _layerMask;

        public TeamMask TeamMask => _teamMask;
    }
}
