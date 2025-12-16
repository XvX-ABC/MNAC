using System;

namespace Tests.Interaction.Influence
{
    [Obsolete]
    public class Health_Obsolete : InfluenceBase
    {
        float _minPoint;
        float _point;
        private float _maxPoint;

        public float MaxPoint { get => _maxPoint; set => _maxPoint = value; }
        public float MinPoint { get => _minPoint; set => _minPoint = value; }
        public float Point
        {
            get => _point;
            set => _point = value;
        }
        public override string Name => "health";
        public bool IsAlive { get => _point > _minPoint; }
    }
}
