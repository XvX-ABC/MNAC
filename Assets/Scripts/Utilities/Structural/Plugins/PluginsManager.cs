using MNAC.Utilities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MNAC.Utilities.Structural.Plugins
{
    public class PluginsManager<T> : ICollection<T>, IUpdatable where T : IPlugin
    {
        protected T[] plugins;

        public int Count => plugins.Length;

        public bool IsReadOnly => false;

        protected virtual void AddPluginToArray(T plugin)
        {
            if (plugins == null)
                plugins = new T[] { plugin };
            else
            {
                plugins = plugins.Append(plugin);
            }
        }
        protected virtual void RemovePluginFromArray(T plugin)
        {
            if (plugins.Length == 1)
                plugins = null;
            else
            {
                plugins.Remove(plugin);
            }
        }
        public virtual void Add(T plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));
            var index = Array.IndexOf(plugins, plugin);
            if (index > -1)
            {
                Debug.LogWarning($"The plugin {plugin} already exists in this plugin manager, please do not add it again.");
                return;
            }
            AddPluginToArray(plugin);
        }
        public virtual bool Remove(T plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));
            var index = Array.IndexOf(plugins, plugin);
            if (index == -1)
                return false;

            RemovePluginFromArray(plugin);
            return true;
        }
        public virtual void ActivatePlugin(T plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));
            var index = Array.IndexOf(plugins, plugin);
            if (index == -1)
            {
                Debug.LogWarning($"The plugin '{plugin}' has not been added to the plugin manager. Please add it first.");
                return;
            }
            plugins[index].Activated = true;
        }
        public virtual void DeactivatePlugin(T plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));
            var index = Array.IndexOf(plugins, plugin);
            if (index == -1)
            {
                Debug.LogWarning($"The plugin '{plugin}' has not been added to the plugin manager. Please add it first.");
                return;
            }
            plugins[index].Activated = false;
        }
        public virtual void Update()
        {
            if (plugins != null)
            {
                for (int i = 0; i < plugins.Length; i++)
                {
                    var plugin = plugins[i];
                    if (plugin.Activated)
                        plugin.Update();
                }
            }
        }

        public void Clear()
        {
            plugins = null;
        }

        public bool Contains(T item)
        {
            return plugins != null && Array.IndexOf(plugins, item) > -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (plugins != null)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index cannot be negative.");
                if (arrayIndex + Count > array.Length)
                    throw new ArgumentException("Destination array is not long enough to hold all the elements.");

                Array.Copy(plugins, 0, array, arrayIndex, Count);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    yield return plugin;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
