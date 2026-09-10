using MNAC.TPhysics;
using MNAC.TPhysics.Environment;
using MNAC.TPhysics.Locomotion;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

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
        public void Modules_OrderMatchesSetModules_InputOrder()
        {
            var a = new Module(3);
            var b = new Module(1);
            var c = new Module(2);
            var set = new List<ILocomotionModule> { a, b, c };

            _lcore.SetModules(set);

            var modules = _lcore.Modules;
            Assert.AreEqual(3, modules.Count);
            Assert.AreSame(a, modules[0]);
            Assert.AreSame(b, modules[1]);
            Assert.AreSame(c, modules[2]);
            Assert.IsTrue(_lcore.Contains(a));
            Assert.IsFalse(_lcore.Contains(new Module(0)));
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
        public void EnableDisableModule_OnUnknownModule_IsNoOp()
        {
            var a = new Module();
            var stranger = new Module();
            _lcore.SetModules(new[] { a });

            Assert.DoesNotThrow(() => _lcore.EnableModule(stranger));
            Assert.DoesNotThrow(() => _lcore.DisableModule(stranger));
            Assert.IsFalse(_lcore.Contains(stranger));
        }
    }
}
