using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    internal class IndicatedTarget : UI.IndicatedTarget
    {
        public override Vector3 Position => transform.position;
        protected override void OnEnable()
        {
            indicatorType = UI.IndicatorType.Box;
            base.OnEnable();
        }
    }
}
