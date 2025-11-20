using Tests.Weapons.Launcher;

namespace Tests.Weapons.MachineGuns
{
    public interface IMachineGun : ILauncher_Obsolete
    {
        public new IMachineGunDefinitions Definitions { get; }
    }
}
