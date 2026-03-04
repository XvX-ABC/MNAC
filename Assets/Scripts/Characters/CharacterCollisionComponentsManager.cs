using NUnit.Framework;
using System.Collections.Generic;
using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.Characters.Humanoid
{
    internal class CharacterCollisionComponentsManager : CharacterCollisionComponent
    {
        internal List<CharacterCollisionComponent> components = new();
        protected internal override void OnCollisionEnterImpl(Collision collision)
        {
            base.OnCollisionEnterImpl(collision);
            foreach (var comp in components)
            {
                if (comp == null)
                    continue;
                comp.OnCollisionEnterImpl(collision);
            }
        }

        protected internal override void OnCollisionStayImpl(Collision collision)
        {
            base.OnCollisionStayImpl(collision);
            foreach (var comp in components)
            {
                if (comp == null)
                    continue;
                comp.OnCollisionStayImpl(collision);
            }
        }

        protected internal override void OnCollisionExitImpl(Collision collision)
        {
            base.OnCollisionExitImpl(collision);
            foreach (var comp in components)
            {
                if (comp == null)
                    continue;
                comp.OnCollisionExitImpl(collision);
            }
        }
    }
}
