using MNAC.TPhysics.Environment;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.TestTools;
namespace MNAC.TPhysics.Locomotion.Tests
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
        List<Module> _moduleList;
        void FillModuleToList(int amount, bool randomPriority)
        {
            var fixedPriority = Random.Range(byte.MinValue, byte.MaxValue);
            for (int i = 0; i < amount; i++)
            {
                var module = new Module(randomPriority ? Random.Range(byte.MinValue, byte.MaxValue) : fixedPriority);
                _moduleList.Add(module);
            }
        }
        [SetUp]
        public void Initialize()
        {
            _gameObj = new GameObject();
            _rbody = _gameObj.AddComponent<Rigidbody>();
            _groundDetector = new GroundDetector(30, LayerMask.NameToLayer("Terrain"));

            _moduleList = new();

            _lcore = new(World.Default, _rbody, _groundDetector);
        }
        [SetUp]
        public void OneTimeInitialize()
        {
            var totalAmount = 5;
            var randomAmount = Random.Range(1, totalAmount - 1);
            var fixedAmount = totalAmount - randomAmount;

            FillModuleToList(totalAmount, true);
            FillModuleToList(fixedAmount, false);


            for (int i = 0; i < _moduleList.Count; i++)
            {
                _lcore.AddModule_InsertByPriority(_moduleList[i]);
            }
        }
        [TearDown]
        public void OneTimeCleanup()
        {
            for (int i = 0; i < _moduleList.Count; i++)
            {
                _lcore.RemoveModule(_moduleList[i]);
            }
            _moduleList.Clear();

        }
        [Test]
        [Repeat(5)]
        public void CheckPriority()
        {
            var sb = new StringBuilder();
            var priorityList = new List<int>();
            sb.Append("The priorities waiting to add the list: ");
            foreach (var m in _lcore.Modules)
            {
                priorityList.Add(m.Priority);
                sb.Append(m.Priority.ToString());
                sb.Append(", ");
            }
            Debug.Log(sb.ToString());
            int currentMaxPriority = int.MinValue;
            foreach (var p in priorityList)
            {
                Assert.GreaterOrEqual(p, currentMaxPriority, "");
                currentMaxPriority = p;
            }
        }
    }
}