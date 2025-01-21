using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class MissileLauncherDefines : LauncherDefines, IMissileLauncherDefines
    {
        [SerializeField]
        float _launchDelay;

        public float LaunchDelay { get => _launchDelay; }
    }
}
