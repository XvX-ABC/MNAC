using Tests.Interaction;
using Unity.VisualScripting;
using UnityEngine;

namespace Tess.AI
{
    internal class LockTarget : MonoBehaviour, ILockTarget
    {
        LockType _lockType;
        public LockType LockType { get => _lockType; set => _lockType = value; }

        public GameObject Obj
        {
            get
            {
                if (this.gameObject != null)
                    return this.gameObject;
                return null;
            }
        }

        public Vector3 Position => this.transform.position;
        public override string ToString()
        {
            return Obj.ToString();
        }
    }
}
