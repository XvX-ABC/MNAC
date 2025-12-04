using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    [RequireComponent(typeof(CaseEjectingDevice))]
    internal class CaseEjectingDeviceComponent : LauncherEffectComponent
    {
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            var device = GetComponent<CaseEjectingDevice>();
            device.Launcher = owner;
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
