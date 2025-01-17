using System;
using UnityEngine;

namespace Tests.Weapons
{
    public interface IProjectile
    {
        //public GameObject Object { get; }
        public bool Enabled { get; set; }
        public Action<IProjectile, GameObject> HitAction { get; set; }
    }
}