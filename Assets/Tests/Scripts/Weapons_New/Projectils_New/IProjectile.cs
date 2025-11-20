using System;
using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    public interface IProjectile
    {
        public Action<IProjectile> ActionStartCallback { get; set; }
        public Action<IProjectile> ActionEndCallback { get; set; }

        public Action<IProjectile, GameObject> HitAction { get; set; }
        public GameObject Obj { get; }
        public void StartAction();
        public void EndAction();
    }
}
