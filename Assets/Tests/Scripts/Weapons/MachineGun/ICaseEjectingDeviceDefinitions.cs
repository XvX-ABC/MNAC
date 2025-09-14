using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    public interface ICaseEjectingDeviceDefinitions : ILauncherDefinitions
    {
        public GameObject CaseOrigin { get; }
    }
}
