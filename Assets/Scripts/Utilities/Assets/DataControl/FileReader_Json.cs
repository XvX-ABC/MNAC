using System;
using System.IO;
using UnityEngine;

namespace MNAC.Utilities.Assets.DataControl
{
    public class FileAssetLoader_Json<T> : IDisposable
    {
        string _abName;
        string _abVariant;
        string _resName;
        public string ABName { get => _abName; set => _abName = value; }
        public string ABVariant { get => _abVariant; set => _abVariant = value; }
        public string ResName { get => _resName; set => _resName = value; }
        public FileAssetLoader_Json(string abName = null, string abVariant = null, string resName = null)
        {
            this._abName = abName;
            this._abVariant = abVariant;
            this._resName = resName;
        }

#if UNITY_EDITOR && !ASSETS_DEBUG_ENABLE_AB
        FileInfo _fileInfo;
        TextReader _reader;
        public FileAssetLoader_Json(FileInfo fileInfo, string abName = null, string abVariant = null)
        {
            _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            this._abName = abName;
            this._abVariant = abVariant;
            this._resName = Path.GetFileName(_fileInfo.FullName);
        }
        public string FilePath
        {
            get => _fileInfo?.FullName ?? null;
            set
            {
                if (!File.Exists(value))
                    throw new FileNotFoundException($"Can not find a file by the path '{value}'");
                _fileInfo = new FileInfo(value);
            }
        }


        T LoadFromFile()
        {
            if (_fileInfo == null)
                throw new ArgumentNullException(nameof(_fileInfo));
            _reader = new StreamReader(_fileInfo.OpenRead());
            var json = _reader.ReadToEnd();
            return JsonUtility.FromJson<T>(json);
        }
#endif
        bool IsEmptyString(string str)
        {
            return str == null || str.Length == 0;
        }
        T LoadFromAB()
        {
            var loader = LoadAssetBundleManager.Instance ?? throw new NullReferenceException();
            if (IsEmptyString(_abName) || IsEmptyString(_resName))
                return default;
            var res = default(TextAsset);
            if (IsEmptyString(_abVariant))
                res = loader.LoadResource<TextAsset>($"{_abName}", _resName);
            else
                res = loader.LoadResource<TextAsset>($"{_abName}.{_abVariant}", _resName);
            var json = res.text;
            return JsonUtility.FromJson<T>(json);
        }
        public T Load()
        {
#if UNITY_EDITOR  && !ASSETS_DEBUG_ENABLE_AB
            return LoadFromFile();
#else
            return ReadFromAB();
#endif
        }
        public void Dispose()
        {
#if UNITY_EDITOR && !ASSETS_DEBUG_ENABLE_AB
            _reader.Close();
#endif
        }
    }
}
