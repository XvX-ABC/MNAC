using System;
using Tests.Interaction;
using Tests.States;
using UnityEngine;

namespace Tests.Characters.C_0
{

    [Serializable]
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
        [SerializeField]
        BehaviourTransitionOptions[] _transitionOptions;
        [SerializeField]
        DeathDefinitions _death;

        public LayerMask ProjectilesLayerMaskToHit => _projectilesLayerMaskToHit;

        public TeamMask TeamMask => _teamMask;

        public LayerMask SwordLayerMaskToHit => _swordLayerMaskToHit;

        public HealthDefinitions Health => _health;

        public IDeathDefinitions Death { get => _death; }

        public BlendingTransitionOptions GetTransitionOptions(BehavioursTransition transition)
        {
            var idx = Array.FindIndex(_transitionOptions, o => o.transition == transition);
            if (idx == -1)
                throw new TransitionOptionsCantFoundException($"Can't found a transition options by '{transition}'");
            return _transitionOptions[idx];
        }
    }
}
