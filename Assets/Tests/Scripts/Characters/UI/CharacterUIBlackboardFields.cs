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
            Character_Actor_Cursor_Indicator = UIBlackboardFields.Character_Actor_Cursor_Indicator;
        }
        public readonly static Guid Catcher_Ring;
        public readonly static Guid Blackboard_Main;
        public readonly static Guid Targets_Display;
        public readonly static Guid Character_Actor_Cursor_Indicator;
    }
}
