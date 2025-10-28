using System;
using UnityEngine;

namespace Tests.Interaction
{
    public class InteractableItem : IInteractable
    {
        Guid _id;
        GameObject _obj;
        public InteractableItem(Guid id, GameObject obj)
        {
            _id = id;
            _obj = obj;
        }
        public InteractableItem() : this(Guid.NewGuid(), null)
        {
        }
        public GameObject Obj => _obj;

        public Guid ID => _id;
    }
}
