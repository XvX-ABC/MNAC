using System;
using System.Diagnostics.CodeAnalysis;
using Tests.Extensions;
using Tests.TPhysics.Environment;
using UnityEngine;

namespace Tests.TPhysics.Locomotion
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
        World _world;
        IEvaluationModule[] _evaluationModules;
        Wrapper[] _moduleWrappers;
        Context _context;
        public LocomotionCore(World world, [NotNull] Rigidbody rbody, [NotNull] IGroundDetector groundDetector, params IEvaluationModule[] evaluationModules) : this(new(world, new TPhysics.Context(rbody), groundDetector), evaluationModules)
        {
        }
        public LocomotionCore(Context context, params IEvaluationModule[] evaluationModules)
        {
            if (context.Rbody == null || context.GroundDetector == null)
                throw new ArgumentException("This context is invalidate");
            _context = context;
            EvaluationModules = evaluationModules;
        }

        public World World
        {
            get => _world;
            set
            {
                _world = value == null ? World.Default : value;
                _context.world = _world;
                if (_moduleWrappers != null)
                    for (int i = 0; i < _moduleWrappers.Length; i++)
                    {
                        var m = _moduleWrappers[i].module;
                        m.World = value;
                    }
            }
        }
        public IEvaluationModule[] EvaluationModules
        {
            get => _evaluationModules;
            set
            {
                if (value != null)
                {
                    foreach (var m in value)
                    {
                        m.World = this.World;
                    }
                }
                _evaluationModules = value;
            }
        }
        public Context Context { get => _context; }

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
        public void AddModule(ILocomotionModule module, bool enabled = false)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            var index = IndexOf(module);
            if (index > -1)
                return;
            module.World = _world;
            var w = new Wrapper(module, enabled);
            if (_moduleWrappers == null)
                _moduleWrappers = new Wrapper[] { w };
            else
                ArrayExtensions.Append(ref _moduleWrappers, w);
            //_moduleWrappers.Append(w);
        }
        public void RemoveModule(ILocomotionModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            var index = IndexOf(module);
            if (index == -1)
                return;
            module.World = World.Default;
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
            if (_evaluationModules != null)
            {
                for (int i = 0; i < _evaluationModules.Length; i++)
                {
                    var m = _evaluationModules[i];
                    if (m.Enabled)
                        _context = m.Update(_context);
                }
            }
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
