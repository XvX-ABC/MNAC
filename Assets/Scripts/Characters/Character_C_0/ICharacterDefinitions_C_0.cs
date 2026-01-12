using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Interaction;
using Tests.States;
using UnityEngine;

namespace Tests.Characters.C_0
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
