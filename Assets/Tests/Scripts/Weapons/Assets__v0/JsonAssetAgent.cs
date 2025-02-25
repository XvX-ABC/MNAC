using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
    [Serializable]
    public class JsonAssetAgent<T> : AssetAgentBase<T>
    {
#if UNITY_EDITOR
        JsonAssetAgent() : base(null, null)
        {
        }

        public JsonAssetAgent();
#else
        public JsonAssetAgent() : base(new JsonAssetLoader<T>())
        {
            _loader = new();
        }
#endif

    }
}
