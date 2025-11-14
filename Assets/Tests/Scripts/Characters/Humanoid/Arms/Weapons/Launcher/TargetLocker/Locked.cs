using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    internal class Locked : TargetLockerState
    {
        public Locked(TargetLocker locker, string name, float duration = 0, bool enabled = true) : base(locker, name, duration, enabled)
        {
        }

        protected override void OnExecute(List<GameObject> caughtObjs)
        {
            throw new NotImplementedException();
        }
    }
}
