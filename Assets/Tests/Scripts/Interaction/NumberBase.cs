using UnityEngine;

namespace Tests.Interaction
{
    public class NumberBase : INumerical
    {
        protected float minPoint;
        protected float maxPoint;
        float _point;
        public NumberBase(float maxPoint, float minPoint, float point)
        {
            this.maxPoint = maxPoint;
            this.minPoint = minPoint;
            _point = Mathf.Clamp(point, minPoint, maxPoint);
        }
        public NumberBase(float maxPoint, float point) : this(maxPoint, INumerical.MINPOINT, point)
        {

        }
        public NumberBase(float maxPoint) : this(maxPoint, INumerical.MINPOINT, maxPoint) { }

        public float MaxPoint => maxPoint;

        public float Point => _point;
        protected virtual float point
        {
            get => _point;
            set => _point = Mathf.Clamp(value, minPoint, maxPoint);
        }

        public float MinPoint => minPoint;

        public virtual void ReceivePoint(float point)
        {
            var p = this.point + point;
            this.point = Mathf.Clamp(p, minPoint, maxPoint);
        }
    }
}
