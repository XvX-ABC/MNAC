using System;
using UnityEngine;

namespace Tests.Weapons
{
    public interface IProjectile
    {
        public GameObject Object { get; }
        public Action<IProjectile, GameObject> HitAction { get; set; }
    }
}