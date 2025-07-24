using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.BT;
using Tests.Input;

namespace Tests.Behaviours
{
    [Obsolete]
    public interface IBehaviour : ITask
    {
        public IInput Input { set; }
    }
}
