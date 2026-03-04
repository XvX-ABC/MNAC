using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.Weapons.Launcher
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
