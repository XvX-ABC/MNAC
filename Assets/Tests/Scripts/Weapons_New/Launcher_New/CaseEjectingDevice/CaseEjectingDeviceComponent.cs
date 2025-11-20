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
            blackboard.TryReadValueOrThrowException<ILauncher>(LauncherEffectComponent.OwnerLauncher, out var launcher);
            var device = GetComponent<CaseEjectingDevice>();
            device.Launcher = launcher;
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
