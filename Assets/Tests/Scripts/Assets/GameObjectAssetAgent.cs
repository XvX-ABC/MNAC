using System;
using System.IO;
using UnityEngine;

namespace Tests.Assets
{
    [Serializable]
    public class GameObjectAssetAgent : AssetAgentBase<GameObject>
    {
#if UNITY_EDITOR
        public GameObjectAssetAgent() : base(new PrefabAssetLoader(), new PrefabAssetSaver())
        {
        }
        public override void Save(GameObject obj)
        {
            _saver.BundleName = definitions.BundleName;
            _saver.Save(obj);
            definitions.DirPath = Path.GetDirectoryName(Path.GetRelativePath(Application.dataPath, _saver.SavePath));
            definitions.Name = obj.name;

        }
#else
        public GameObjectAssetAgent() : base(new GameObjectAssetLoader())
        {

        }
#endif

    }
}
