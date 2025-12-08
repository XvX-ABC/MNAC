using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Interaction
{
    [Serializable]
    public class Health : NumberBase, IHealth
    {
        bool _enabled;
        Influence.Health _healthInfluence;
        IHealthEffector _healthEffector;

        public Health(float maxPoint, Influence.Health healthInfluence = null, IHealthEffector healthEffector = null) : base(maxPoint)
        {
            this.healthInfluence = healthInfluence;
            this.healthEffector = healthEffector;
        }

        public Health(float maxPoint, float point, Influence.Health healthInfluence = null, IHealthEffector healthEffector = null) : base(maxPoint, point)
        {
            this.healthInfluence = healthInfluence;
            this.healthEffector = healthEffector;
        }

        public Health(float maxPoint, float minPoint, float point, Influence.Health healthInfluence = null, IHealthEffector healthEffector = null) : base(maxPoint, minPoint, point)
        {
            this.healthInfluence = healthInfluence;
            this.healthEffector = healthEffector;
        }

        public bool IsAlive => point > minPoint;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_healthInfluence != null)
                    _healthInfluence.Enabled = value;
                if (_healthEffector != null)
                    _healthEffector.Enabled = value;
                _enabled = value;
            }
        }

        internal Influence.Health healthInfluence
        {
            get => _healthInfluence;
            set
            {
                _healthInfluence = value;
                if (_healthInfluence != null)
                {
                    _healthInfluence.MinPoint = minPoint;
                    _healthInfluence.MaxPoint = maxPoint;
                    _healthInfluence.Point = point;
                }
            }
        }
        internal IHealthEffector healthEffector
        {
            get => _healthEffector;
            set
            {
                _healthEffector = value;
                if (_healthEffector != null)
                {
                    _healthEffector.MinPoint = minPoint;
                    _healthEffector.MaxPoint = maxPoint;
                    _healthEffector.Point = point;
                }
            }
        }
    }
}
