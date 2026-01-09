using RootMotion.Demos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.AI.Debugging
{
    [DefaultExecutionOrder(1)]
    internal class MoveBetweenPoints : DebuggingBase
    {
        [SerializeField]
        List<GameObject> _points;
        [SerializeField]
        AICore _aicore;
        AINavigation _navigation;
        [SerializeField]
        int _currentIndex;
        [SerializeField]
        bool _startMoving;
        private void Start()
        {
            _navigation = _aicore.componentContext.navigation;
            //_aicore.BehaviourTree.enabled = false;
        }
        GameObject GetPoint()
        {
            var point = _points[_currentIndex++];
            if (_currentIndex >= _points.Count)
                _currentIndex = 0;
            return point;
        }
        private void FixedUpdate()
        {
            if (!_navigation.IsMoving)
            {
                var point = GetPoint();
                _aicore.TargetObj = point;
            }
        }
    }
}
