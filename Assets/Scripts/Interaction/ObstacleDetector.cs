using System;
using UnityEngine;

namespace Tests.Interaction
{
    [Serializable]
    public class ObstacleDetector
    {
        [SerializeField]
        LayerMask _obstacleMask;
        [SerializeField]
        bool _enableDebug;
        public bool TryDetect(Vector3 originPosition, Vector3 targetPosition, out GameObject obstacleObj)
        {
            obstacleObj = default;
            var tv = targetPosition - originPosition;
            var distance = tv.magnitude;
            var ray = new Ray(originPosition, tv.normalized);
            if (Physics.Raycast(ray, out var hit, distance, _obstacleMask))
            {
                obstacleObj = hit.collider.gameObject;
                if (_enableDebug)
                    Debug.DrawLine(originPosition, targetPosition, Color.red, Time.deltaTime);
                return true;
            }
            if (_enableDebug)
                Debug.DrawLine(originPosition, targetPosition, Color.green, Time.deltaTime);
            return false;
        }
    }
}
