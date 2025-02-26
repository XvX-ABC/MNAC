using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
#if UNITY_EDITOR
    public class TextAssetSaver : AssetSaverBase<string>
    {
        public override bool Save(string text)
        {
            return SaveImpl(text);
        }

        public override bool Save(object obj)
        {
            return SaveImpl(obj as string);
        }
        protected virtual bool SaveImpl(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            WriteTextToFile(text);
            AddBundleTag();
            return true;
        }

        protected void WriteTextToFile(string text)
        {
            if (Directory.Exists(savePath))
                throw new FileNotFoundException($"The path '{savePath}' is a directory.");
            var d = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(d))
                Directory.CreateDirectory(d);
            using (var writer = new StreamWriter(File.Open(savePath, FileMode.OpenOrCreate)))
                writer.Write(text);
            AssetDatabase.Refresh();
        }
        protected virtual void AddBundleTag()
        {
            //var path = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, savePath));
            AssetImporter importer = AssetImporter.GetAtPath(savePath);
            importer.assetBundleName = Path.GetDirectoryName(abPath);
            if (abPath.Length > 0)
                importer.assetBundleVariant = "";
        }
    }
#endif
}
