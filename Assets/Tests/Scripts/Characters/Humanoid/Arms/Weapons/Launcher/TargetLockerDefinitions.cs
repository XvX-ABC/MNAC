using System;
using Tests.Interaction;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{

    [Serializable]
    internal class TargetLockerDefinitions : ITargetLockerDefinitions
    {
        [SerializeField]
        ObstacleDetector _obstacleDetector;
        [SerializeField]
        ushort _handleAmountInCoroutine = 30;
        [SerializeField]
        float _catchAngle = 60;
        [SerializeField]
        float _targetChangedDuration = 0.2f;
        [SerializeField]
        float _receiveInputDuration = 0.05f;
        [SerializeField]
        bool _enabled = true;

        public ObstacleDetector ObstacleDetector { get => _obstacleDetector; }
        public ushort HandleAmountInCoroutine { get => _handleAmountInCoroutine; }
        public float CatchAngle { get => _catchAngle; }
        public float TargetChangedDuration { get => _targetChangedDuration; }
        public float ReceiveInputDuration { get => _receiveInputDuration; }
        public bool Enabled { get => _enabled; }
    }
}
