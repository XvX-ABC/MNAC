using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.Weapons.Sword
{
    internal abstract class SwordActionComponent : SwordComponent
    {
        protected Sword sword;
        protected internal float multiplier;
        float _duration;
        SwordAction _ownerAction;

        protected abstract SwordActionType actionType { get; }
        protected internal virtual float duration { get => _duration; set => _duration = value; }

        protected override void Awake()
        {
            base.Awake();
            this.enabled = false;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException(SwordActionComponent.OwnerSword, out sword); //TODO：没必要从黑板中读取，在Sword中初始化组件时可以直接传入
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