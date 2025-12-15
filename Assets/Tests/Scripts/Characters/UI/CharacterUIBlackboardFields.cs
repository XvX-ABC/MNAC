using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.UI;

namespace Tests.Characters.UI
{
    public static class CharacterUIBlackboardFields
    {
        static CharacterUIBlackboardFields()
        {
            Catcher_Ring = UIBlackboardFields.Catcher_Ring;
            Blackboard_Main = Guid.NewGuid();
            Targets_Display = UIBlackboardFields.Targets_Display;
            Player_Cursor_Indicator = UIBlackboardFields.Cursor_Indicator;
            Indicators_Manager = UIBlackboardFields.Indicators_Manager;
            Weapons_Text_Grid = UIBlackboardFields.Weapons_Text_Grid;
        }
        public readonly static Guid Catcher_Ring;
        public readonly static Guid Blackboard_Main;
        public readonly static Guid Targets_Display;
        public readonly static Guid Player_Cursor_Indicator;
        public readonly static Guid Indicators_Manager;
        public static readonly Guid Weapons_Text_Grid;
    }
}
