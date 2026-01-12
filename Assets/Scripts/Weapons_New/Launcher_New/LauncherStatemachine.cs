using Tests.States;

namespace Tests.Weapons_New.Launcher
{
    internal class LauncherStatemachine : WithCallbackPlayableStatemachine<object>
    {
        public LauncherStatemachine(string name, bool enabled = true) : base($"launcher_{name}_statemachine", enabled)
        {
        }
    }
}
