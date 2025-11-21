using System;
using System.Collections.Generic;

namespace Tests.Interaction

{
    public interface ICatcher<T>
    {
        public bool Enabled { get; set; }
        public IReadOnlyList<T> CaughtItems { get; }
        public Action<List<T>> CaughtItemsChangedAction { get; set; }
        Action<T> ItemCaughtAction { get; set; }
        Action<T> ItemReleaseAction { get; set; }

        public void AddItem(T target);
        public void RemoveItem(T target);
    }
}
