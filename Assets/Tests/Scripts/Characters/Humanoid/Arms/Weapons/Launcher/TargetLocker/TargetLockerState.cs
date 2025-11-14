using System;
using System.Collections.Generic;
using Tests.States;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    internal abstract class TargetLockerState : WithCallbackPlayableState
    {
        StateLifeCycleWatcher<object> _lifeWatcher;
        protected TargetLocker locker;
        LifeCycle _lifeCycle { get => _lifeWatcher.CurrentLifeCycle; }
        public TargetLockerState(TargetLocker locker, string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
            this.locker = locker ?? throw new ArgumentNullException(nameof(locker));
            _lifeWatcher = new(this);
        }
        protected abstract void OnExecute(List<GameObject> caughtObjs);
        public void Execute(List<GameObject> caughtObjs)
        {
            if (_lifeCycle == LifeCycle.Ready || _lifeCycle == LifeCycle.Exited)
                return;
            OnExecute(caughtObjs);
        }
    }
}
