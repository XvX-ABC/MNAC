using System;
using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Weapons.Projectiles
{
    public interface IProjectile
    {

        public Action<IProjectile> ActionStartCallback { get; set; }
        public Action<IProjectile> ActionEndCallback { get; set; }

        public GameObject Obj { get; }
        TeamMask TeamMask { get; set; }
        LayerMask LayerMaskToHit { get; set; }

        public void StartAction();
        public void EndAction();
    }
}
