using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tests.Interaction
{
    public class InteractableItem : IVolumetricInteractable
    {
        Guid _id;
        GameObject _obj;
        Func<Bounds> _boundsGetFunc;
        public InteractableItem(Guid id, GameObject obj, Func<Bounds> boundsGetFunc)
        {
            _id = id;
            _obj = obj;
            _boundsGetFunc = boundsGetFunc ?? throw new ArgumentNullException(nameof(boundsGetFunc));
        }
        public InteractableItem() : this(Guid.NewGuid(), null, default)
        {
        }
        public GameObject Obj => _obj;

        public Guid ID => _id;

        public Bounds Bounds => _boundsGetFunc();
    }
}
