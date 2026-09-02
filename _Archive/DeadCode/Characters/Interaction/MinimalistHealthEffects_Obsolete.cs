using Minimalist.Quantity;
using System;
using MNAC.Interaction;
namespace MNAC.Characters.Interaction
{
    [Obsolete]
    public class MinimalistHealthEffects_Obsolete : IHealthEffects_Obsolete
    {
        QuantityBhv _bhv;
        public bool Enabled
        {
            get => _bhv.enabled;
            set
            {
                _bhv.enabled = value;
            }
        }
        public float MaxPoint { get => _bhv.MaximumAmount; set => _bhv.MaximumAmount = value; }
        public float MinPoint { get => _bhv.MinimumAmount; set => _bhv.MinimumAmount = value; }
        public float Point
        {
            get => _bhv.Amount;
            set
            {
                _bhv.Amount = value;
            }
        }

        public MinimalistHealthEffects_Obsolete(QuantityBhv quantityBhv, float maxPoint, float point)
        {
            _bhv = quantityBhv ?? throw new ArgumentNullException(nameof(quantityBhv));

            quantityBhv.MaximumAmount = maxPoint;
            quantityBhv.MinimumAmount = INumerical.MINPOINT;
            Point = point;
        }
        public MinimalistHealthEffects_Obsolete(QuantityBhv quantityBhv, float maxPoint) : this(quantityBhv, maxPoint, maxPoint)
        {

        }
    }
}
