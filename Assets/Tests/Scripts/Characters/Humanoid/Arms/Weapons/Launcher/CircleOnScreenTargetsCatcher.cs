using System;
using System.Collections;
using System.Collections.Generic;
using Tests.Behaviours.Input;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.UI;
using Tests.Input;
using Tests.Interaction;
using Tests.UI;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    public class CircleOnScreenTargetsCatcher : ComponentBase, ITargetsCatcher
    {
        GameObjsInScreenFilter _filter;
        Tests.Interaction.CircleOnScreenTargetsCatcher _catcher;
        RingCatcher _ringCatcher;
        TargetsDisplay _targetDisplay;
        Camera _camera;
        IBaseInput _input;

        ITargetsCatcherDefinitions_V0 _definitions;

        GameObject _actorObj;
        [Obsolete]
        public CircleOnScreenTargetsCatcher(ITargetsCatcherDefinitions_V0 definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }
        public CircleOnScreenTargetsCatcher(Camera camera, IBaseInput input, GameObject actorObj, RingCatcher ringCatcher, TargetsDisplay targetsDisplay, ITargetsCatcherDefinitions_V0 definitions, bool enabled = true)
        {
            _filter = new(definitions.CatchingObjsTag, camera, definitions.FilterAmountOneFrame);
            _catcher = new(_filter.ObjsInScreen, actorObj, camera, definitions.TargetsMask, definitions.FilterAmountOneFrame);
            _ringCatcher = ringCatcher ?? throw new ArgumentNullException(nameof(ringCatcher));
            _actorObj = actorObj ?? throw new ArgumentNullException(nameof(actorObj));
            _targetDisplay = targetsDisplay ?? throw new ArgumentNullException(nameof(targetsDisplay));
            _ringCatcher.Camera = camera;

            _input = input ?? throw new ArgumentNullException(nameof(input));

            _camera = camera;

            Enabled = enabled;
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
        internal Tests.Interaction.CircleOnScreenTargetsCatcher catcher
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
            if (!blackboard.TryReadValue(CharacterBlackboardFields.Character_Camera_Main, out _camera))
                throw new Exception();
            blackboard.TryReadUIValueOrThrowException<IHumanInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
            if (!blackboard.TryReadValue(CharacterBlackboardFields.Character_Obj_Main, out _actorObj))
                throw new Exception();
            blackboard.TryReadUIValueOrThrowException(CharacterUIBlackboardFields.Catcher_Ring, out _ringCatcher);
            blackboard.TryReadUIValueOrThrowException(CharacterUIBlackboardFields.Targets_Display, out _targetDisplay);
            _filter = new(_definitions.CatchingObjsTag, _camera, _definitions.FilterAmountOneFrame);
            _catcher = new(_filter.ObjsInScreen, _actorObj, _camera, _definitions.TargetsMask, _definitions.FilterAmountOneFrame);

            _ringCatcher.Camera = _camera;

            Enabled = base.Enabled;
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
}
