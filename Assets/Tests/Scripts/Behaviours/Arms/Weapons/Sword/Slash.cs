using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Interaction;
using Tests.States;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    internal class SlashHelper
    {
        internal Slash state;
        internal ITarget target;
        public SlashHelper(LocomotionCore locomotionCore, IRotationLocker rotationLocker, float slashDuration)
        {
            state = new(locomotionCore, rotationLocker, slashDuration);
            state.EntryAction += () => target = null;
        }
        public virtual bool EntryEvent { get => target != null; }
        //public virtual bool EntryEvent
        //{
        //    get
        //    {
        //        var r = _target != null;
        //        return r;
        //    }
        //}
        public virtual bool ExitEvent { get => state.Timeline.NormalizedTime >= 1; }
        public ITarget Target
        {
            get => target;
            set => target = value;
        }
    }
    internal class Slash : ArmedArmStateBase
    {
        LocomotionCore _lcore;
        StopLocomotion _locomotion;
        IRotationLocker _rotationLocker;
        ISword _sword;

        public ISword Sword { get => _sword; set => _sword = value; }

        public Slash(LocomotionCore locomotionCore, IRotationLocker rotationLocker, string name, float duration) : base(name == null ? "slash" : $"slash_{name}", 0)
        {
            timeline = new Timeline_V1(duration);
            _rotationLocker = rotationLocker ?? throw new ArgumentNullException(nameof(rotationLocker));
            if (locomotionCore == null)
                throw new ArgumentNullException(nameof(locomotionCore));
            _locomotion = new();
            _lcore = locomotionCore;
            _lcore.AddModule(_locomotion);
        }
        public Slash(LocomotionCore locomotionCore, IRotationLocker rotationLocker, float duration) : this(locomotionCore, rotationLocker, null, duration) { }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _rotationLocker.Lock();
        }
        public override void OnEnter()
        {
            base.OnEnter();
            if (_sword != null)
                _sword.EnableDamage = true;
            timeline.Restart();
            _lcore.EnableModule(_locomotion);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            timeline.OnUpdate(Time.deltaTime);

        }
        public override void OnExit()
        {
            if (_sword != null)
                _sword.EnableDamage = false;
            timeline.End();
            _lcore.DisableModule(_locomotion);
            _rotationLocker.UnLock();
            base.OnExit();
        }
    }
}
