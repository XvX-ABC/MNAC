using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.Interaction
{
    public interface IGameObjTarget_New : ITarget_New
    {
        public GameObject Obj { get; }
    }

}
