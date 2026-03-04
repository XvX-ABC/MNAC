using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MNAC.Utilities.Assets.DataControl
{
    /// <summary>
    /// 加载AssetBundle工具类 单例 
    /// </summary>
    public class LoadAssetBundleManager : Singleton<LoadAssetBundleManager>
    {
        //主AB包 
        private AssetBundle mainAssetBundle = null;
        //包体依赖manifest 
        private AssetBundleManifest assetBundleManifest = null;

        //防止AB包重复加载 对已经加载的AB包存储
        private Dictionary<string, AssetBundle> assetBundlesDic = new Dictionary<string, AssetBundle>();

        //加载路径
        private string pathAssetBundle
        {
            get
            {
                return Application.streamingAssetsPath + "/";
            }
        }
        //主包名称
        private string mainAssetBundleName
        {
            get
            {
#if UnITY_IOS
                return "IOS";
#elif UNITY_ANDROID
                return "Android";
#else
                return "StandaloneWindows";
#endif
            }
        }

        public override void Awake()
        {
            base.Awake();
            persistentAcrossScenes = true;
        }

        /// <summary>
        /// 根据名称加载AB包 也会检查相关依赖包 进行加载
        /// </summary>
        /// <param name="assetBundleName">AB包的名称</param>
        public void LoadAssetBundle(string assetBundleName)
        {
            if (!assetBundlesDic.ContainsKey(assetBundleName))
            {
                AssetBundle resAssetBundle = AssetBundle.LoadFromFile(pathAssetBundle + assetBundleName);
                assetBundlesDic.Add(assetBundleName, resAssetBundle);
            }
            //加载主资源包 从主资源包中获取对manifest
            if (mainAssetBundle == null)
            {
                mainAssetBundle = AssetBundle.LoadFromFile(pathAssetBundle + mainAssetBundleName);
                assetBundleManifest = mainAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            //加载目标资源包的依赖AB
            string[] dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
            foreach (var dependency in dependencies)
            {
                AssetBundle currentAB = null;
                if (!assetBundlesDic.ContainsKey(dependency))
                {
                    //加载依赖的ab包
                    currentAB = AssetBundle.LoadFromFile(pathAssetBundle + dependency);
                    assetBundlesDic.Add(dependency, currentAB);
                }
            }
        }

        /// <summary>
        /// 从AB包中获取具体资源
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <param name="resName">资源名称</param>
        /// <returns>Object资源</returns>
        public Object LoadResource(string abName, string resName)
        {
            LoadAssetBundle(abName);
            Object resObj = null;
            resObj = assetBundlesDic[abName].LoadAsset(resName);
            return resObj;
        }
        /// <summary>
        /// 泛型方法重载
        /// </summary>
        public T LoadResource<T>(string abName, string resName) where T : Object
        {
            LoadAssetBundle(abName);
            T res = assetBundlesDic[abName].LoadAsset<T>(resName);
            return res;
        }
        /// <summary>
        /// 根据资源类型重载方法
        /// </summary>
        public Object LoadResource(string abName, string resName, System.Type type)
        {
            LoadAssetBundle(abName);
            Object obj = assetBundlesDic[abName].LoadAsset(resName, type);
            return obj;
        }
        //--------------------------------------------------------
        //同步加载的AB包 异步加载res资源
        public void LoadResourceAsync(string abName, string resName, UnityAction<Object> callback)
        {
            StartCoroutine(LoadResourceIEn(abName, resName, callback));
        }
        //异步加载协程
        private IEnumerator LoadResourceIEn(string abName, string resName, UnityAction<Object> callback)
        {
            LoadAssetBundle(abName);
            AssetBundleRequest request = assetBundlesDic[abName].LoadAssetAsync(resName);
            yield return request;
            callback(request.asset);
        }
        //根据泛型来异步加资源
        public void LoadResourceAsync<T>(string abName, string resName, UnityAction<Object> callback) where T : Object
        {
            StartCoroutine(LoadResourceIEn<T>(abName, resName, callback));
        }
        //异步加载协程
        private IEnumerator LoadResourceIEn<T>(string abName, string resName, UnityAction<Object> callback) where T : Object
        {
            LoadAssetBundle(abName);
            AssetBundleRequest request = assetBundlesDic[abName].LoadAssetAsync<T>(resName);
            yield return request;
            callback(request.asset);
        }
        //根据res类型异步加载资源
        //根据泛型来异步加资源
        public void LoadResourceAsync(string abName, string resName, System.Type type, UnityAction<Object> callback)
        {
            StartCoroutine(LoadResourceIEn(abName, resName, type, callback));
        }
        //异步加载协程
        private IEnumerator LoadResourceIEn(string abName, string resName, System.Type type, UnityAction<Object> callback)
        {
            LoadAssetBundle(abName);
            AssetBundleRequest request = assetBundlesDic[abName].LoadAssetAsync(resName, type);
            yield return request;
            callback(request.asset);
        }
        //资源包的卸载
        public void UnLoadAssetBundle(string abName)
        {
            if (assetBundlesDic.ContainsKey(abName))
            {
                assetBundlesDic[abName].Unload(false);
                assetBundlesDic.Remove(abName);
            }
        }
        //卸载所有加载的资源包
        public void UnLoadAllAssetBundle()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            assetBundlesDic.Clear();
            mainAssetBundle = null;
            assetBundleManifest = null;
        }

    }
}
