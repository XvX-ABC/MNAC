using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Animations;
using Tests.Characters.Interaction.Influences;
using Tests.Interaction;
using Tests.Interaction.Influences;
using Tests.Utilities.Blackboards;
using UnityEngine;
using Health = Tests.Interaction.Influences.Health;

namespace Tests.Characters
{
    [Interactable]
    public class Character_Debug : CharacterBase, ITeamMember, IDamageable, IKnockbackable
    {
        [SerializeField]
        Health _health;
        [SerializeField]
        TeamMask _teamMask;
        [SerializeField]
        Collider _collider;
        Knockback_Rbody _knockbackInfluence;
        public IHealth HP => _health;

        public TeamMask TeamMask { get => _teamMask; set => _teamMask = value; }

        protected override Bounds bounds => _collider.bounds;

        public IKnockback Knockback => _knockbackInfluence;

        protected override void Awake()
        {
            influenceCore = CreateInfluences();
            colliders = GetComponentsInChildren<Collider>().ToList();
            accessors = new();
            base.SetAccessorsForChildrenColliders();
            //SetAccessors();
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

        internal override InfluenceCore CreateInfluences()
        {
            var core = new InfluenceCore(_health);
            if (TryGetComponent<Rigidbody>(out var rbody))
            {
                _knockbackInfluence = new(rbody);
                core.AddInfluence(_knockbackInfluence);
            }
            return core;
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
        //void SetAccessors()
        //{
        //    var colliders = GetComponentsInChildren<Collider>();
        //    foreach (var c in colliders)
        //    {
        //        var obj = c.gameObject;
        //        if (obj == this)
        //            continue;
        //        var accessor = obj.AddComponent<CharacterAccessor_Debug>(); ;
        //        accessor.character = this;
        //        _accessors.Add(accessor);
        //    }
        //}
        //void DestroyAccessors()
        //{
        //    foreach (var a in _accessors)
        //    {
        //        Destroy(a);
        //    }
        //    _accessors.Clear();
        //}

        internal override CharacterAccessor SetAccessorToObj(GameObject obj)
        {
            return obj.AddComponent<CharacterAccessor_Debug>();
        }
    }
}
