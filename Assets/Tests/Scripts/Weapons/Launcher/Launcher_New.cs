using Tests.States;
using UnityEngine;

namespace Tests.Weapons.Launcher
{
    //UNDONE: 使用状态机重写发射器逻辑
    internal class LauncherState : WithCallbackPlayableState<object>
    {
        public LauncherState(string name, float duration = 0, bool enabled = true) : base($"launcher_{name}", duration, enabled)
        {
        }
    }
    internal class LauncherStatemachine : WithCallbackPlayableStatemachine<object>
    {
        public LauncherStatemachine(string name, bool enabled = true) : base($"launcher_{name}_statemachine", enabled)
        {
        }
    }
    internal class DelayLaunch : LauncherState
    {
        public DelayLaunch(string name, float duration = 0, bool enabled = true) : base($"{name}_delay_launch", duration, enabled)
        {
        }
    }
    internal class Launching : LauncherState
    {
        public Launching(string name, float duration = 0, bool enabled = true) : base($"{name}_launching", duration, enabled)
        {
        }
    }
    internal class Reload : LauncherState
    {
        public Reload(string name, float duration = 0, bool enabled = true) : base($"{name}_reload", duration, enabled)
        {
        }
    }

    internal class Launcher_New : MonoBehaviour
    {
        ILauncherDefinitions _definitions;
        private void Awake()
        {
            _definitions = GetComponent<ILauncherDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherDefinitions));
        }
    }
}
