using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Tests.Utilities;
using UnityEngine;

namespace Tests.Interaction
{
    public class InteractionManager : Singleton<InteractionManager>
    {
        HashSet<IInteractable> _items;
        internal static HashSet<IInteractable> items { get => Instance._items; }
        public static int Count { get => Instance._items.Count; }

        public InteractionManager()
        {
            _items = new();
        }
        public override void Awake()
        {
            base.Awake();
        }
        public override string ToString()
        {
            return "Interaction Manager";
        }
        public static void AddItem(InteractableItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            items.Add(item);
        }
        public static void RemoveItem(InteractableItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            items.Remove(item);
        }
    }
}
