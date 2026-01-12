namespace Tests.Weapons_New.Launcher
{
    internal class Idle : LauncherState
    {
        public Idle(Launcher launcher, string name, float duration = 0, bool enabled = true) : base(launcher, $"{name}_idle", duration, enabled) { }
    }
}
