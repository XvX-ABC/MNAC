//#define EDITOR_ASSET_LOAD
using System;
using System.IO;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons.Assets__0
{
    [Serializable]
    public class AssetDefinitions
    {
        public string Name;
        public string BundleName;
#if UNITY_EDITOR
        public string DirPath;
#endif
        public string GetPath()
        {
#if UNITY_EDITOR && EDITOR_ASSET_LOAD
            return Path.Join(DirPath, Name);
#else
            return Path.Join(BundleName,Name);
#endif
        }

        public void SetPath(string path)
        {
#if UNITY_EDITOR && EDITOR_ASSET_LOAD
            Name = Path.GetFileName(path);
            DirPath = Path.GetDirectoryName(path);
#else

#endif
        }
    }
}
