using MNAC.Utilities.Assets.DataControl;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace MNAC.Utilities.Tests
{
    public class FileReadAndWrite_Json_Test
    {
        string _basePath;
        string _filePath;
        FileInfo _fileInfo;
        string _fileFullPath => Path.Join(_basePath, _filePath);
        string _abName;
        string _abVariant;
        Data _data;
        FileAssetSaver_Json<Data> _writer;
        FileAssetLoader_Json<Data> _reader;

        [Serializable]
        struct Data
        {
            [SerializeField]
            public int Field_0;
            [SerializeField]
            public string Field_1;
            [SerializeField]
            public float Field_2;
        }
        [SetUp]
        public void SetUp()
        {
            _basePath = Application.dataPath;
            _filePath = @"Scripts/Utilities/Tests/data_file_writer_test.json";
            _abName = @"test/file_writer_test";
            _abVariant = "v1";
            _data = new()
            {
                Field_0 = 1,
                Field_1 = "test",
                Field_2 = Mathf.PI,
            };
            _fileInfo = new FileInfo(_fileFullPath);
        }
        void DeleteFile()
        {
            if (_fileInfo != null && _fileInfo.Exists)
            {
                File.Delete(_fileInfo.FullName);
                AssetDatabase.Refresh();
            }
        }
        void SaveData()
        {
            _writer = new FileAssetSaver_Json<Data>(_fileInfo, _abName, _abVariant);
            try
            {
                _writer.Save(_data);

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
                DeleteFile();
            }
        }
        void CheckAssetBundleInfo()
        {
            var importer = AssetsHelper.GetImporterBy(_fileInfo.FullName);
            Assert.AreEqual(_abName, importer.assetBundleName);
            Assert.AreEqual(_abVariant, importer.assetBundleVariant);
        }
        [Test]
        public void SaveTest_0()
        {
            DeleteFile();
            SaveData();

            var json = JsonUtility.ToJson(_data, true);
            Assert.IsTrue(_fileInfo.Exists);
            Assert.AreEqual(json, File.ReadAllText(_fileFullPath));

            CheckAssetBundleInfo();
        }
#if UNITY_EDITOR && !ASSETS_DEBUG_ENABLE_AB
        [Test]
        public void LoadFromFile_Test()
        {
            DeleteFile();
            SaveData();
            using (_reader = new FileAssetLoader_Json<Data>(_fileInfo, _abName, _abVariant))
            {
                var data = default(Data);
                try
                {
                    data = _reader.Load();
                }
                catch (Exception e)
                {
                    Assert.Fail(e.ToString());
                    DeleteFile();
                }
                Assert.AreEqual(_data, data);
            }

        }
#endif
    }
}
