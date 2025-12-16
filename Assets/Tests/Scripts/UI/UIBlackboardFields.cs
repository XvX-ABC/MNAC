using System;

namespace Tests.UI
{
    public static class UIBlackboardFields
    {
        static UIBlackboardFields()
        {

            Input = Guid.NewGuid();
            Camera_Main = Guid.NewGuid();
            Catcher_Ring = Guid.NewGuid();
            Targets_Display = Guid.NewGuid();
            Indicators_Manager = Guid.NewGuid();
            Cursor_Indicator = Guid.NewGuid();
            Weapons_Text_Grid = Guid.NewGuid();
            Health_Bar = Guid.NewGuid();
        }
        public static readonly Guid Input;
        public static readonly Guid Camera_Main;
        public static readonly Guid Catcher_Ring;
        public static readonly Guid Targets_Display;
        public static readonly Guid Indicators_Manager;
        public static readonly Guid Cursor_Indicator;
        public static readonly Guid Weapons_Text_Grid;
        internal static readonly Guid Health_Bar;
    }
}
