//#define EDITOR_ASSET_LOAD
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
    public class GameObjectAssetSaver : AssetSaverBase<GameObject>
    {
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
            var path = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, savePath));
            var importer = AssetImporter.GetAtPath(path);
            importer.assetBundleName = abPath;
            if (abPath.Length > 0)
                importer.assetBundleVariant = "";
            return true;
        }
    }
}
