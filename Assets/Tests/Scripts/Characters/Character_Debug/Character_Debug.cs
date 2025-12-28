using System;
using System.Collections.Generic;
using Tests.Animations;
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

        protected override Bounds bounds => throw new NotImplementedException();

        protected override void Awake()
        {
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
            var a = obj.AddComponent<CharacterAccessor_Debug>();
            a.character = this;
            return a;
        }
    }
}
