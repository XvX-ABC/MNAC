using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    [SerializeField]
    public class GameObjsInRadiusCatcher : CatcherBase<GameObject>
    {
        [SerializeField]
        ushort _processingAmountOfFrames;
        Action<List<GameObject>> _catchCompletedAction;
        [SerializeField]
        Vector3 _origin;
        [SerializeField]
        float _radius;
        float _sqrRadius;
        public GameObjsInRadiusCatcher(Vector3 origin, float radius, ushort processingAmountInCoroutine = 30)
        {
            Origin = origin;
            Radius = radius;
            _processingAmountOfFrames = processingAmountInCoroutine;
        }

        public ushort ProcessingAmountOfFrames { get => _processingAmountOfFrames; set => _processingAmountOfFrames = value; }
        public Action<List<GameObject>> CatchCompletedAction { get => _catchCompletedAction; set => _catchCompletedAction = value; }
        public Vector3 Origin { get => _origin; set => _origin = value; }
        public float Radius
        {
            get => _radius;
            set
            {
                _radius = Mathf.Max(0, _radius);
                _sqrRadius = Mathf.Pow(_radius, 2);
            }
        }

        bool CheckInRadius(GameObject obj)
        {
            var origin = _origin;
            var target = obj.transform.position;
            var tv = origin - target;
            return tv.sqrMagnitude <= _sqrRadius;
        }
        public override IEnumerator UpdateWithCoroutine()
        {
            while (true)
            {
                if (!enabled)
                    CleanAll();
                var i = 0;
                InteractionManager.SynchronizeChanges();
                foreach (var item in InteractionManager.items)
                {
                    var obj = item.Obj;
                    var index = caughtItems.FindIndex(o => o == obj);
                    if (obj == null)
                        continue;
                    var inRadius = CheckInRadius(obj);
                    if (index > -1 && !inRadius)
                    {
                        RemoveItemImpl(obj);
                    }
                    else if (index == -1 && inRadius)
                    {
                        AddItemImpl(obj);
                    }
                    if (i <= 0 || i % _processingAmountOfFrames == 0)
                        yield return null;
                    i++;
                }
                yield return null;
            }
        }
    }
}
