using Minimalist.Quantity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Characters
{
#if UNITY_EDITOR
    [RequireComponent(typeof(QuantityBhv))]
    internal class HealthDisplay_Debug : MonoBehaviour
    {
        QuantityBhv _bhv;
        IHealth _health;
        private void Awake()
        {
            _bhv = GetComponent<QuantityBhv>();
         
        }
        private void Start()
        {
            _health = GetComponent<IDamageable>().HP;
            _bhv.MinimumAmount = _health.MinPoint;
            _bhv.MaximumAmount = _health.MaxPoint;
        }
        private void Update()
        {
            _bhv.Amount = _health.Point;
        }

    }
#endif
}
