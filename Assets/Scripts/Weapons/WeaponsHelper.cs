using System;
using UnityEngine;

namespace MNAC.Weapons
{
    internal class WeaponsHelper
    {
        [Obsolete("应该使用'transform.PutInParent'扩展方法")]
        public static void PutInParent(Transform trans, Transform parent)
        {
            if (trans == null)
                throw new ArgumentNullException(nameof(trans));
            trans.SetParent(parent);
            if (parent != null)
            {
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
            }
        }
        [Obsolete("应该使用'gameobject.PutInParent'扩展方法")]
        public static void PutInParent(GameObject obj, Transform parent)
        {
            PutInParent(obj?.transform, parent);
        }
        public static void SynchronizeWorldTransform(Transform source, Transform target)
        {
            if (source == null || target == null)
                throw new ArgumentNullException($"{nameof(source)} or {nameof(target)} is null");
            source.position = target.position;
            source.rotation = target.rotation;
        }
        public static void SynchronizeWorldPosition(GameObject source, GameObject target)
        {
            SynchronizeWorldTransform(source?.transform, target?.transform);
        }
    }
}
