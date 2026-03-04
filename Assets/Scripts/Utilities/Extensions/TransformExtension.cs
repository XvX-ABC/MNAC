using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MNAC.Utilities.Extensions
{
    public static class TransformExtension
    {
        public static void PutInParent(this Transform trans, Transform parent)
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
    }
}
