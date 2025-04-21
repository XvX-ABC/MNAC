using System;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    public enum MountPointType
    {
        Weapon
    }
    [Serializable]
    public class MountPoint
    {
        [SerializeField]
        GameObject _mountPointObj;
        [SerializeField]
        MountPointType _type;

        Func<GameObject, GameObject, GameObject> _loadChangedFunc;
        Action<GameObject> _loadAction;
        GameObject _loadObj;
        public GameObject LoadObj
        {
            get => _loadObj;
            set
            {
                var currentLoad = _loadObj;
                var newLoad = value;
                if (_loadChangedFunc != null)
                    newLoad = _loadChangedFunc(currentLoad, newLoad);
                else
                    newLoad = value;

                if (currentLoad != null)
                    Unmount();
                if (newLoad != null)
                    Mount(newLoad);
            }
        }
        public Func<GameObject, GameObject, GameObject> LoadObjChangeFunc
        {
            get => _loadChangedFunc;
            set => _loadChangedFunc = value;
        }
        public Action<GameObject> LoadObjAction { get => _loadAction; set => _loadAction = value; }
        public string Name { get => _mountPointObj.name; }

        protected void Unmount()
        {
            _loadObj.transform.parent = null;
            _loadObj = null;
        }
        protected void Mount(GameObject obj)
        {
            obj.transform.parent = _mountPointObj.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _loadObj = obj;
        }
        public void OnUpdate()
        {
            _loadAction?.Invoke(_loadObj);
        }
    }
}
