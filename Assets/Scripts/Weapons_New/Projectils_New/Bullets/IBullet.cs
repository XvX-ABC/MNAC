using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons_New.Projectiles;
using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    public interface IBullet : IProjectile
    {
        public Ray ShootingRay { get; set; }
    }
}
