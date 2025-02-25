using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class MissileLauncherDefinitions : LauncherDefinitions, IMissileLauncherDefinitions
    {
        [SerializeField]
        float _launchDelay;
        [SerializeField]
        Vector2 _launchDelay_New;

        public float LaunchDelay { get => _launchDelay; }
        public Vector2 LaunchDelay_New { get => _launchDelay_New; }
    }
}
