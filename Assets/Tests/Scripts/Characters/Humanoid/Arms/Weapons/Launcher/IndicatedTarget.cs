using Tests.UI;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
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
