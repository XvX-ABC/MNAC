using System;
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
            sword.HitAction += WhenHitTarget;
        }
        public override void Dispose()
        {
            var action = sword.actions[actionType];
            action.RemoveComponent(this);

            sword.tipTrigger.EntryAction -= WhenTargetEnter;
            sword.tipTrigger.ExitAction -= WhenTargetExit;
            sword.HitAction -= WhenHitTarget;
            base.Dispose();
        }
        [Obsolete]
        protected virtual void WhenTargetEnter(GameObject ob) { }
        [Obsolete]
        protected virtual void WhenTargetExit(GameObject obj) { }
        protected virtual void WhenHitTarget(GameObject obj) { }
    }
}
