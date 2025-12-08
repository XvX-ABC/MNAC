using System;
using Tests.Animations;
using Tests.Characters.Interaction;
using Tests.Interaction;
using Tests.Interaction.Influence;
using Tests.Utilities.Blackboards;
using UnityEngine;
using Health = Tests.Interaction.Health;

namespace Tests.Characters
{
    [Interactable]
    public class Character_Debug : CharacterBase, ITeamMember, IDamageable
    {
        [SerializeField]
        Health _health;
        [SerializeField]
        TeamMask _teamMask;
        public IHealth HP => _health;

        public TeamMask TeamMask { get => _teamMask; set => _teamMask = value; }

        protected override void Awake()
        {
        }
        protected override void Start()
        {
        }

        protected override void OnEnable()
        {
            TryRegisterToInteractionManager();
        }
        protected override void OnDisable()
        {
            UnregisterFromInteractionManager();
        }
        protected override void Update()
        {
        }
        protected override void OnDestroy()
        {
        }
        internal override void ComponentsDispose()
        {
        }

        internal override InfluenceCore CreateInfluenceCore()
        {
            return default;
        }

        internal override CharacterBehavioursStatemachine CreateStatemachine()
        {
            return null;
        }

        internal override CharacterComponent[] GetComponents()
        {
            return null;
        }

        internal override AnimationPlayablePartBase GetMainAnimationPlayablePart()
        {
            return null;
        }

        internal override void InitializeComponents(Blackboard blackboard)
        {

        }
    }
}
