using Minimalist.Bar;
using Minimalist.Quantity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.UI;
using UnityEngine;

namespace MNAC.Characters.UI
{
    [RequireComponent(typeof(QuantityBhv))]
    public class ProgressSlider_QuantityBhv : ProgressSlider
    {
        [SerializeField]
        BarBhv _barBhv;
        QuantityBhv _bhv;
        public override Color Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override float Value { get => _bhv.FillAmount; set => _bhv.FillAmount = value; }
        public float MaxAmount { get => _bhv.MaximumAmount; set => _bhv.MaximumAmount = value; }
        public float MinAmount { get => _bhv.MinimumAmount; set => _bhv.MinimumAmount = value; }
        public float CurrentAmount { get => _bhv.Amount; set => _bhv.Amount = value; }
        protected override void Awake()
        {
            _bhv = GetComponent<QuantityBhv>();
        }
        protected override void ApplyMode(ProgressSliderMode mode)
        {
            throw new NotImplementedException();
        }
        private void OnEnable()
        {
            _barBhv.gameObject.SetActive(true);
        }
        private void OnDisable()
        {
            _barBhv.gameObject.SetActive(false);
        }
    }
}
