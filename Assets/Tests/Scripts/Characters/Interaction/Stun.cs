using System;
using StunInfluence = Tests.Interaction.Influence.Stun;
namespace Tests.Characters.Interaction
{
    public class Stun : IStun
    {
        internal StunInfluence influence;

        public Stun(StunInfluence stun)
        {
            this.influence = stun ?? throw new ArgumentNullException(nameof(stun));
        }

        public bool Enabled => influence.Enabled;

        public void Begin(float length)
        {
            influence.DurationTime = length;
            influence.Enabled = true;
        }
        public void EndEarly()
        {
            influence.Enabled = false;
        }
    }
}
