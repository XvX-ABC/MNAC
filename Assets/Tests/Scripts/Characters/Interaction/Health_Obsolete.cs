using System;
using Tests.Interaction;
using UnityEngine;
using HealthInfluence = Tests.Interaction.Influence.Health;
namespace Tests.Characters.Interaction
{
    [SerializeField]
    [Obsolete]
    public class Health_Obsolete : NumberBase, IHealth
    {
        private HealthInfluence influence;
        IHealthEffects_Obsolete _effects;
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

        public bool IsAlive => point > minPoint;

        public Health_Obsolete(float maxPoint, float minPoint, float point, HealthInfluence influence, IHealthEffects_Obsolete effects) : base(maxPoint, minPoint, point)
        {
            this._effects = effects ?? throw new ArgumentNullException(nameof(effects));

            _effects.MaxPoint = maxPoint;
            _effects.MinPoint = minPoint;
            Influence = influence;
        }
        public Health_Obsolete(float maxPoint, float point, IHealthEffects_Obsolete effects) : this(maxPoint, INumerical.MINPOINT, point, null, effects)
        {

        }
        public Health_Obsolete(float maxPoint, IHealthEffects_Obsolete effects) : this(maxPoint, maxPoint, effects)
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
