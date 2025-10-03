using Tests.Interaction;
using UnityEngine;
using HealthInfluence = Tests.Interaction.Influence.Health;
namespace Tests.Characters.Interaction
{
    public class Health : NumberBase, IHealth
    {
        internal HealthInfluence influence;
        protected override float point
        {
            get => base.point;
            set
            {
                influence.Point = value;
                base.point = value;
            }
        }
        public Health(float maxPoint, float point, HealthInfluence influence) : base(maxPoint, point)
        {
            this.influence = influence;
            if (this.influence != null)
            {
                this.influence.MaxPoint = maxPoint;
                this.influence.MinPoint = INumerical.MINPOINT;
                this.influence.Point = point;
            }
        }
        public Health(float maxPoint, HealthInfluence influence) : this(maxPoint, maxPoint, influence)
        {

        }
        public Health(float maxPoint) : this(maxPoint, null)
        {

        }
    }
}
