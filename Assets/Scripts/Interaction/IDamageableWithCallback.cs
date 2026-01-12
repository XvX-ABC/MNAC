using System;
using UnityEngine;

namespace Tests.Interaction
{
    public interface IDamageableWithCallback : IDamageable
    {
        public new IHealthWithCallBack HP { get; }
        public Action<GameObject> DiedAction { get; set; }
    }
}
