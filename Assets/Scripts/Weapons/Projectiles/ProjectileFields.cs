using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNAC.Weapons.Projectiles
{
    internal static class ProjectileFields
    {
        static ProjectileFields()
        {
            Hit_LayerMask = Guid.NewGuid();
            TeamMask = Guid.NewGuid();
        }
        public readonly static Guid Hit_LayerMask;
        internal static readonly Guid TeamMask;
    }
}
