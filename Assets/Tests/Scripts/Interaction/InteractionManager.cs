using PlasticPipe.PlasticProtocol.Messages.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Tests.Utilities;
using UnityEngine;
using UnityEngine.Analytics;

namespace Tests.Interaction
{
    public class InteractionManager : Singleton<InteractionManager>
    {
        HashSet<IInteractable> _items;
        HashSet<IInteractable> _waitingAddition;
        HashSet<IInteractable> _waitingRemoval;
        Action<IInteractable> _itemAddedAction;
        Action<IInteractable> _itemRemovedAction;
        List<Action<GameObject>> _actionList;
        List<Action<IInteractable>> _handlerList;
        Action _coroutineEndAction;
        internal static HashSet<IInteractable> items { get => Instance._items; }
        internal static HashSet<IInteractable> waitingAddition { get => Instance._waitingAddition; }
        internal static HashSet<IInteractable> waitingRemoval { get => Instance._waitingRemoval; }
        internal static Action<IInteractable> itemAddedAction { get => Instance._itemAddedAction; set => Instance._itemAddedAction = value; }
        internal static Action<IInteractable> itemRemovedAction { get => Instance._itemRemovedAction; set => Instance._itemRemovedAction = value; }
        public static int Count { get => Instance._items.Count; }
        public static List<Action<GameObject>> actionList { get => Instance._actionList; set => Instance._actionList = value; }
        public static List<Action<IInteractable>> handlerList { get => Instance._handlerList; set => Instance._handlerList = value; }
        public static Action CoroutineEndAction { get => Instance._coroutineEndAction; set => Instance._coroutineEndAction = value; }

        public InteractionManager()
        {
            _items = new();
            _waitingAddition = new();
            _waitingRemoval = new();
            _actionList = new();
            _handlerList = new();
        }
        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }
        private void OnEnable()
        {
            StartCoroutine(UpdateWithCoroutine());
        }
        private void OnDisable()
        {
            StopCoroutine(UpdateWithCoroutine());
        }
        public override string ToString()
        {
            return "Interaction Manager";
        }
        public static void AddItem(InteractableItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            waitingAddition.Add(item);
        }
        public static void RemoveItem(InteractableItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            waitingRemoval.Add(item);
        }
        IEnumerator UpdateWithCoroutine()
        {
            while (true)
            {
                var i = 0;
                SynchronizeChanges();
                foreach (var item in items)
                {
                    var obj = item.Obj;
                    for (int j = 0; j < _actionList.Count; j++)
                    {
                        var ac = _actionList[j];
                        if (ac == null)
                            continue;
                        ac(item.Obj);
                    }
                    for (int j = 0; j < _handlerList.Count; j++)
                    {
                        var h = _handlerList[j];
                        if (h == null)
                            continue;
                        h(item);
                    }
                    if (i <= 0 || i % 30 == 0)
                        yield return null;
                    i++;
                }
                _coroutineEndAction?.Invoke();
                yield return null;
            }
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
