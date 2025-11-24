using Tests.Utilities.Blackboards;
using UnityEditor.Build.Content;

namespace Tests.Weapons_New.Sword
{
    internal abstract class SwordActionComponent : SwordComponent
    {
        protected Sword sword;
        protected abstract SwordActionType actionType { get; }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException(SwordActionComponent.OwnerSword, out sword);
            var action = sword.actions[actionType];
            action.AddComponent(this);
        }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
    }
}