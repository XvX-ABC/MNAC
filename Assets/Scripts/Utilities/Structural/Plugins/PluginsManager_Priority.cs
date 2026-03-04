using MNAC.Utilities.Extensions;
using System.Linq;

namespace MNAC.Utilities.Structural.Plugins
{
    public class PluginsManager_Priority<T> : PluginsManager<T> where T : IPlugin_Priority
    {
        int FindIndexByPriority(T[] plugins, int target)
        {
            int left = 0, right = plugins.Length;
            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (plugins[mid].Priority <= target)
                    left = mid + 1;
                else
                    right = mid;
            }
            return left;
        }
        protected override void AddPluginToArray(T plugin)
        {
            if (plugins == null)
            {
                plugins = new T[] { plugin };
            }
            else
            {
                var priority = plugin.Priority;
                var index = FindIndexByPriority(plugins, plugin.Priority);
                if (index == -1)
                {
                    plugins = plugins.Append(plugin);
                }
                else
                {
                    plugins = plugins.Insert(index, plugin);
                }
            }
        }
    }
}
