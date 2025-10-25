using System;
using System.Collections;
using System.Collections.Generic;
using Tests.Characters.Locomotion;
using Tests.Characters.UI;
using Tests.Input;
using Tests.Interaction;
using Tests.UI;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    public class ScreenCircleTargetsCatcher : ComponentBase, ITargetsCatcher
    {
        GameObjsInScreenFilter _filter;
        Tests.Interaction.ScreenCircleTargetsCatcher _catcher;
        RingCatcher _ringCatcher;
        TargetsDisplay _targetDisplay;
        Camera _camera;

        IInput _input;

        ITargetsCatcherDefinitions_V0 _definitions;

        GameObject _actorObj;
        public ScreenCircleTargetsCatcher(ITargetsCatcherDefinitions_V0 definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }
        public override string Name => "launcher_targets_catcher_v0";

        public IReadOnlyList<ITarget> Targets => _catcher.Targets;

        public Action<IList<ITarget>> TargetsChangedAction { get => _catcher.TargetsChangedAction; set => _catcher.TargetsChangedAction = value; }
        public float CatchingRadius
        {
            get => _catcher.Radius;
            set
            {
                _catcher.Radius = value;
            }
        }
        public override bool Enabled
        {
            get => base.Enabled;
            //set => base.Enabled = value;
            set
            {
                base.Enabled = value;
                _catcher.Enabled = value;
                _ringCatcher.HIde = !value;
                _filter.enabled = value;
            }
        }
        internal Tests.Interaction.ScreenCircleTargetsCatcher catcher
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
            if (!blackboard.TryReadValue<Camera>(CharacterBlackboardFields.Character_Camera_Main, out _camera))
                throw new Exception();
            if (!blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Character_Input_Main, out _input))
                throw new Exception();
            if (!blackboard.TryReadValue<GameObject>(CharacterBlackboardFields.Character_Obj_Main, out _actorObj))
                throw new Exception();
            blackboard.TryReadUIValueOrThrowException(CharacterUIBlackboardFields.Catcher_Ring, out _ringCatcher);
            blackboard.TryReadUIValueOrThrowException(CharacterUIBlackboardFields.Targets_Display, out _targetDisplay);
            _filter = new(_definitions.CatchingObjsTag, _camera, _definitions.FilterCountOneFrame);
            _catcher = new(_filter.ObjsInScreen, _actorObj, _camera, _definitions.TargetsMask, _definitions.FilterCountOneFrame);

            _ringCatcher.Camera = _camera;

            this.Enabled = base.Enabled;
        }
        void ShowAllWaitingForSelectObjs()
        {
            if (_filter.ObjsInScreen.Count == 0)
                return;
            var obj = _filter.ObjsInScreen[0];
            _targetDisplay.Activated = obj != null;
            if (_targetDisplay.Activated)
            {
                _targetDisplay.TargetWorldPos = obj.transform.position;
            }
        }
        public void Update()
        {
            _catcher.ActorPosition = _actorObj.transform.position;
            _catcher.MousePosition = _input.MousePosition;
            UpdateRingCatcher();
            ShowAllWaitingForSelectObjs();
        }
        void UpdateRingCatcher()
        {
            _ringCatcher.MousePosition = _input.MousePosition;
            var pixelSize = _camera.pixelRect.size;
            _ringCatcher.RingRadius = Mathf.Min(pixelSize.x, pixelSize.y) * _catcher.Radius;
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
        SimpleLeadingActorTargetsCatcher _catcher;
        ITargetsCatcherDefinitions _definitions;
        public TargetCatcher(ITargetsCatcherDefinitions definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }
        internal SimpleLeadingActorTargetsCatcher catcher
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
