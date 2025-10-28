using Minimalist.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}
