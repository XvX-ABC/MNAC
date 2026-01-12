using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.Utilities
{
    public static class TransformHelper
    {
        static TransformHelper()
        {
            InvalidPosition_V2 = Vector2.positiveInfinity;
        }
        public static readonly Vector2 InvalidPosition_V2;
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
