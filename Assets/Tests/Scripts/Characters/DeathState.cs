using System;
using Tests.Characters.Humanoid.Locomotion;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events.Point;
using UnityEngine;
using LocomotionCore = Tests.Characters.Humanoid.Locomotion.LocomotionCore;

namespace Tests.Characters
{
    internal class DeathStateHelper
    {
        internal DeathState state;
        public LocomotionCore LocomotionCore
        {
            get => state.LocomotionCore;
            set
            {
                state.LocomotionCore = value;
            }
        }
        public DeathStateHelper(GameObject obj, Action<GameObject> startAction, Action<GameObject> endAction, float duration = 0, bool enabled = true)
        {
            state = new DeathState(obj, startAction, endAction, duration, enabled);
        }
    }
    internal class DeathState : CharacterBehaviourStateBase
    {
        GameObject _obj;
        Action<GameObject> _startAction;
        Action<GameObject> _endAction;
        LocomotionCore _locomotionCore;
        DiedLocomotion _diedLocomotion;

        public LocomotionCore LocomotionCore
        {
            get => _locomotionCore;
            set
            {
                if (_locomotionCore != null)
                    _locomotionCore.RemoveModule(_diedLocomotion);
                if (value != null)
                    value.AddModule(_diedLocomotion);
                _diedLocomotion.LocomotionCore = value;
                _locomotionCore = value;
            }
        }

        internal class DiedLocomotion : LocomotionModuleBase
        {
            StopLocomotion _stopLocomotion;
            StopRotation _stopRotation;
            LocomotionCore _lcore;

            public DiedLocomotion()
            {
                _stopLocomotion = new();
                _stopRotation = new();
            }

            internal LocomotionCore LocomotionCore { get => _lcore; set => _lcore = value; }

            public override Context OnEnd(Context context)
            {
                var rbody = context.Rbody;
                rbody.drag = 0;
                return context;
            }

            public override Context OnStart(Context context)
            {
                return OnUpdate(context);
            }

            public override Context OnUpdate(Context context)
            {
                var groundsDetector = context.GroundDetector;
                if (groundsDetector.Grounds.Count > 0)
                {
                    if (_lcore != null)
                    {
                        var range = _lcore.definitions.MutativeDrag.Range;
                        var maxDrag = Mathf.Max(range.x, range.y);
                        context.Rbody.drag = maxDrag;
                    }
                    context = _stopLocomotion.OnUpdate(context);
                    return _stopRotation.OnUpdate(context);
                }
                return context;
            }
        }
        public DeathState(GameObject obj, Action<GameObject> startAction, Action<GameObject> endAction, float duration = 0, bool enabled = true) : base("death", duration, enabled)
        {
            _obj = obj;
            _startAction = startAction;
            _endAction = endAction;
            _diedLocomotion = new();
            this.timeline.AddPointEvent(0, _ => _startAction?.Invoke(_obj));
            this.timeline.AddPointEvent(1, DoDied);
        }
        void DoDied(TimelineContext _)
        {
            if (_obj != null)
                _endAction?.Invoke(_obj);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            _locomotionCore?.EnableModule(_diedLocomotion);
            timeline.Restart();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            timeline.OnUpdate(Time.deltaTime);
        }
        public override void OnExit()
        {
            timeline.End();
            _locomotionCore?.DisableModule(_diedLocomotion);
            base.OnExit();
        }
        ~DeathState()
        {
            _locomotionCore?.RemoveModule(_diedLocomotion);
        }
    }
}
