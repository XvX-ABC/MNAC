using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Interaction;
using MNAC.States;
using UnityEngine;

namespace MNAC.Characters.C_0
{
    internal interface ICharacterDefinitions_C_0
    {
        public HealthDefinitions Health { get; }
        public LayerMask ProjectilesLayerMaskToHit { get; }
        public LayerMask SwordLayerMaskToHit { get; }
        public TeamMask TeamMask { get; }
        public IDeathDefinitions Death { get; }
        public BlendingTransitionOptions GetTransitionOptions(BehavioursTransition transition);
    }

}
