using Tests.Weapons;

namespace Assets.Tests.Scripts.Weapons
{
    public interface IMissileLauncherDefines : ILauncherDefines
    {
        public float LaunchDelay { get; }
    }
}
