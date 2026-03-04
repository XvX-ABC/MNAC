using MNAC.Interaction;
using MNAC.States;
using UnityEngine;

namespace MNAC.Characters.C_0
{
    [CreateAssetMenu(fileName = "C_0_Definitions", menuName = SOHelper.DEFINITIONS_MENU_NAME + "/C_0")]
    internal class C_0Definitions_SO : ScriptableObject, ICharacterDefinitions_C_0
    {
        [SerializeField]
        C_0Definitions _definitions;

        public HealthDefinitions Health => ((ICharacterDefinitions_C_0)_definitions).Health;

        public LayerMask ProjectilesLayerMaskToHit => ((ICharacterDefinitions_C_0)_definitions).ProjectilesLayerMaskToHit;

        public LayerMask SwordLayerMaskToHit => ((ICharacterDefinitions_C_0)_definitions).SwordLayerMaskToHit;

        public TeamMask TeamMask => ((ICharacterDefinitions_C_0)_definitions).TeamMask;

        public IDeathDefinitions Death => ((ICharacterDefinitions_C_0)_definitions).Death;

        public BlendingTransitionOptions GetTransitionOptions(BehavioursTransition transition)
        {
            return ((ICharacterDefinitions_C_0)_definitions).GetTransitionOptions(transition);
        }
    }
}
