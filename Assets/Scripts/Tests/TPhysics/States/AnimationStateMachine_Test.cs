using MNAC.States;
using NUnit.Framework;
using UnityEngine;

namespace MNAC.Tests.TPhysics
{
    /// <summary>AnimationStateMachine 的 IK 转发与 Enabled 门控回归。</summary>
    public class AnimationStateMachine_Test
    {
        class TestAnimState : AnimationStateBase<string>
        {
            public int IKs;

            public TestAnimState(string name, Animator animator) : base(name, animator)
            {
            }

            public override void OnEnter()
            {
            }

            public override void OnUpdate(float deltaTime)
            {
            }

            public override void OnExit()
            {
            }

            public override void OnAnimationIK(int layerIndex) => IKs++;
        }

        GameObject _go;
        Animator _animator;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("AnimatorHost");
            _animator = _go.AddComponent<Animator>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null)
                Object.DestroyImmediate(_go);
        }

        [Test]
        public void OnAnimatorIK_ForwardsToCurrentState_AndHonorsEnabled()
        {
            var state = new TestAnimState("idle", _animator);
            var machine = new AnimationStateMachine<string>("anim");
            machine.AddState(state);
            machine.SetInitialState(state);
            machine.OnEnter();

            machine.OnAnimatorIK(0);
            Assert.AreEqual(1, state.IKs);

            machine.Enabled = false;
            machine.OnAnimatorIK(0);
            Assert.AreEqual(1, state.IKs, "状态机禁用时不应转发 IK");
        }

        [Test]
        public void OnAnimatorIK_ForwardsThroughNestedMachine()
        {
            var state = new TestAnimState("idle", _animator);
            var nested = new AnimationStateMachine<string>("nested");
            nested.AddState(state);
            nested.SetInitialState(state);

            var outer = new AnimationStateMachine<string>("outer");
            outer.AddState(nested);
            outer.SetInitialState(nested);
            outer.OnEnter();

            outer.OnAnimatorIK(0);
            Assert.AreEqual(1, state.IKs);
        }
    }
}
