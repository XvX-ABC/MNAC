using Tests.States;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    internal class TargetLockerStatemachine : WithCallbackPlayableStatemachine<object>
    {
        public TargetLockerStatemachine(string name = "launcher_target_locker_statemachine", bool enabled = true) : base(name, enabled)
        {
        }
    }
}
