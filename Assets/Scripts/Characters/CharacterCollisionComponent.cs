using UnityEngine;

namespace MNAC.Characters.Humanoid
{
    internal class CharacterCollisionComponent : CharacterComponent
    {
        protected internal virtual void OnCollisionEnterImpl(Collision collision) { }
        protected internal virtual void OnCollisionExitImpl(Collision collision) { }
        protected internal virtual void OnCollisionStayImpl(Collision collision) { }
    }
}
