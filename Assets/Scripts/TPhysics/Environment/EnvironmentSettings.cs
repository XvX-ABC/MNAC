using System;
using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    /// 环境地面检测的唯一配置入口（合并自原 Definitions/Mono 若干类）。
    [Serializable]
    public class EnvironmentSettings
    {
        [SerializeField]
        Vector3 _worldUp = Vector3.up;
        [Range(0f, 90f)]
        [SerializeField]
        float _maxSlope = 45f;
        [SerializeField]
        LayerMask _groundMask;
        [Tooltip("空中探针最大射线距离，0 表示关闭")]
        [SerializeField]
        float _maxGroundDistance = 1f;

        public Vector3 WorldUp => _worldUp;
        public float MaxSlope => _maxSlope;
        public LayerMask GroundMask => _groundMask;
        public float MaxGroundDistance => _maxGroundDistance;

        public EnvironmentSettings()
        {
        }

        public EnvironmentSettings(Vector3 worldUp, float maxSlope, LayerMask groundMask, float maxGroundDistance = 1f)
        {
            _worldUp = worldUp.normalized;
            _maxSlope = maxSlope;
            _groundMask = groundMask;
            _maxGroundDistance = Mathf.Max(0f, maxGroundDistance);
        }
    }
}
