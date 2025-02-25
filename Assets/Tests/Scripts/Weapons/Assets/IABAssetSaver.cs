using Assets.Scripts.Utilities.Assets;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Assets.Tests.Scripts.Weapons.Assets
{
#if UNITY_EDITOR

    public interface IABAssetSaver
    {
        public void Save();
    }
    public interface IABAssetSaver<T> : IABAssetSaver
    {
        public Func<T> GetFunc { get; set; }
        public void Save(T asset);
    }
    public abstract class AssetBindSaverBase<T> : IABAssetSaver<T>
    {
        Func<T> _getFunc;
        IABAssetSaver<T> _assetSaver;

        public Func<T> GetFunc { get => _getFunc; set => _getFunc = value; }

        protected AssetBindSaverBase(string assetPath, Func<T> getFunc)
        {
            _assetSaver = NewAssetSaver(assetPath);
            _getFunc = getFunc;
        }
        public AssetBindSaverBase(IABAssetAgent assetAgent, Func<T> getFunc) : this(Path.Combine(assetAgent.BundlePath, assetAgent.ResourceName), getFunc)
        {
        }
        protected abstract IABAssetSaver<T> NewAssetSaver(string assetPath);
        public void Save()
        {
            var t = _getFunc();
            _assetSaver.Save(t);
        }
        public void Save(T asset)
        {

        }
    }
    public abstract class AssetSaverBase<T> : IABAssetSaver<T>
    {
        protected string assetPath;
        protected string assetSavePath;

        public Func<T> GetFunc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AssetSaverBase(string assetPath)
        {
            this.assetPath = assetPath ?? throw new ArgumentNullException(nameof(assetPath));
            this.assetSavePath = Path.Combine("Assets", assetPath);
        }
        public AssetSaverBase(IABAssetAgent assetAgent) : this(Path.Combine(assetAgent.BundlePath, assetAgent.ResourceName))
        {

        }
        protected void AddBundleTag()
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetSavePath);
            importer.assetBundleName = assetPath;
            importer.assetBundleVariant = "";
        }
        public abstract void Save(T asset);

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
    public class ObjectAssetSaver : AssetSaverBase<Object>
    {
        public ObjectAssetSaver(string assetPath) : base(assetPath)
        {
        }
        public ObjectAssetSaver(IABAssetAgent assetAgent) : base(assetAgent)
        {

        }

        public override void Save(Object asset)
        {
            assetSavePath = AssetDatabase.GetAssetPath(asset);
            AddBundleTag();
        }
    }
    public class ObjectAssetBindSaver : AssetBindSaverBase<Object>
    {
        public ObjectAssetBindSaver(string assetPath, Func<Object> getFunc) : base(assetPath, getFunc)
        {
        }

        public ObjectAssetBindSaver(IABAssetAgent assetAgent, Func<Object> getFunc) : base(assetAgent, getFunc)
        {
        }

        protected override IABAssetSaver<Object> NewAssetSaver(string assetPath)
        {
            return new ObjectAssetSaver(assetPath);
        }
    }
    public class TextAssetSaver : AssetSaverBase<string>
    {
        protected string filePath;
        public TextAssetSaver(string assetPath) : base(assetPath)
        {
            filePath = Path.Combine(Application.dataPath, assetPath);
        }
        public TextAssetSaver(IABAssetAgent assetAgent) : base(assetAgent)
        {
            filePath = Path.Combine(Application.dataPath, assetPath);
        }
        protected void WriteToFile(string text)
        {
            if (Directory.Exists(filePath))
                throw new FileNotFoundException($"The path '{filePath}' is a directory.");
            var d = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(d))
                Directory.CreateDirectory(d);
            using (var writer = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate)))
                writer.Write(text);
            Debug.Log("FilePath: " + filePath);
            AssetDatabase.Refresh();
        }
        public override void Save(string text)
        {
            WriteToFile(text);
            AddBundleTag();
        }
    }

    public class JsonAssetSaver : TextAssetSaver, IABAssetSaver<object>
    {
        public JsonAssetSaver(string assetPath) : base(assetPath)
        {
            filePath = Path.ChangeExtension(filePath, ".json");
            assetSavePath = Path.ChangeExtension(assetSavePath, ".json");
        }
        public JsonAssetSaver(IABAssetAgent assetAgent) : base(assetAgent)
        {
            filePath = Path.ChangeExtension(filePath, ".json");
            assetSavePath = Path.ChangeExtension(assetSavePath, ".json");
        }

        Func<object> IABAssetSaver<object>.GetFunc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Save(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            var text = EditorJsonUtility.ToJson(obj, true);
            if (text.Length > 0)
                base.Save(text);

        }
    }
    public class JsonAssetBindSaver : AssetBindSaverBase<object>
    {
        public JsonAssetBindSaver(string assetPath, Func<object> getFunc) : base(assetPath, getFunc)
        {
        }

        public JsonAssetBindSaver(IABAssetAgent assetAgent, Func<object> getFunc) : base(assetAgent, getFunc)
        {
        }

        protected override IABAssetSaver<object> NewAssetSaver(string assetPath)
        {
            return new JsonAssetSaver(assetPath);
        }
    }
#endif
}
