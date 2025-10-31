using System;
using System.IO;
using UnityEngine;

namespace Tests.Assets
{
    [Serializable]
    public class PrefabAssetAgent : AssetAgentBase<GameObject>
    {
#if UNITY_EDITOR
        public PrefabAssetAgent() : base(new PrefabAssetLoader(), new PrefabAssetSaver())
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
    [Serializable]
    public class PrefabAssetAgent_Managed : PrefabAssetAgent, IAssetAgent_Managed, IAssetAgent_Managed<GameObject>
    {
        [SerializeField]
        GameObject _asset;
        object IAssetAgent_Managed.Asset { get => _asset; set => _asset = (GameObject)value; }
        public GameObject Asset { get => _asset; set => _asset = value; }
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
