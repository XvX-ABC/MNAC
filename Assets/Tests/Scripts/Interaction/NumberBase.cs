using UnityEngine;

namespace Tests.Interaction
{
    public class NumberBase : INumerical
    {
        protected float maxPoint;
        float _point;

        public NumberBase(float maxPoint, float point)
        {
            this.maxPoint = maxPoint;
            _point = Mathf.Clamp(point, INumerical.MINPOINT, maxPoint);
        }
        public NumberBase(float maxPoint) : this(maxPoint, maxPoint) { }

        public float MaxPoint => maxPoint;

        public float Point => _point;
        protected virtual float point
        {
            get => _point;
            set => _point = value;
        }

        public void ReceivePoint(float point)
        {
            point = Mathf.Clamp(point, INumerical.MINPOINT, maxPoint);
        }
    }
}
