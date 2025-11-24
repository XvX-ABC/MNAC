using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    internal abstract class SwordEffector : SwordEffectComponent
    {

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            sword.tipTrigger.EntryAction += WhenTargetEnter;
            sword.tipTrigger.ExitAction += WhenTargetExit;
        }
        public override void Dispose()
        {
            var action = sword.actions[actionType];
            action.RemoveComponent(this);

            sword.tipTrigger.EntryAction -= WhenTargetEnter;
            sword.tipTrigger.ExitAction -= WhenTargetExit;
            base.Dispose();
        }
        protected abstract void WhenTargetEnter(GameObject ob);
        protected abstract void WhenTargetExit(GameObject obj);
    }
}
