using System;
using System.IO;
using UnityEditor;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
#if UNITY_EDITOR
    public class JsonAssetSaver<T> : AssetSaverBase<T>
    {
        TextAssetSaver _saver;
        public override string SavePath
        {
            get => _saver.SavePath;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                var p = value;
                if (Path.GetExtension(p) != "json")
                    p = Path.ChangeExtension(p, "json");
                _saver.SavePath = p;
            }
        }
        public override string ABPath
        {
            get => _saver.ABPath;
            set => _saver.ABPath = value;
        }
        public JsonAssetSaver()
        {
            _saver = new();
        }
        public override bool Save(T obj)
        {
            return SaveImpl(obj);
        }

        public override bool Save(object obj)
        {
            if (obj is T o)
                return SaveImpl(o);
            else
                throw new ArgumentNullException(nameof(obj));
        }
        protected bool SaveImpl(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            var text = EditorJsonUtility.ToJson(obj, true);
            return _saver.Save(text);
        }

    }
#endif
}
