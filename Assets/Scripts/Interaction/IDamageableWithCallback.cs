using System;
using UnityEngine;

namespace MNAC.Interaction
{
    public interface IDamageableWithCallback : IDamageable
    {
        public new IHealthWithCallBack HP { get; }
        public Action<GameObject> DiedAction { get; set; }
    }
}
