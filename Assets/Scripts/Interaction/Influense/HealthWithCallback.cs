using System;
using System.Collections.Generic;
using UnityEngine;

namespace MNAC.Interaction.Influences
{
    [Serializable]
    public class HealthWithCallback : Health, IHealthWithCallBack
    {
        struct Wrapper
        {
            internal IHealthCallback callback;
            internal ushort executedAmount;
        }
        GameObject _ownerObj;
        List<Wrapper> _callbackWrapperList;
        public HealthWithCallback(GameObject ownerObj, float maxPoint, IHealthEffector healthEffector = null) : base(maxPoint, healthEffector)
        {
            _ownerObj = ownerObj ?? throw new ArgumentNullException(nameof(ownerObj));
            _callbackWrapperList = new();
        }

        public HealthWithCallback(GameObject ownerObj, float maxPoint, float point, IHealthEffector healthEffector = null) : base(maxPoint, point, healthEffector)
        {
            _ownerObj = ownerObj ?? throw new ArgumentNullException(nameof(ownerObj));
            _callbackWrapperList = new();
        }

        public HealthWithCallback(GameObject ownerObj, float maxPoint, float minPoint, float point, IHealthEffector healthEffector = null) : base(maxPoint, minPoint, point, healthEffector)
        {
            _ownerObj = ownerObj ?? throw new ArgumentNullException(nameof(ownerObj));
            _callbackWrapperList = new();
        }
        protected override float point
        {
            get => base.point;
            set
            {
                base.point = value;
                ExecuteCallbacks(point);
            }
        }
        float pointProportion { get => maxPoint > 0 ? point / maxPoint : 0; }
        void ExecuteCallbacks(float point)
        {
            for (int i = 0; i < _callbackWrapperList.Count; i++)
            {
                var w = _callbackWrapperList[i];
                var cb = w.callback;
                if (cb.TriggerProportion >= pointProportion && (cb.RepetitiveExecution || w.executedAmount == 0))
                {
                    cb.Execute(_ownerObj, this);
                    w.executedAmount++;
                    _callbackWrapperList[i] = w;
                }
            }
        }
        public void AddCallback(IHealthCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            _callbackWrapperList.Add(new() { callback = callback });
        }
        public bool RemoveCallback(IHealthCallback callback)
        {
            var idx = _callbackWrapperList.FindIndex(w => w.callback == callback);
            if (idx == -1)
                return false;
            _callbackWrapperList.RemoveAt(idx);
            return true;
        }
        public void Reset()
        {
            point = maxPoint;
            Debug.Log("health reset: " + IsAlive + ", " + point);
        }
    }
}
