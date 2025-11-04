using System;
using System.Linq;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Characters.Interaction
{
    public class CharacterBase : MonoBehaviour, ICharacter
    {
        Guid _id;
        InteractableItem _item;
        public CharacterBase()
        {
            _id = Guid.NewGuid();

        }
        protected virtual void OnEnable()
        {
            TryRegisterToInteractionManager();
        }
        protected virtual void OnDisable()
        {
            UnregisterFromInteractionManager();
        }
        void TryRegisterToInteractionManager()
        {
            var attrs = this.GetType().GetCustomAttributes(true);
            var a = attrs.FirstOrDefault(a => a is InteractableAttribute);
            if (a != null)
            {
                _item = new InteractableItem(this.ID, this.gameObject);
                InteractionManager.AddItem(_item);
            }
        }
        void UnregisterFromInteractionManager()
        {
            InteractionManager.RemoveItem(_item);
        }
        public Guid ID => _id;

        public string Name => this.name;
    }
}
