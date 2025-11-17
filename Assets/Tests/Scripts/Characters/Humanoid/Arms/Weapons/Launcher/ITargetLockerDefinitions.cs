using Tests.Interaction;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    public interface ITargetLockerDefinitions
    {
        float CatchAngle { get; }
        bool Enabled { get; }
        ushort HandleAmountInCoroutine { get; }
        ObstacleDetector ObstacleDetector { get; }
        float ReceiveInputDuration { get; }
        float TargetChangedDuration { get; }
    }
}