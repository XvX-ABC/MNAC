using System;
using Tests.Interaction;
using HealthInfluence = Tests.Interaction.Influence.Health;
namespace Tests.Characters.Interaction
{
    public class Health : NumberBase, IHealth
    {
        private HealthInfluence influence;
        IHealthEffects _effects;
        bool _enabled;
        protected override float point
        {
            get => base.point;
            set
            {
                if (influence != null)
                    influence.Point = value;
                _effects.Point = value;
                base.point = value;
            }
        }

        internal HealthInfluence Influence
        {
            get => influence;
            set
            {
                if (value != null)
                {

                    value.MaxPoint = maxPoint;
                    value.MinPoint = minPoint;
                    value.Point = point;
                    value.Enabled = enabled;
                }
                influence = value;

            }
        }
        internal bool enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                _effects.Enabled = value;
                if (influence != null)
                    influence.Enabled = value;
            }
        }
        public Health(float maxPoint, float minPoint, float point, HealthInfluence influence, IHealthEffects effects) : base(maxPoint, minPoint, point)
        {
            this._effects = effects ?? throw new ArgumentNullException(nameof(effects));

            _effects.MaxPoint = maxPoint;
            _effects.MinPoint = minPoint;
            Influence = influence;
        }
        public Health(float maxPoint, float point, IHealthEffects effects) : this(maxPoint, INumerical.MINPOINT, point, null, effects)
        {

        }
        public Health(float maxPoint, IHealthEffects effects) : this(maxPoint, maxPoint, effects)
        {

        }
        public override void ReceivePoint(float point)
        {
            if (!enabled)
                return;
            base.ReceivePoint(point);
        }
    }
}
