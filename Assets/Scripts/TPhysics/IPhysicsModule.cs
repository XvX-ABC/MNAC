using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNAC.TPhysics
{
    public interface IPhysicsModule
    {
        public Context Update(Context context);
    }
}
