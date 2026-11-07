using System.Collections.Generic;
using MNAC.TPhysics.Environment;
using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 运动模块的"可视化装配"运行宿主（MonoBehaviour）。
    /// 负责：Inspector 里经 <see cref="modules"/> 面板完成模块增删/排序/启停与参数编辑；
    /// 运行时把面板清单喂给 <see cref="LocomotionCore"/>，并把碰撞回调转发给 <see cref="GroundDetector"/>。
    /// 模块清单为空 = 空链（不预填任何模块），行为完全由面板决定。不包含任何演示/测试逻辑。
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class LocomotionVisualRunner : MonoBehaviour
    {
        [SerializeField]
        Rigidbody targetRigidbody;

        [SerializeField]
        EnvironmentSettings environmentSettings = new EnvironmentSettings(Vector3.up, 45f, ~0, 1f);

        [Tooltip("运动模块；运行时按 Priority 升序执行，同优先级保持此列表顺序（面板里可视编辑）")]
        [SerializeReference]
        List<ILocomotionModule> modules = new();

        GroundDetector _groundDetector;
        LocomotionCore _core;

        public LocomotionCore Core => _core;
        public IGroundDetector GroundDetector => _groundDetector;

        void OnValidate()
        {
            if (targetRigidbody == null)
                targetRigidbody = GetComponent<Rigidbody>();
        }

        void Awake()
        {
            if (targetRigidbody == null)
                targetRigidbody = GetComponent<Rigidbody>();
            Build();
        }

        void OnEnable()
        {
            if (_groundDetector != null)
                _groundDetector.Enabled = true;
        }

        void OnDisable()
        {
            if (_groundDetector != null)
                _groundDetector.Enabled = false;
        }

        void Build()
        {
            var settings = environmentSettings ?? new EnvironmentSettings(Vector3.up, 45f, ~0, 1f);
            var world = new World(settings.WorldUp, Physics.gravity);
            _groundDetector = new GroundDetector(settings, world);
            _groundDetector.Enabled = isActiveAndEnabled;
            _core = new LocomotionCore(world, targetRigidbody, _groundDetector);
            _core.SetModules(modules);
        }

        void FixedUpdate()
        {
            if (_groundDetector == null)
                return;
            _groundDetector.Position = targetRigidbody.position;
            _groundDetector.OnFixedUpdate();
            _core?.Update();
        }

        /// 选中对象时驱动各模块的调试绘制（模块默认空实现，只有覆写了才有输出）。
        void OnDrawGizmosSelected()
        {
            if (_core == null)
                return;
            var modules = _core.Modules;
            var context = _core.Context;
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i] is LocomotionModuleBase module)
                    module.OnDrawGizmos(context);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (enabled)
                _groundDetector?.OnCollisionEnter(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            if (enabled)
                _groundDetector?.OnCollisionStay(collision);
        }

        void OnCollisionExit(Collision collision)
        {
            if (enabled)
                _groundDetector?.OnCollisionExit(collision);
        }
    }
}
