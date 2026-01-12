using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TPhysics
{
    public interface IPhysicsModule
    {
        public Context Update(Context context);
    }
}
