using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MNAC.Utilities.Assets.DataControl
{
    public class AssetsHelper
    {
        public static string BasePath;
        static AssetsHelper()
        {
#if UNITY_EDITOR
            BasePath = Application.dataPath;
#endif
        }
        public static AssetImporter GetImporterBy(FileInfo fileInfo)
        {
            return GetImporterBy(fileInfo.FullName);
        }
        public static AssetImporter GetImporterBy(string fileFullPath)
        {
            var relativePath = Path.GetRelativePath(Directory.GetParent(BasePath).FullName, fileFullPath);
            var importer = AssetImporter.GetAtPath(relativePath);
            if (importer == null)
            {
                throw new NullReferenceException($"Can not get a importer by file path '{fileFullPath}'.");
            }
            return importer;
        }
    }
}
