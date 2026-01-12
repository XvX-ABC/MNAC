
using UnityEngine;

namespace Tests.Interaction.Influences
{
    public interface IKnockback : IInfluence
    {
        public void ReceiveForce(Vector3 force, bool applyMass = false);
    }
}
