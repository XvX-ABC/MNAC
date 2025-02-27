using UnityEngine;
using Tests.Weapons;
using System;

namespace Assets.Tests.Scripts.Weapons
{
    public interface IMissileLauncherDefinitionsEditor : ILauncherDefinitionsEditor
    {
        public float LaunchDurationTime { get; set; }
        public Vector2 LaunchDelayRange { get; set; }
    }
    public interface IMissileLauncherDefinitions : ILauncherDefinitions
    {
    }
}
