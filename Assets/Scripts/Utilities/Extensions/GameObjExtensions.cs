using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MNAC.Utilities.Extensions
{
    public static class GameObjExtensions
    {
        public static void PutInParent(this GameObject obj, GameObject parent)
        {
            obj?.transform?.SetParent(parent?.transform);
        }
    }
}
