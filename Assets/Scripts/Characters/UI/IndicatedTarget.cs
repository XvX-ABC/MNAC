using UnityEngine;

namespace MNAC.Characters.UI
{
    internal class IndicatedTarget : MNAC.UI.IndicatedTarget
    {
        protected override void OnEnable()
        {
            indicatorType = MNAC.UI.IndicatorType.Box;
            base.OnEnable();
        }
    }
}
