using MNAC.TPhysics.Environment;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 运动模块装配核心。
    /// 装配入口：<see cref="SetModules"/>（全量替换）与 <see cref="AddModule"/>（单个插入），
    /// 单一数据源（内部 <see cref="Wrapper"/> 列表）。
    /// 执行顺序 = <see cref="ILocomotionModule.Priority"/> 升序；同优先级保持装配顺序（二分插入，稳定）。
    /// 模块修改 Priority 会立即通知本核心重排（保持生命周期状态）；<see cref="Update"/> 期间修改会被忽略并告警。
    /// 模块生命周期由内部 stepNum 状态机驱动（Start → Update → End）。
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
        bool _updating;

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

        /// 全量替换。按 <see cref="ILocomotionModule.Priority"/> 升序执行，
        /// 同优先级保持传入顺序（二分查找到插入位置，稳定）。传入 null/空 安全（清空后 Update 不崩）。
        public void SetModules(IEnumerable<ILocomotionModule> modules)
        {
            _moduleWrappers.Clear();
            if (modules == null)
                return;
            foreach (var module in modules)
                AddModule(module);
        }

        /// 单个插入：按 Priority 二分查找到位置（同优先级排在已有同值之后）。
        /// null / 已在链中的模块会被忽略。<returns>是否实际插入。</returns>
        public bool AddModule(ILocomotionModule module)
        {
            if (module == null || Contains(module))
                return false;
            _moduleWrappers.Insert(FindInsertIndex(module.Priority), new Wrapper(module));
            if (module is LocomotionModuleBase m)
                m.Owner = this;
            return true;
        }

        /// Update 迭代期间为 true；此时模块不允许修改 Priority。
        internal bool IsUpdating => _updating;

        /// 模块 Priority 变化后的即时重排：原位移动，保留 Wrapper 的生命周期状态。
        internal void Reposition(LocomotionModuleBase module)
        {
            int index = IndexOf(module);
            if (index < 0)
                return; // 已不在链中（如被 SetModules 移除），忽略
            var wrapper = _moduleWrappers[index];
            _moduleWrappers.RemoveAt(index);
            _moduleWrappers.Insert(FindInsertIndex(wrapper.module.Priority), wrapper);
        }

        /// 二分查找：返回第一个 Priority 大于给定值的下标（升序、同值排在已有同值之后）。
        int FindInsertIndex(int priority)
        {
            int left = 0, right = _moduleWrappers.Count;
            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (_moduleWrappers[mid].module.Priority <= priority)
                    left = mid + 1;
                else
                    right = mid;
            }
            return left;
        }

        public bool Contains(ILocomotionModule module) => IndexOf(module) >= 0;

        int IndexOf(ILocomotionModule module)
        {
            if (module == null)
                return -1;
            for (int i = 0; i < _moduleWrappers.Count; i++)
            {
                if (_moduleWrappers[i].module == module)
                    return i;
            }
            return -1;
        }

        /// 启用模块。空模块 / 不在装配链中 / Update 迭代期间调用均会被拒绝并告警。
        /// <returns>是否实际修改。</returns>
        public bool EnableModule(ILocomotionModule module) => SetModuleEnabled(module, true, nameof(EnableModule));

        /// 停用模块。空模块 / 不在装配链中 / Update 迭代期间调用均会被拒绝并告警。
        /// <returns>是否实际修改。</returns>
        public bool DisableModule(ILocomotionModule module) => SetModuleEnabled(module, false, nameof(DisableModule));

        bool SetModuleEnabled(ILocomotionModule module, bool enabled, string operation)
        {
            if (module == null)
            {
                Debug.LogWarning($"[LocomotionCore] {operation}: 模块为空，已忽略。");
                return false;
            }
            if (_updating)
            {
                Debug.LogWarning($"[LocomotionCore] {operation}: Update 期间不允许修改模块激活状态（{module.GetType().Name}），已忽略。");
                return false;
            }
            int index = IndexOf(module);
            if (index < 0)
            {
                Debug.LogWarning($"[LocomotionCore] {operation}: {module.GetType().Name} 不在装配链中，已忽略。");
                return false;
            }
            _moduleWrappers[index].module.Enabled = enabled;
            return true;
        }

        public void Update()
        {
            _context.Synchronise();
            _updating = true;
            try
            {
                for (int i = 0; i < _moduleWrappers.Count; i++)
                {
                    var w = _moduleWrappers[i];
                    _context = w.Update(_context);
                    _moduleWrappers[i] = w;
                }
            }
            finally
            {
                _updating = false;
            }
            _context.Apply();
        }
    }
}
