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
    [Serializable]
    public struct HealthDefinitions
    {
        [SerializeField]
        public float MaxPoint;
    }
    public enum BehavioursTransition
    {
        None,
        Normal_Death
    }
    internal interface ICharacterDefinitions_C_0
    {
        public HealthDefinitions Health { get; }
        public LayerMask ProjectilesLayerMaskToHit { get; }
        public LayerMask SwordLayerMaskToHit { get; }
        public TeamMask TeamMask { get; }
        public BlendingTransitionOptions GetTransitionOptions(BehavioursTransition transition);
    }
    [Serializable]
    internal class BehaviourTransitionOptions : BlendingTransitionOptions
    {
        [SerializeField]
        internal BehavioursTransition transition;
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
        [SerializeField]
        BehaviourTransitionOptions[] _transitionOptions;

        public LayerMask ProjectilesLayerMaskToHit => _projectilesLayerMaskToHit;

        public TeamMask TeamMask => _teamMask;

        public LayerMask SwordLayerMaskToHit => _swordLayerMaskToHit;

        public HealthDefinitions Health => _health;

        public BlendingTransitionOptions GetTransitionOptions(BehavioursTransition transition)
        {
            var idx = Array.FindIndex(_transitionOptions, o => o.transition == transition);
            if (idx == -1)
                throw new TransitionOptionsCantFoundException($"Can't found a transition options by '{transition}'");
            return _transitionOptions[idx];
        }
    }
}
