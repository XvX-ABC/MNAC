using UnityEngine;

namespace Tests.Characters.UI
{
    internal class IndicatedTarget : Tests.UI.IndicatedTarget
    {
        protected override void OnEnable()
        {
            indicatorType = Tests.UI.IndicatorType.Box;
            base.OnEnable();
        }
    }
}
