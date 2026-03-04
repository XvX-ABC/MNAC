using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MNAC.Utilities.Assets.DataControl
{
#if UNITY_EDITOR
    public class FileAssetSaver_Json<T>
    {
        FileInfo _fileInfo;
        string _abName;
        string _abVariant;
        public string FilePath
        {
            get => _fileInfo?.FullName ?? null;
            set
            {
                _fileInfo = new FileInfo(value);
            }
        }

        public string ABName { get => _abName; set => _abName = value; }
        public string ABVariant { get => _abVariant; set => _abVariant = value; }

        public FileAssetSaver_Json()
        {

        }
        public FileAssetSaver_Json(string filePath, string abName = null, string abVariant = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Can not find a file by the path '{filePath}'");
            FilePath = filePath;
            _abName = abName;
            _abVariant = abVariant;
        }
        public FileAssetSaver_Json(FileInfo fileInfo, string abName = null, string abVariant = null)
        {
            this._fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            _abName = abName;
            _abVariant = abVariant;
        }
        void WriteToFile(T data)
        {
            if (_fileInfo == null)
                throw new NullReferenceException(nameof(_fileInfo));
            using (var writer = new StreamWriter(_fileInfo.Open(FileMode.OpenOrCreate)))
            {
                var json = JsonUtility.ToJson(data, true);
                writer.Write(json);
            }
        }
        public void Save(T data)
        {
            try
            {
                WriteToFile(data);
            }
            catch (Exception)
            {
                throw;
            }

            AssetDatabase.Refresh();

            var filePath = _fileInfo.FullName;
            var importer = AssetsHelper.GetImporterBy(_fileInfo);
            if (_abName != null && _abName.Length > 0)
                importer.assetBundleName = _abName;
            if (_abVariant != null && _abVariant.Length > 0)
                importer.assetBundleVariant = _abVariant;
        }

    }
}
#endif