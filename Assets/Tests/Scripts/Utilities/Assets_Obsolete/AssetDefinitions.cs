//#define EDITOR_ASSET_LOAD
using System;
using System.IO;
using UnityEngine;

namespace Tests.Obsolete_Assets
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
            if (Name == null || Name.Length == 0)
                return null;
            return Path.Join(DirPath, Name);
#else
            if(Name==null || Name.Length==0)
                return null;
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
