using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Tests.Interaction
{
    public abstract class CatcherBase<T> : ICatcher<T>
    {
        protected internal List<T> caughtItems;
        protected internal Action<List<T>> caughtItemsChangedAction;
        protected internal Action<T> itemCatchAction;
        protected internal Action<T> itemReleaseAction;
        protected internal bool enabled;
        public CatcherBase()
        {
            caughtItems = NewTargetsContainer() ?? throw new NullReferenceException(nameof(caughtItems));
        }
        protected virtual List<T> NewTargetsContainer()
        {
            return new List<T>();
        }
        public IReadOnlyList<T> CaughtItems => caughtItems;

        public Action<List<T>> CaughtItemsChangedAction { get => caughtItemsChangedAction; set => caughtItemsChangedAction = value; }
        public virtual bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                if (!enabled)
                    CleanAll();
            }
        }

        public Action<T> ItemCaughtAction { get => itemCatchAction; set => itemCatchAction = value; }
        public Action<T> ItemReleaseAction { get => itemReleaseAction; set => itemReleaseAction = value; }

        internal protected virtual void AddItemImpl(T target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (caughtItems.Contains(target))
                return;
            caughtItems.Add(target);
            itemCatchAction?.Invoke(target);
            caughtItemsChangedAction?.Invoke(caughtItems);
        }
        internal protected virtual void RemoveItemImpl(T target)
        {
            //Debug.Log((target as GameObject).name);
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (caughtItems.Remove(target))
            {
                itemReleaseAction?.Invoke(target);
                caughtItemsChangedAction?.Invoke(caughtItems);
            }
        }
        internal protected virtual void CleanAll()
        {
            if (caughtItems.Count == 0)
                return;
            for (int i = 0; i < caughtItems.Count;)
            {
                var item = caughtItems[i];
                if (caughtItems.Remove(item) && item != null)
                    itemReleaseAction?.Invoke(item);
            }
            caughtItems.TrimExcess();
            caughtItems.Clear();
            caughtItemsChangedAction?.Invoke(caughtItems);
        }
        public virtual void Update()
        {

        }
        public virtual IEnumerator UpdateWithCoroutine()
        {
            yield return null;
        }
        public void TargetsToString(StringBuilder sb)
        {
            if (caughtItems != null)
                for (int i = 0; i < caughtItems.Count; i++)
                {
                    var t = caughtItems[i];
                    sb.AppendLine($"{i} -> " + t.ToString());
                }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine("enabled: " + enabled);
            TargetsToString(sb);
            return sb.ToString();
        }

        public void AddItem(T item)
        {
            AddItemImpl(item);
        }

        public void RemoveItem(T item)
        {
            RemoveItemImpl(item);
        }
    }
}
