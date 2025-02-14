using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class MissileLauncherDefines : LauncherDefines, IMissileLauncherDefines
    {
        [SerializeField]
        float _launchDelay;
        [SerializeField]
        Vector2 _launchDelay_New;

        public float LaunchDelay { get => _launchDelay; }
        public Vector2 LaunchDelay_New { get => _launchDelay_New; }

    }
}
