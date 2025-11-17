using System;
using System.Collections.Generic;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Input;
using Tests.Interaction;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    [Obsolete]
    public class TargetCatcher_Obsolete : ComponentBase, ITargetsCatcher
    {
        SimpleLeadingActorTargetsCatcher_Obsolete _catcher;
        ITargetsCatcherDefinitions _definitions;
        public TargetCatcher_Obsolete(ITargetsCatcherDefinitions definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }
        internal SimpleLeadingActorTargetsCatcher_Obsolete catcher
        {
            get
            {
                if (_catcher == null)
                    throw new Exception("This component node is not initialized, so don't try to get any element of this node");
                return _catcher;
            }
        }
        public IReadOnlyList<Tests.Interaction.ITarget_Obsolete> Targets => catcher.Targets;

        public Action<IList<Tests.Interaction.ITarget_Obsolete>> TargetsChangedAction { get => catcher.TargetsChangedAction; set => catcher.TargetsChangedAction = value; }

        public override string Name => "launcher_targets_catcher";
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<Camera>(CharacterBlackboardFields.Character_Camera_Main, out var camera);
            blackboard.TryReadValueOrThrowException<IInput_Obsolete>(CharacterBlackboardFields.Character_Input_Main_Obsolete, out var input);
            blackboard.TryReadValueOrThrowException<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotion);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Main, out var obj);

            //_catcher = new(_definitions, obj, camera, input, locomotion.core);
        }
        public void Update()
        {
            catcher.Update();
        }
    }
}
