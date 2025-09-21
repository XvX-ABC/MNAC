using System;
using System.Collections.Generic;
using Tests.Characters.Locomotion;
using Tests.Input;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    public class TargetCatcher : ComponentBase, ITargetsCatcher
    {
        LeadingActorTargetsCather _catcher;
        ITargetsCatcherDefinitions _definitions;
        public TargetCatcher(ITargetsCatcherDefinitions definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }
        internal LeadingActorTargetsCather catcher
        {
            get
            {
                if (_catcher == null)
                    throw new Exception("This component node is not initialized, so don't try to get any element of this node");
                return _catcher;
            }
        }
        public IReadOnlyList<ITarget> Targets => catcher.Targets;

        public Action<IList<ITarget>> TargetsChangedAction { get => catcher.TargetsChangedAction; set => catcher.TargetsChangedAction = value; }

        public override string Name => "launcher_targets_catcher";
        // TODO: 逻辑需要优化
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (!blackboard.TryReadValue<Camera>(CharacterBlackboardFields.Character_Camera_Main, out var camera))
                throw new Exception();
            if (!blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Character_Input_Main, out var input))
                throw new Exception();
            if (!blackboard.TryReadValue<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotion))
                throw new Exception();
            if (!blackboard.TryReadValue<GameObject>(CharacterBlackboardFields.Character_Obj_Main, out var obj))
                throw new Exception();
            _catcher = new(_definitions, obj, camera, input, locomotion.core);
        }
        public void Update()
        {
            catcher.Update();
        }
    }
}
