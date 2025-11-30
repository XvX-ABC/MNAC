using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    internal abstract class SwordActionComponent : SwordComponent
    {
        protected Sword sword;
        protected internal float multiplier;
        SwordAction _ownerAction;

        protected abstract SwordActionType actionType { get; }
        protected override void Awake()
        {
            base.Awake();
            this.enabled = false;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException(SwordActionComponent.OwnerSword, out sword);
            _ownerAction = sword.actions[actionType];
            _ownerAction.AddComponent(this);
        }
        public override void Dispose()
        {
            _ownerAction.RemoveComponent(this);
            base.Dispose();
        }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
    }
}