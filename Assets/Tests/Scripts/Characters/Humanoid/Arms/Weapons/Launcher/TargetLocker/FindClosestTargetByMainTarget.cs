using System.Collections.Generic;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    internal class FindClosestTargetByMainTarget : TargetLockerState
    {
        public FindClosestTargetByMainTarget(TargetLocker locker, string name, float duration = 0, bool enabled = true) : base(locker, name, duration, enabled)
        {
        }

        protected override void OnExecute(List<GameObject> caughtObjs)
        {
            var obj = locker.FindClosestObjByMainObj(caughtObjs);
            if (obj != null)
                locker.MainTargetObj = obj;
        }
    }
}
