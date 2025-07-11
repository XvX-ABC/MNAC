using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Tests.Scripts.BDExtensions.Variables
{
    public class SharedAnimator : SharedVariable<Animator>
    {
        public static implicit operator SharedAnimator(Animator value)
        {
            return new SharedAnimator { Value = value };
        }
    }
}
