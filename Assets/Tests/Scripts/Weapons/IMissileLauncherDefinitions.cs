using UnityEngine;
using Tests.Weapons;
using System;

namespace Assets.Tests.Scripts.Weapons
{
    public interface IMissileLauncherDefinitions : ILauncherDefinitions
    {
        [Obsolete]
        public float LaunchDelay { get; }
        public Vector2 LaunchDelay_New { get; }
    }
}
