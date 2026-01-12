using Tests.Interaction;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    [CreateAssetMenu(fileName = "TargetLockerDefinitions", menuName = "Tests/Definitions/Characters/Humanoid/Arms/Weapons/Launchers/TargetLockerDefinitions")]
    internal class TargetLockerDefinitions_SO : ScriptableObject, ITargetLockerDefinitions
    {
        [SerializeField]
        TargetLockerDefinitions _definitions;

        public float CatchAngle => _definitions.CatchAngle;

        public bool Enabled => _definitions.Enabled;

        public ushort HandleAmountInCoroutine => _definitions.HandleAmountInCoroutine;

        public ObstacleDetector ObstacleDetector => _definitions.ObstacleDetector;

        public float ReceiveInputDuration => _definitions.ReceiveInputDuration;

        public float TargetChangedDuration => _definitions.TargetChangedDuration;
    }
}
