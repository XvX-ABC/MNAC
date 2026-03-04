using System;
using MNAC.Interaction;
using StunInfluence = MNAC.Interaction.Influence.Stun;
namespace MNAC.Characters.Interaction
{
    //TODO：Stun逻辑应该在Tests.Interaction中实现
    [Obsolete]
    public class Stun : IStun
    {
        private StunInfluence influence;

        public Stun(StunInfluence influence)
        {
            this.influence = influence ?? throw new ArgumentNullException(nameof(influence));
        }
        public Stun()
        {

        }

        public bool Enabled => influence?.Enabled ?? true;

        internal StunInfluence Influence { get => influence; set => influence = value; }

        public void Begin(float length)
        {
            if (influence == null)
                return;
            influence.DurationTime = length;
            influence.Enabled = true;
        }
        public void EndEarly()
        {
            if (influence == null)
                return;
            influence.Enabled = false;
        }
    }
}
