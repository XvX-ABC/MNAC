//#define EDITOR_ASSET_LOAD
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Tests.Obsolete_Assets
{
#if UNITY_EDITOR
    public class PrefabAssetSaver : AssetSaverBase<GameObject>
    {
        public override string SavePath
        {
            get => savePath;
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentException("The save path can't assign a empty value.");
                var p = value;
                if (Path.GetExtension(p) != "prefab")
                    p = Path.ChangeExtension(p, "prefab");
                savePath = p;
            }
        }
        public override bool Save(GameObject obj)
        {
            return SaveImpl(obj);
        }

        public override bool Save(object obj)
        {
            return SaveImpl(obj as GameObject);
        }
        protected bool SaveImpl(GameObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            var filePath = AssetDatabase.GetAssetPath(obj);
            savePath = filePath;
            //var path = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, savePath));
            var path = savePath;
            var importer = AssetImporter.GetAtPath(path);
            importer.assetBundleName = bundleName;
            if (bundleName.Length > 0)
                importer.assetBundleVariant = "";
            return true;
        }
    }
#endif
}
