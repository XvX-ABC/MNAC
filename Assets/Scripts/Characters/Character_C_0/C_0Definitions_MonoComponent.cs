using System;
using MNAC.Interaction;
using MNAC.States;
using UnityEngine;

namespace MNAC.Characters.C_0
{
    [Obsolete]
    internal class C_0Definitions_MonoComponent : MonoBehaviour, ICharacterDefinitions_C_0
    {
        [SerializeField]
        LayerMask _projectilesLayerMaskToHit;
        [SerializeField]
        LayerMask _swordLayerMaskToHit;
        [SerializeField]
        TeamMask _teamMask;
        [SerializeField]
        HealthDefinitions _health;
        [SerializeField]
        BehaviourTransitionOptions[] _transitionOptions;
        public LayerMask ProjectilesLayerMaskToHit => _projectilesLayerMaskToHit;

        public TeamMask TeamMask => _teamMask;

        public LayerMask SwordLayerMaskToHit => _swordLayerMaskToHit;

        public HealthDefinitions Health => _health;

        public IDeathDefinitions Death => throw new NotImplementedException();

        public BlendingTransitionOptions GetTransitionOptions(BehavioursTransition transition)
        {
            var idx = Array.FindIndex(_transitionOptions, o => o.transition == transition);
            if (idx == -1)
                throw new TransitionOptionsCantFoundException($"Can't found a transition options by '{transition}'");

            return _transitionOptions[idx];
        }
    }
}
