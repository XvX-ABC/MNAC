using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Player
{
    internal class BlackboardFields
    {
        static BlackboardFields()
        {
            Camera_Main = Guid.NewGuid();
        }
        public readonly static Guid Camera_Main;
    }
}
