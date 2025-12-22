using DG.Tweening;
using System;
using System.Collections.Generic;
using Tests.Extensions;
using UnityEngine;

namespace Tests.Interaction
{
    public class FilterCollection_New<T>
    {
        ICaughtItemFilter<T>[] _filters;
        public void AddFilter(ICaughtItemFilter<T> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (_filters == null)
                _filters = new ICaughtItemFilter<T>[] { filter };
            else
                _filters = _filters.Append(filter);
        }
        public void RemoveFilter(ICaughtItemFilter<T> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (_filters.Length == 1)
                _filters = null;
            else
                _filters = _filters.Remove(filter);
        }
        public bool CanCatch(T item)
        {
            if (_filters == null)
                return true;
            foreach (var filter in _filters)
            {
                if (!filter.CanCatch(item))
                {
                    Debug.Log($"The check by filter '{filter.GetType().Name}' to item '{item}' was not pass succeed");
                    return false;
                }
            }
            return true;
        }
    }
    public class FilterCollection
    {
        ICaughtItemFilter<GameObject>[] _filters;
        public void AddFilter(ICaughtItemFilter<GameObject> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (_filters == null)
                _filters = new ICaughtItemFilter<GameObject>[] { filter };
            else
                _filters = _filters.Append(filter);
        }
        public void RemoveFilter(ICaughtItemFilter<GameObject> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (_filters.Length == 1)
                _filters = null;
            else
                _filters = _filters.Remove(filter);
        }
        public bool CanCatch(GameObject obj)
        {
            if (_filters == null)
                return true;
            foreach (var filter in _filters)
            {
                if (!filter.CanCatch(obj))
                    return false;
            }
            return true;
        }
    }
}
