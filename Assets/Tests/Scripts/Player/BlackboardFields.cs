using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.UI;

namespace Tests.Player
{
    internal class BlackboardFields
    {
        static BlackboardFields()
        {
            Camera_Main = Guid.NewGuid();
            Component_Input = Guid.NewGuid();
            Component_ScreenCatcher = Guid.NewGuid();
            Component_TargetLocker = Guid.NewGuid();
            UI_Cursor_Indicator = UIBlackboardFields.Character_Actor_Cursor_Indicator;
            UI_Indicators_Manager = UIBlackboardFields.Indicators_Manager;
        }
        public readonly static Guid Camera_Main;
        public readonly static Guid Component_Input;
        public readonly static Guid Component_ScreenCatcher;
        public readonly static Guid Component_TargetLocker;
        public readonly static Guid UI_Cursor_Indicator;
        public readonly static Guid UI_Indicators_Manager;
    }
}
