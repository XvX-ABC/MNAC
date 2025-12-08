using System;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Weapons_New.Projectiles
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
