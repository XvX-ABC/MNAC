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
            Character_Actor_Cursor_Indicator = Guid.NewGuid();
        }
        public static readonly Guid Input;
        public static readonly Guid Camera_Main;
        public static readonly Guid Catcher_Ring;
        public static readonly Guid Targets_Display;
        public static readonly Guid Indicators_Manager;
        public static readonly Guid Character_Actor_Cursor_Indicator;
    }
}
