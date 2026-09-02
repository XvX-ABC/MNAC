using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Weapons.Projectiles;
using UnityEngine;

namespace MNAC.Weapons.Projectiles
{
    public interface IBullet : IProjectile
    {
        public Ray ShootingRay { get; set; }
    }
}
