using MNAC.TPhysics.Environment;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using MNAC.Utilities;
using MNAC.Utilities.Extensions;
using System.Collections.ObjectModel;

namespace MNAC.TPhysics.Locomotion
{
    public class LocomotionCore
    {
        enum ModuleState
        {
            Ready,
            Started,
            Update,
            Ended,
        }
        struct Wrapper
        {
            public static readonly Wrapper Default = new Wrapper();
            internal ILocomotionModule module;
            internal ModuleState state;
            byte stepNum;

            public Wrapper(ILocomotionModule module, bool enabled)
            {
                this.module = module;
                state = ModuleState.Ready;
                stepNum = 0;

                module.Enabled = enabled;
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
                            state = ModuleState.Started;
                        }
                        else
                            state = ModuleState.Ready;
                        break;
                    case 1:
                        context = module.Update(context);
                        state = ModuleState.Update;
                        break;
                    case 2:
                        context = module.End(context);
                        state = ModuleState.Ended;
                        stepNum = 0;
                        break;
                }
                return context;
            }

        }
        ILocomotionModule[] _modules;
        Wrapper[] _moduleWrappers;
        Context _context;
        public LocomotionCore(World world, [NotNull] Rigidbody rbody, [NotNull] IGroundDetector groundDetector) : this(new(world, new TPhysics.Context(rbody), groundDetector))
        {
        }
        public LocomotionCore(Context context)
        {
            if (context.Rbody == null || context.GroundDetector == null)
                throw new ArgumentException("This context was invalidated");
            _context = context;
        }
        public Context Context { get => _context; }

        public ReadOnlyCollection<ILocomotionModule> Modules { get => Array.AsReadOnly(_modules); }

        int IndexOf(ILocomotionModule module)
        {
            if (_moduleWrappers == null)
                return -1;
            for (int i = 0; i < _moduleWrappers.Length; i++)
            {
                var m = _moduleWrappers[i].module;
                if (m == module)
                    return i;
            }
            return -1;
        }
        public bool Contains(ILocomotionModule module)
        {
            return IndexOf(module) != -1;
        }
        int FindIndexByPriority<T>(T[] arr, Func<T, int> match, int priority)
        {
            int left = 0, right = arr.Length;
            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (match(arr[mid]) <= priority)
                    left = mid + 1;
                else
                    right = mid;
            }
            return left;
        }
        public void AddModule(ILocomotionModule module, bool enabled = false)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            var index = IndexOf(module);
            if (index > -1)
                return;
            var w = new Wrapper(module, enabled);
            if (_moduleWrappers == null)
                _moduleWrappers = new Wrapper[] { w };
            else
                _moduleWrappers = _moduleWrappers.Append(w);
        }
        public void AddModule_InsertByPriority(ILocomotionModule module, bool enabled = false)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            var index = IndexOf(module);
            if (index > -1)
                return;
            var w = new Wrapper(module, enabled);
            if (_moduleWrappers == null)
            {
                _moduleWrappers = new Wrapper[] { w };
                _modules = new ILocomotionModule[] { module };
            }
            else
            {
                var priority = module.Priority;
                var idx = FindIndexByPriority<Wrapper>(_moduleWrappers, m => m.module.Priority, priority);
                if (idx == -1)
                {
                    _moduleWrappers = _moduleWrappers.Append(w);
                    _modules = _modules.Append(module);

                }
                else
                {
                    _moduleWrappers = _moduleWrappers.Insert(idx, w);
                    _modules = _modules.Insert(idx, module);
                }
            }
        }
        public void RemoveModule(ILocomotionModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            var index = IndexOf(module);
            if (index == -1)
                return;
            if (_moduleWrappers.Length == 1)
                _moduleWrappers = null;
            else
                _moduleWrappers.Remove(index);
        }

        public void EnableModule(ILocomotionModule module)
        {
            var index = IndexOf(module);
            if (index == -1)
                return;
            _moduleWrappers[index].module.Enabled = true;
        }
        public void DisableModule(ILocomotionModule module)
        {
            var index = IndexOf(module);
            if (index == -1)
                return;
            _moduleWrappers[index].module.Enabled = false;
        }
        public void Update()
        {
            _context.Synchronise();
            if (_moduleWrappers != null)
                for (int i = 0; i < _moduleWrappers.Length; i++)
                {
                    var w = _moduleWrappers[i];
                    _context = w.Update(_context);
                    _moduleWrappers[i] = w;
                }
            _context.Apply();
        }

    }
}
