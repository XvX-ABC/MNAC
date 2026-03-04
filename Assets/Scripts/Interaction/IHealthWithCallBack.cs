using System.Collections.Generic;
using UnityEngine;

namespace MNAC.Interaction
{
    public interface IHealthWithCallBack : IHealth
    {

        void AddCallback(IHealthCallback callback);
        bool RemoveCallback(IHealthCallback callback);
    }
}
