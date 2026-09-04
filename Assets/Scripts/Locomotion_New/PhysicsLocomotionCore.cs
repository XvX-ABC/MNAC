using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MNAC.TPhysics;
using MNAC.TPhysics.Environment;
using MNAC.Locomotion.Interfaces;

namespace MNAC.Locomotion.Adapters
{
    /// <summary>
    /// 物理系统运动核心适配器，实现运动核心接口
    /// </summary>
    public class PhysicsLocomotionCore : ILocomotionCore<ILocomotionContext, ILocomotionModule<ILocomotionContext>>
    {
        private enum ModuleState { Ready, Started, Update, Ended }

        private class ModuleWrapper
        {
            public ILocomotionModule<ILocomotionContext> Module;
            public ModuleState State;
            public byte StepNum;

            public ModuleWrapper(ILocomotionModule<ILocomotionContext> module, bool enabled)
            {
                Module = module;
                State = ModuleState.Ready;
                StepNum = 0;
                module.Enabled = enabled;
            }

            public ILocomotionContext Update(ILocomotionContext context)
            {
                if (StepNum > 0 && !Module.Enabled)
                    StepNum = 2;

                switch (StepNum)
                {
                    case 0:
                        if (Module.Enabled)
                        {
                            context = Module.Start(context);
                            StepNum = 1;
                            State = ModuleState.Started;
                        }
                        else
                            State = ModuleState.Ready;
                        break;
                    case 1:
                        context = Module.Update(context);
                        State = ModuleState.Update;
                        break;
                    case 2:
                        context = Module.End(context);
                        State = ModuleState.Ended;
                        StepNum = 0;
                        break;
                }
                return context;
            }
        }

        private ILocomotionContext _physicsContext;
        private readonly List<ModuleWrapper> _moduleWrappers = new();
        private readonly List<IEvaluationModule<ILocomotionContext>> _evaluationModules = new();

        public PhysicsLocomotionCore(World world, Rigidbody rbody, IGroundDetector groundDetector,
            params IEvaluationModule<ILocomotionContext>[] evaluationModules)
        {
            _physicsContext = new PhysicsLocomotionContext(world, rbody, groundDetector);
            if (evaluationModules != null)
                _evaluationModules.AddRange(evaluationModules);
        }

        public ILocomotionContext Context => _physicsContext;

        public void AddModule(ILocomotionModule<ILocomotionContext> module, bool enabled = false)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (Contains(module)) return;

            var wrapper = new ModuleWrapper(module, enabled);
            _moduleWrappers.Add(wrapper);
        }

        public void AddModule_InsertByPriority(ILocomotionModule<ILocomotionContext> module, bool enabled = false)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (Contains(module)) return;

            var wrapper = new ModuleWrapper(module, enabled);
            int insertIndex = _moduleWrappers.FindIndex(w => w.Module.Priority > module.Priority);

            if (insertIndex == -1)
                _moduleWrappers.Add(wrapper);
            else
                _moduleWrappers.Insert(insertIndex, wrapper);
        }

        public void RemoveModule(ILocomotionModule<ILocomotionContext> module)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            _moduleWrappers.RemoveAll(w => w.Module == module);
        }

        public void EnableModule(ILocomotionModule<ILocomotionContext> module)
        {
            var wrapper = _moduleWrappers.Find(w => w.Module == module);
            if (wrapper != null)
                wrapper.Module.Enabled = true;
        }

        public void DisableModule(ILocomotionModule<ILocomotionContext> module)
        {
            var wrapper = _moduleWrappers.Find(w => w.Module == module);
            if (wrapper != null)
                wrapper.Module.Enabled = false;
        }

        public bool Contains(ILocomotionModule<ILocomotionContext> module)
        {
            return _moduleWrappers.Any(w => w.Module == module);
        }

        public void Update()
        {
            // 1. 从物理引擎同步当前状态
            _physicsContext.Synchronize();

            // 2. 更新评估模块
            foreach (var evalModule in _evaluationModules.Where(m => m.Enabled))
            {
                evalModule.Update(_physicsContext);
            }

            // 3. 按优先级更新运动模块
            for (int i = 0; i < _moduleWrappers.Count; i++)
            {
                _physicsContext = _moduleWrappers[i].Update(_physicsContext);
            }

            // 4. 将运动变化应用到物理引擎
            _physicsContext.Apply();
        }
    }
}