using System;
using Tests.Interaction;
using Tests.Interaction.Targets;
using UnityEngine;

namespace Tests.Weapons.Sword
{
    public class SwordTargetsTrigger : TargetsTrigger_MonoComponent
    {
        protected new class Catcher : TargetsCatcherBase_MonoComponent.Catcher
        {
            Action<GameObjTarget> _whenTargetAdditionAction;
            Action<GameObjTarget> _whenTargetRemovalAction;

            public Action<GameObjTarget> WhenTargetAdditionAction { get => _whenTargetAdditionAction; set => _whenTargetAdditionAction = value; }
            public Action<GameObjTarget> WhenTargetRemovalAction { get => _whenTargetRemovalAction; set => _whenTargetRemovalAction = value; }

            protected override void AddTarget(ITarget_Obsolete target)
            {
                base.AddTarget(target);
                _whenTargetAdditionAction?.Invoke((GameObjTarget)target);
            }
            protected override void RemoveTarget(ITarget_Obsolete target)
            {
                base.RemoveTarget(target);
                _whenTargetRemovalAction?.Invoke((GameObjTarget)target);
            }

        }
        protected new Catcher catcher;
        protected override TargetsCatcherBase_MonoComponent.Catcher CreateCatcher()
        {
            catcher = new Catcher();
            return catcher;
        }


        public Action<GameObjTarget> WhenTargetEntryAction { get => catcher.WhenTargetAdditionAction; set => catcher.WhenTargetAdditionAction = value; }
        public Action<GameObjTarget> WhenTargetExitAction { get => catcher.WhenTargetRemovalAction; set => catcher.WhenTargetRemovalAction = value; }
    }
}
