using PlasticPipe.PlasticProtocol.Messages.Serialization;
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
        HashSet<IInteractable> _waitingAddition;
        HashSet<IInteractable> _waitingRemoval;
        Action<IInteractable> _itemAddedAction;
        Action<IInteractable> _itemRemovedAction;
        internal static HashSet<IInteractable> items { get => Instance._items; }
        internal static HashSet<IInteractable> waitingAddition { get => Instance._waitingAddition; }
        internal static HashSet<IInteractable> waitingRemoval { get => Instance._waitingRemoval; }
        internal static Action<IInteractable> itemAddedAction { get => Instance._itemAddedAction; set => Instance._itemAddedAction = value; }
        internal static Action<IInteractable> itemRemovedAction { get => Instance._itemRemovedAction; set => Instance._itemRemovedAction = value; }
        public static int Count { get => Instance._items.Count; }

        public InteractionManager()
        {
            _items = new();
            _waitingAddition = new();
            _waitingRemoval = new();
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
            //items.Add(item);
            waitingAddition.Add(item);
        }
        public static void RemoveItem(InteractableItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            //items.Remove(item);
            waitingRemoval.Add(item);
        }
        public static void SynchronizeChanges()
        {
            foreach (var ai in waitingAddition)
            {
                items.Add(ai);
                itemAddedAction?.Invoke(ai);
            }
            waitingAddition.Clear();
            foreach (var ri in waitingRemoval)
            {
                items.Remove(ri);
                itemRemovedAction?.Invoke(ri);
            }
            waitingRemoval.Clear();
        }
    }
}
