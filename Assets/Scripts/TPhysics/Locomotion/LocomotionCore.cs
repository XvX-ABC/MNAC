using MNAC.TPhysics.Environment;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 运动模块装配核心。
    /// 单一装配入口 <see cref="SetModules"/>，单一数据源（内部 <see cref="Wrapper"/> 列表），
    /// 执行顺序 = 传入顺序。模块生命周期由内部 stepNum 状态机驱动（Start → Update → End）。
    public class LocomotionCore
    {
        struct Wrapper
        {
            internal ILocomotionModule module;
            byte stepNum;

            public Wrapper(ILocomotionModule module)
            {
                this.module = module;
                stepNum = 0;
            }

            public Context Update(Context context)
            {
                if (stepNum > 0 && !module.Enabled)
                    stepNum = 2;
                switch (stepNum)
                {
                    case 0:
                        if (module.Enabled)
                        {
                            context = module.Start(context);
                            stepNum = 1;
                        }
                        break;
                    case 1:
                        context = module.Update(context);
                        break;
                    case 2:
                        context = module.End(context);
                        stepNum = 0;
                        break;
                }
                return context;
            }
        }

        readonly List<Wrapper> _moduleWrappers = new List<Wrapper>();
        Context _context;

        public LocomotionCore(World world, [NotNull] Rigidbody rbody, [NotNull] IGroundDetector groundDetector)
            : this(new(world, new TPhysics.Context(rbody), groundDetector))
        {
        }

        public LocomotionCore(Context context)
        {
            if (context.Rbody == null || context.GroundDetector == null)
                throw new ArgumentException("This context was invalidated");
            _context = context;
        }

        public Context Context { get => _context; }

        /// 从单一数据源投影出的只读模块清单（顺序 = 执行顺序）。
        public IReadOnlyList<ILocomotionModule> Modules => _moduleWrappers.Select(w => w.module).ToList();

        /// 唯一装配入口：全量替换，顺序 = 执行顺序。传入 null/空 安全（清空后 Update 不崩）。
        public void SetModules(IEnumerable<ILocomotionModule> modules)
        {
            _moduleWrappers.Clear();
            if (modules == null)
                return;
            foreach (var module in modules)
            {
                if (module == null)
                    continue;
                _moduleWrappers.Add(new Wrapper(module));
            }
        }

        public bool Contains(ILocomotionModule module)
        {
            if (module == null)
                return false;
            for (int i = 0; i < _moduleWrappers.Count; i++)
            {
                if (_moduleWrappers[i].module == module)
                    return true;
            }
            return false;
        }

        public void EnableModule(ILocomotionModule module)
        {
            if (module == null)
                return;
            for (int i = 0; i < _moduleWrappers.Count; i++)
            {
                if (_moduleWrappers[i].module == module)
                {
                    _moduleWrappers[i].module.Enabled = true;
                    return;
                }
            }
        }

        public void DisableModule(ILocomotionModule module)
        {
            if (module == null)
                return;
            for (int i = 0; i < _moduleWrappers.Count; i++)
            {
                if (_moduleWrappers[i].module == module)
                {
                    _moduleWrappers[i].module.Enabled = false;
                    return;
                }
            }
        }

        public void Update()
        {
            _context.Synchronise();
            for (int i = 0; i < _moduleWrappers.Count; i++)
            {
                var w = _moduleWrappers[i];
                _context = w.Update(_context);
                _moduleWrappers[i] = w;
            }
            _context.Apply();
        }
    }
}
