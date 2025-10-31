using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.Assets
{
    [Serializable]
    public class JsonAssetAgent<T> : AssetAgentBase<T>
    {
#if UNITY_EDITOR
        protected JsonAssetAgent() : base(new JsonAssetLoader<T>(), new JsonAssetSaver<T>())
        {
        }

#else
        public JsonAssetAgent() : base(new JsonAssetLoader<T>())
        {
        }
#endif

    }

    [Serializable]
    public class JsonAssetAgent_Managed<T> : JsonAssetAgent<T>, IAssetAgent_Managed<T>, IAssetAgent_Managed
    {
        [SerializeField]
        T _asset;
        object IAssetAgent_Managed.Asset { get => _asset; set => _asset = (T)value; }
        public T Asset { get => _asset; set => _asset = value; }
        public JsonAssetAgent_Managed() : base()
        {
            var t = typeof(T);
            if (!t.IsSerializable)
                throw new Exception($"The type '{t.Name}' must be has 'serializable' attribute.");
        }
        public new void Load()
        {
            _asset = base.Load();
        }
        public void Save()
        {
            base.Save(_asset);
        }
    }
}
