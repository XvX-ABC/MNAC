using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    internal class IndicatedTarget : UI.IndicatedTarget
    {
        protected override void OnEnable()
        {
            indicatorType = UI.IndicatorType.Box;
            base.OnEnable();
        }
    }
}
