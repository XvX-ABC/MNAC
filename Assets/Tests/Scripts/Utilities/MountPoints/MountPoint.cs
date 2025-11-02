using System;
using UnityEngine;

namespace Tests.Utilities.MountPoints
{
    [Serializable]
    public class MountPoint
    {
        [SerializeField]
        GameObject _mountPointObj;
        [SerializeField]
        MountPointType _type;

        Func<ILoad, ILoad, ILoad> _loadChangedFunc;
        ILoad _load;
        public virtual GameObject LoadObj
        {
            get => _load.Obj;
            set
            {
                var load = value.GetComponent<ILoad>() ?? throw new ComponentCantFindException(value, typeof(ILoad));
                Load = load;
            }
        }
        public virtual ILoad Load
        {
            get => _load;
            set
            {
                var currentLoad = _load;
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
        public Func<ILoad, ILoad, ILoad> LoadObjChangeFunc
        {
            get => _loadChangedFunc;
            set => _loadChangedFunc = value;
        }
        public string Name { get => _mountPointObj.name; }

        protected void Unmount()
        {
            var obj = _load.Obj;
            obj.transform.parent = null;
            obj = null;
            _load.WhenUnmounted(_mountPointObj);
        }
        protected void Mount(ILoad load)
        {
            var obj = load.Obj;
            obj.transform.parent = _mountPointObj.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _load = load;
            _load.WhenMounted(_mountPointObj);
        }
    }
}
