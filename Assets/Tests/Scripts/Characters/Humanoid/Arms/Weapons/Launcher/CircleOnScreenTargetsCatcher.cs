using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tests.Behaviours.Arms.Weapons.Launcher;
using Tests.Behaviours.Input;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.UI;
using Tests.Interaction;
using Tests.UI;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    public partial class CircleOnScreenTargetsCatcher : ComponentBase, ITargetsCatcher_New<GameObjTarget>, ITargetsCatcher
    {
        GameObjsInScreenFilter _filter;
        Tests.Interaction.CircleOnScreenTargetsCatcher _catcher;
        RingCatcher _ringCatcher;
        TargetsDisplay _targetDisplay;
        IndicatorsManager _indicatorsManager;
        Camera _camera;
        IBaseInput _input;
        Action<IList<ITarget>> _targetsChangedAction;

        ICircleOnScreenTargetsCatcherDefinitions _definitions;

        GameObject _actorObj;
        [Obsolete]
        public CircleOnScreenTargetsCatcher(ICircleOnScreenTargetsCatcherDefinitions definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }
        public CircleOnScreenTargetsCatcher(Camera camera, IBaseInput input, GameObject actorObj, RingCatcher ringCatcher, TargetsDisplay targetsDisplay, ICircleOnScreenTargetsCatcherDefinitions definitions, bool enabled = true)
        {
            _filter = new(camera, definitions.FilterAmountOneFrame);
            _catcher = new(_filter.ObjsInScreen, actorObj, camera, definitions.TargetsMask, definitions.FilterAmountOneFrame);
            _ringCatcher = ringCatcher ?? throw new ArgumentNullException(nameof(ringCatcher));
            _actorObj = actorObj ?? throw new ArgumentNullException(nameof(actorObj));
            _targetDisplay = targetsDisplay ?? throw new ArgumentNullException(nameof(targetsDisplay));
            _ringCatcher.Camera = camera;

            _input = input ?? throw new ArgumentNullException(nameof(input));

            _camera = camera;

            Enabled = enabled;

            _catcher.TargetsChangedAction += targets =>
            {
                _targetsChangedAction?.Invoke(targets.Cast<ITarget>().ToList());
            };
        }

        public CircleOnScreenTargetsCatcher(Camera camera, IBaseInput input, GameObject actorObj, RingCatcher ringCatcher, IndicatorsManager indicatorsManager, ICircleOnScreenTargetsCatcherDefinitions definitions, bool enabled = true)
        {
            _filter = new(camera, definitions.FilterAmountOneFrame);
            _catcher = new(_filter.ObjsInScreen, actorObj, camera, definitions.TargetsMask, definitions.FilterAmountOneFrame);
            _ringCatcher = ringCatcher ?? throw new ArgumentNullException(nameof(ringCatcher));
            _actorObj = actorObj ?? throw new ArgumentNullException(nameof(actorObj));
            _ringCatcher.Camera = camera;
            _indicatorsManager = indicatorsManager ?? throw new ArgumentNullException(nameof(indicatorsManager));
            _input = input ?? throw new ArgumentNullException(nameof(input));

            _camera = camera;

            Enabled = enabled;

            _catcher.TargetsChangedAction += targets =>
            {
                _targetsChangedAction?.Invoke(targets.Cast<ITarget>().ToList());
            };

            _catcher.Radius = definitions.CatchingViewPortRadius;
            _catcher.TargetCaughtAction += WhenTargetCaught;
            _catcher.TargetReleaseAction += WhenTargetRelease;
        }

        public override string Name => "launcher_targets_catcher_v0";

        public IReadOnlyList<GameObjTarget> Targets => _catcher.Targets;

        public Action<IList<GameObjTarget>> TargetsChangedAction { get => _catcher.TargetsChangedAction; set => _catcher.TargetsChangedAction = value; }
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
                if (_ringCatcher != null)
                    _ringCatcher.HIde = !value;
                _filter.Enabled = value;
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

        IReadOnlyList<ITarget> ITargetsCatcher.Targets => _catcher.Targets;

        Action<IList<ITarget>> ITargetsCatcher.TargetsChangedAction { get => _targetsChangedAction; set => _targetsChangedAction = value; }
        void WhenTargetCaught(GameObjTarget target)
        {
            var obj = target.Obj;
            _indicatorsManager.AddTargetFor<IndicatedTarget>(obj);
        }
        void WhenTargetRelease(GameObjTarget target)
        {
            var obj = target.Obj;
            _indicatorsManager.RemoveTargetFor<IndicatedTarget>(obj);
        }
        public void LateUpdate()
        {
            _catcher.MousePosition = _input.MousePosition;
            UpdateRingCatcher();
            //ShowAllWaitingForSelectObjs();
        }
        void UpdateRingCatcher()
        {
            _ringCatcher.MousePosition = _input.MousePosition;
            var pixelSize = _camera.pixelRect.size;
            _ringCatcher.RingRadius = Mathf.Max(pixelSize.x, pixelSize.y) * _catcher.Radius;
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
