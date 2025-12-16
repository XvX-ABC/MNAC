using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Interaction.Influence;

namespace Tests.Interaction
{
    [Serializable]
    public class Health : NumberBase, IHealth, IInfluence
    {
        bool _enabled;
        IHealthEffector _healthEffector;

        public Health(float maxPoint, IHealthEffector healthEffector = null) : base(maxPoint)
        {
            this.healthEffector = healthEffector;
        }

        public Health(float maxPoint, float point, IHealthEffector healthEffector = null) : base(maxPoint, point)
        {

            this.healthEffector = healthEffector;
        }

        public Health(float maxPoint, float minPoint, float point, IHealthEffector healthEffector = null) : base(maxPoint, minPoint, point)
        {

            this.healthEffector = healthEffector;
        }

        public bool IsAlive => point > minPoint;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
            }
        }

        public string Name => "Health";

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

        public void Update()
        {
        }
    }
}
