using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Utilities.Assets_New
{
    public interface IResourceLoader<T> : IDisposable
    {
        public T Resource { get; }
        public bool Load();
    }
}
