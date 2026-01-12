using System;
using Tests.Interaction.Influences;
using UnityEngine;

namespace Tests.Characters.Interaction.Influences
{
    internal class Knockback_Rbody : InfluenceBase, IKnockback
    {
        Rigidbody _rbody;
        public Knockback_Rbody(Rigidbody rbody)
        {
            _rbody = rbody ?? throw new ArgumentNullException(nameof(rbody));
        }
        public override string Name => "Knockback_rbody";

        public void ReceiveForce(Vector3 force, bool applyMass = false)
        {
            var mode = applyMass switch
            {
                true => ForceMode.Impulse,
                false => ForceMode.VelocityChange,
            };
            _rbody.AddForce(force, mode);
        }
    }
}
