using Tests.Weapons;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    public class MissileLauncherDefinitions : LauncherDefinitions, IMissileLauncherDefinitions
    {
        [SerializeField]
        Vector2 _launchDelay_New;

        public Vector2 LaunchDelayRange { get => _launchDelay_New; }
    }
}
