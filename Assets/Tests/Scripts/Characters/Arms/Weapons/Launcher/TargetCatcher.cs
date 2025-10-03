using System;
using System.Collections;
using System.Collections.Generic;
using Tests.Characters.Locomotion;
using Tests.Input;
using Tests.Interaction;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    public class TargetsCatcher_V0 : ComponentBase, ITargetsCatcher
    {
        GameObjsInScreenFilter _filter;
        CircleRangeTargetsCatcher _catcher;

        IInput _input;

        ITargetsCatcherDefinitions_V0 _definitions;

        GameObject _actorObj;
        public TargetsCatcher_V0(ITargetsCatcherDefinitions_V0 definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }
        public override string Name => "launcher_targets_catcher_v0";

        public IReadOnlyList<ITarget> Targets => _catcher.Targets;

        public Action<IList<ITarget>> TargetsChangedAction { get => _catcher.TargetsChangedAction; set => _catcher.TargetsChangedAction = value; }
        internal CircleRangeTargetsCatcher catcher
        {
            get
            {
                if (_catcher == null)
                    throw new Exception("This component node is not initialized, so don't try to get any element of this node");
                return _catcher;
            }
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (!blackboard.TryReadValue<Camera>(CharacterBlackboardFields.Character_Camera_Main, out var camera))
                throw new Exception();
            if (!blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Character_Input_Main, out _input))
                throw new Exception();
            if (!blackboard.TryReadValue<GameObject>(CharacterBlackboardFields.Character_Obj_Main, out _actorObj))
                throw new Exception();
            _filter = new(_definitions.CatchingObjsTag, camera, _definitions.FilterCountOneFrame);
            _catcher = new(_filter.ObjsInScreen, _actorObj, _definitions.TargetsMask, _definitions.FilterCountOneFrame);
        }
        public void Update()
        {
            _catcher.ActorPosition = _actorObj.transform.position;
            _catcher.MousePosition = _input.MousePosition;
            _catcher.ViewPortRadius = _definitions.CatchingViewPortRadius;
        }
        public IEnumerator FilterUpdateWithCoroutine()
        {
            return _filter.Update();
        }
        public IEnumerator CatcherUpdateWithCoroutine()
        {
            return _catcher.UpdateWithCoroutine();
        }
    }
    public class TargetCatcher : ComponentBase, ITargetsCatcher
    {
        SimpleLeadingActorTargetsCather _catcher;
        ITargetsCatcherDefinitions _definitions;
        public TargetCatcher(ITargetsCatcherDefinitions definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }
        internal SimpleLeadingActorTargetsCather catcher
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
