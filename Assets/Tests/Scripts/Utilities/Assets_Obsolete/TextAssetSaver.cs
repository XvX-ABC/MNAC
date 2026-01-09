using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Tests.Obsolete_Assets
{
#if UNITY_EDITOR
    public class TextAssetSaver : AssetSaverBase<string>
    {
        static string s_basePath;
        static TextAssetSaver()
        {
            s_basePath = Application.dataPath;
        }
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
            WriteText(text);
            AddBundleTag();
            return true;
        }

        protected void WriteText(string text)
        {
            var path = Path.Join(s_basePath, savePath);
            if (Directory.Exists(path))
                throw new FileNotFoundException($"The path '{path}' is a directory.");
            var d = Path.GetDirectoryName(path);
            if (!Directory.Exists(d))
                Directory.CreateDirectory(d);
            using (var writer = new StreamWriter(File.Open(path, FileMode.OpenOrCreate)))
                writer.Write(text);
            AssetDatabase.Refresh();
        }
        protected virtual void AddBundleTag()
        {
            var path = Path.Join("Assets", savePath);
            AssetImporter importer = AssetImporter.GetAtPath(path);
            importer.assetBundleName = bundleName;
            if (bundleName.Length > 0)
                importer.assetBundleVariant = "";
        }
    }
#endif
}
