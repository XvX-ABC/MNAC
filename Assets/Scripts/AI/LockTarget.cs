using Tests.Characters;
using Tests.Characters.Humanoid;
using Tests.Interaction;
using Unity.VisualScripting;
using UnityEngine;

namespace Tess.AI
{
    internal class LockTarget : MonoBehaviour, ILockTarget
    {
        LockType _lockType;
        GameObject _chestObj;
        public LockType LockType { get => _lockType; set => _lockType = value; }

        public GameObject Obj
        {
            get
            {
                if (this != null)
                    return this.gameObject;
                return null;
            }
        }

        public Vector3 Position => _chestObj?.transform?.position ?? this.transform.position;
        //public Vector3 Position
        //{
        //    get
        //    {
        //        if (_chestObj != null)
        //        {
        //            Debug.Log("Current get position of chest obj");
        //            return _chestObj.transform.position;
        //        }
        //        return this.transform.position;
        //    }
        //}
        protected void Awake()
        {
            if (TryGetComponent<ICompositeItems>(out var compositeItems))
            {
                _chestObj = compositeItems.GetItem((uint)HumanBodyPart.Chest);
            }
        }
        public override string ToString()
        {
            return Obj.ToString();
        }
    }
}
