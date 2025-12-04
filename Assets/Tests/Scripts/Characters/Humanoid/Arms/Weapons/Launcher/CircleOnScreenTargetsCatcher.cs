using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tests.Behaviours.Input;
using Tests.Interaction;
using Tests.UI;
using Tests.Utilities.Composable;
using UnityEngine;
using IndicatedTarget = Tests.Characters.UI.IndicatedTarget;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    //public class LauncherLocker
    //{
    //    CircleOnScreenTargetsCatcher _circleCatcher;
    //    IGameObjTarget _mainTarget;
    //    public LauncherLocker()
    //    {
    //        _circleCatcher.TargetsChangedAction += WhenTargetsChanged;
    //    }
    //    void WhenTargetsChanged(List<IGameObjTarget> targets)
    //    {
    //        targets.Sort((a, b) =>
    //        {
    //            return 0;
    //        });
    //    }

    //}
    public partial class CircleOnScreenTargetsCatcher : ComponentBase, ITargetsCatcher_New<IGameObjTarget_New>
    {
        GameObjsInScreenCatcher_Obsolete _screenCatcher;
        Tests.Interaction.CircleOnScreenTargetsCatcher _catcher;
        RingCatcher _ringCatcher;
        [Obsolete]
        TargetsDisplay _targetDisplay;
        IndicatorsManager _indicatorsManager;
        Camera _camera;
        IBaseInput _input;
        Action<IList<Tests.Interaction.ITarget_Obsolete>> _targetsChangedAction;

        ICircleOnScreenTargetsCatcherDefinitions _definitions;

        GameObject _actorObj;
        [Obsolete]
        public CircleOnScreenTargetsCatcher(ICircleOnScreenTargetsCatcherDefinitions definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }
        [Obsolete]
        public CircleOnScreenTargetsCatcher(Camera camera, IBaseInput input, GameObject actorObj, RingCatcher ringCatcher, TargetsDisplay targetsDisplay, ICircleOnScreenTargetsCatcherDefinitions definitions, bool enabled = true)
        {
            _screenCatcher = new(camera, definitions.FilterAmountOneFrame);
            _catcher = new(_screenCatcher.ObjsInScreen, actorObj, camera, definitions.TargetsMask, definitions.FilterAmountOneFrame);
            _ringCatcher = ringCatcher ?? throw new ArgumentNullException(nameof(ringCatcher));
            _actorObj = actorObj ?? throw new ArgumentNullException(nameof(actorObj));
            _targetDisplay = targetsDisplay ?? throw new ArgumentNullException(nameof(targetsDisplay));
            _ringCatcher.Camera = camera;

            _input = input ?? throw new ArgumentNullException(nameof(input));

            _camera = camera;

            Enabled = enabled;

            _catcher.CaughtItemsChangedAction += targets =>
            {
                _targetsChangedAction?.Invoke(targets.Cast<Tests.Interaction.ITarget_Obsolete>().ToList());
            };
        }

        public CircleOnScreenTargetsCatcher(Camera camera, IBaseInput input, GameObject actorObj, RingCatcher ringCatcher, IndicatorsManager indicatorsManager, ICircleOnScreenTargetsCatcherDefinitions definitions, bool enabled = true)
        {
            _screenCatcher = new(camera, definitions.FilterAmountOneFrame);
            _catcher = new(_screenCatcher.ObjsInScreen, actorObj, camera, definitions.TargetsMask, definitions.FilterAmountOneFrame);
            _ringCatcher = ringCatcher ?? throw new ArgumentNullException(nameof(ringCatcher));
            _actorObj = actorObj ?? throw new ArgumentNullException(nameof(actorObj));
            _ringCatcher.Camera = camera;
            _indicatorsManager = indicatorsManager ?? throw new ArgumentNullException(nameof(indicatorsManager));
            _input = input ?? throw new ArgumentNullException(nameof(input));

            _camera = camera;

            Enabled = enabled;

            _catcher.CaughtItemsChangedAction += targets =>
            {
                _targetsChangedAction?.Invoke(targets.Cast<Tests.Interaction.ITarget_Obsolete>().ToList());
            };

            _catcher.Radius = definitions.CatchingViewPortRadius;
            _catcher.ItemCaughtAction += WhenTargetCaught;
            _catcher.ItemReleaseAction += WhenTargetRelease;
        }

        public override string Name => "launcher_targets_catcher_v0";

        public IReadOnlyList<IGameObjTarget_New> CaughtItems => _catcher.CaughtItems;

        public Action<List<IGameObjTarget_New>> CaughtItemsChangedAction { get => _catcher.CaughtItemsChangedAction; set => _catcher.CaughtItemsChangedAction = value; }
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
                _screenCatcher.Enabled = value;
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

        public Action<IGameObjTarget_New> ItemCaughtAction { get => _catcher.ItemCaughtAction; set => _catcher.ItemCaughtAction = value; }
        public Action<IGameObjTarget_New> ItemReleaseAction { get => _catcher.ItemReleaseAction; set => _catcher.ItemReleaseAction = value; }

        void WhenTargetCaught(IGameObjTarget_New target)
        {
            var obj = target.Obj;
            _indicatorsManager.AddTargetFor<IndicatedTarget>(obj);
        }
        void WhenTargetRelease(IGameObjTarget_New target)
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
            _ringCatcher.CursorPosition = _input.MousePosition;
            var pixelSize = _camera.pixelRect.size;
            _ringCatcher.RingRadius = Mathf.Max(pixelSize.x, pixelSize.y) * _catcher.Radius;
        }
        public IEnumerator FilterUpdateWithCoroutine()
        {
            return _screenCatcher.Update();
        }
        public IEnumerator CatcherUpdateWithCoroutine()
        {
            return _catcher.UpdateWithCoroutine();
        }

        public void AddItem(IGameObjTarget_New target)
        {
            _catcher.AddItem(target);
        }

        public void RemoveItem(IGameObjTarget_New target)
        {
            _catcher.RemoveItem(target);
        }
    }
}
