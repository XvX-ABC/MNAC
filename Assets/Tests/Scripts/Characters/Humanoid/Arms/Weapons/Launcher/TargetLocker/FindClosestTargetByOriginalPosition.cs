using System.Collections.Generic;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    internal class FindClosestTargetByOriginalPosition : TargetLockerState
    {
        public FindClosestTargetByOriginalPosition(TargetLocker locker, string name, float duration = 0, bool enabled = true) : base(locker, name, duration, enabled)
        {
        }

        protected override void OnExecute(List<GameObject> caughtObjs)
        {
            var obj = locker.FindClosestObj(caughtObjs);
            if (obj != null)
                locker.MainTargetObj = obj;
        }
    }
}
