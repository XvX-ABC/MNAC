using Tests.Weapons.Launcher;

namespace Tests.Weapons.MachineGuns
{
    public interface IMachineGun : ILauncher
    {
        public new IMachineGunDefinitions Definitions { get; }
    }
}
