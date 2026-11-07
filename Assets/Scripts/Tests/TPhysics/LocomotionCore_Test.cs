using MNAC.TPhysics;
using MNAC.TPhysics.Environment;
using MNAC.TPhysics.Locomotion;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace MNAC.Tests.TPhysics
{
    public class LocomotionCore_Test
    {
        class Module : LocomotionModuleBase
        {
            public Module(int priority = 0) : base(priority)
            {
            }
        }

        GameObject _gameObj;
        Rigidbody _rbody;
        IGroundDetector _groundDetector;
        LocomotionCore _lcore;

        [SetUp]
        public void Initialize()
        {
            _gameObj = new GameObject();
            _rbody = _gameObj.AddComponent<Rigidbody>();
            _groundDetector = new GroundDetector(30f, LayerMask.GetMask("Default"));
            _lcore = new LocomotionCore(World.Default, _rbody, _groundDetector);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(_gameObj);
        }

        [Test]
        public void SetModules_OrdersByPriority_Ascending()
        {
            var a = new Module(3);
            var b = new Module(1);
            var c = new Module(2);
            var set = new List<ILocomotionModule> { a, b, c };

            _lcore.SetModules(set);

            var modules = _lcore.Modules;
            Assert.AreEqual(3, modules.Count);
            Assert.AreSame(b, modules[0]);
            Assert.AreSame(c, modules[1]);
            Assert.AreSame(a, modules[2]);
            Assert.IsTrue(_lcore.Contains(a));
            Assert.IsFalse(_lcore.Contains(new Module(0)));
        }

        [Test]
        public void SetModules_SamePriority_KeepsInputOrder()
        {
            var a = new Module(0);
            var b = new Module(0);
            var c = new Module(0);

            _lcore.SetModules(new ILocomotionModule[] { a, b, c });

            var modules = _lcore.Modules;
            Assert.AreSame(a, modules[0]);
            Assert.AreSame(b, modules[1]);
            Assert.AreSame(c, modules[2]);
        }

        [Test]
        public void AddModule_InsertsAtPriorityPosition()
        {
            var a = new Module(2);
            _lcore.SetModules(new ILocomotionModule[] { a });

            var b = new Module(0);
            Assert.IsTrue(_lcore.AddModule(b));

            var modules = _lcore.Modules;
            Assert.AreSame(b, modules[0]);
            Assert.AreSame(a, modules[1]);
        }

        [Test]
        public void PriorityChange_ReordersImmediately()
        {
            var a = new Module(0);
            var b = new Module(1);
            _lcore.SetModules(new ILocomotionModule[] { a, b });

            b.Priority = -1; // 赋值即触发重排，无需等待 Update

            var modules = _lcore.Modules;
            Assert.AreSame(b, modules[0]);
            Assert.AreSame(a, modules[1]);
        }

        [Test]
        public void AddModule_NullOrDuplicate_IsIgnored()
        {
            var a = new Module(0);
            _lcore.SetModules(new ILocomotionModule[] { a });

            Assert.IsFalse(_lcore.AddModule(null));
            Assert.IsFalse(_lcore.AddModule(a));
            Assert.AreEqual(1, _lcore.Modules.Count);
        }

        [Test]
        public void SetModules_MixedPriorities_BinaryInsertsStably()
        {
            var first = new Module(2);
            var sameOne = new Module(2);
            var negative = new Module(-1);
            var zero = new Module(0);

            // 输入顺序与优先级顺序不同，且存在同优先级项
            _lcore.SetModules(new ILocomotionModule[] { first, sameOne, negative, zero });

            var modules = _lcore.Modules;
            Assert.AreSame(negative, modules[0]);
            Assert.AreSame(zero, modules[1]);
            Assert.AreSame(first, modules[2]);
            Assert.AreSame(sameOne, modules[3]);
        }

        [Test]
        public void SetModules_ReplacesEntireList_NoDuplicates()
        {
            var a = new Module();
            var b = new Module();
            _lcore.SetModules(new[] { a, b });
            _lcore.SetModules(new[] { b }); // 全量替换，a 应被移除

            var modules = _lcore.Modules;
            Assert.AreEqual(1, modules.Count);
            Assert.AreSame(b, modules[0]);
        }

        [Test]
        public void SetModules_NullOrEmpty_IsSafe()
        {
            _lcore.SetModules(null);
            Assert.AreEqual(0, _lcore.Modules.Count);

            _lcore.SetModules(new Module[0]);
            Assert.AreEqual(0, _lcore.Modules.Count);

            _lcore.SetModules(null); // 不抛异常
        }

        [Test]
        public void EnableDisableModule_TogglesModuleEnabled_WithoutChangingList()
        {
            var a = new Module();
            _lcore.SetModules(new[] { a });

            _lcore.EnableModule(a);
            Assert.IsTrue(a.Enabled);
            Assert.AreEqual(1, _lcore.Modules.Count);

            _lcore.DisableModule(a);
            Assert.IsFalse(a.Enabled);
            Assert.AreEqual(1, _lcore.Modules.Count);
        }

        [Test]
        public void EnableDisableModule_OnUnknownModule_WarnsAndIsIgnored()
        {
            var a = new Module();
            var stranger = new Module();
            _lcore.SetModules(new[] { a });

            LogAssert.Expect(LogType.Warning, new Regex("不在装配链中"));
            Assert.IsFalse(_lcore.EnableModule(stranger));
            LogAssert.Expect(LogType.Warning, new Regex("不在装配链中"));
            Assert.IsFalse(_lcore.DisableModule(stranger));
            Assert.IsFalse(_lcore.Contains(stranger));
        }

        [Test]
        public void EnableDisableModule_Null_WarnsAndIsIgnored()
        {
            LogAssert.Expect(LogType.Warning, new Regex("模块为空"));
            Assert.IsFalse(_lcore.EnableModule(null));
            LogAssert.Expect(LogType.Warning, new Regex("模块为空"));
            Assert.IsFalse(_lcore.DisableModule(null));
        }
    }
}
