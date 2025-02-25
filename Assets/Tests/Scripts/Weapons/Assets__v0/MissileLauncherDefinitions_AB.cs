using System;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons.Assets__0
{
    public class MissileLauncherDefinitions_AB : LauncherDefinitions_AB, IMissileLauncherDefinitions
    {
        [Serializable]
        protected new class Definitions : LauncherDefinitions_AB.Definitions
        {
            public Vector2 LaunchDelay_New;
        }
        Definitions definitions;
        public float LaunchDelay => throw new NotImplementedException();
        public Vector2 LaunchDelay_New
        {
            get
            {
                TryLoadDefinitions();
                return definitions.LaunchDelay_New;
            }
        }

    }
}
