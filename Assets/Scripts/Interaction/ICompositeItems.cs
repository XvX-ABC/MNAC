using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MNAC.Interaction
{
    public interface ICompositeItems<T>
    {
        public GameObject GetItem(uint key);
    }
}
