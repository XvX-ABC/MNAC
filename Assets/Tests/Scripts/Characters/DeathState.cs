using Cinemachine;
using System;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Timeline;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

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
        public DeathStateHelper(GameObject obj, Action<GameObject> deathAction, float duration = 0, bool enabled = true)
        {
            state = new DeathState(obj, deathAction, duration, enabled);
        }
    }
    internal class DeathState : CharacterBehaviourStateBase
    {
        GameObject _obj;
        Action<GameObject> _action;
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
                _locomotionCore = value;
            }
        }

        internal class DiedLocomotion : LocomotionModuleBase
        {
            StopLocomotion _locomotion;
            public DiedLocomotion()
            {
                _locomotion = new();
            }
            public override Context OnEnd(Context context)
            {
                return OnUpdate(context);
            }

            public override Context OnStart(Context context)
            {
                return OnUpdate(context);
            }

            public override Context OnUpdate(Context context)
            {
                var groundsDetector = context.GroundDetector;
                if (groundsDetector.Grounds.Count > 0)
                    return _locomotion.OnUpdate(context);
                return context;
            }
        }
        public DeathState(GameObject obj, Action<GameObject> deathAction, float duration = 0, bool enabled = true) : base("death", duration, enabled)
        {
            _obj = obj;
            _action = deathAction;
            _diedLocomotion = new();
            this.timeline.EndAction += DoDied;
        }
        void DoDied(TimelineContext _)
        {
            if (_obj != null)
                _action?.Invoke(_obj);
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
