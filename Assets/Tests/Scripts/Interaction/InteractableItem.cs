using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tests.Interaction
{
    public class InteractableItem : IVolumetricInteractable
    {
        Guid _id;
        GameObject _obj;
        Bounds _bounds;
        public InteractableItem(Guid id, GameObject obj, Bounds bounds)
        {
            _id = id;
            _obj = obj;
            _bounds = bounds;
        }
        public InteractableItem() : this(Guid.NewGuid(), null, default)
        {
        }
        public GameObject Obj => _obj;

        public Guid ID => _id;

        public Bounds Bounds => _bounds;
    }
}
