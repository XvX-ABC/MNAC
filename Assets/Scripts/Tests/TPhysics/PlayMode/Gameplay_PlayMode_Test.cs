using System.Collections;
using MNAC.Demo.TPhysics.Locomotion;
using MNAC.TPhysics.Environment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MNAC.Tests.TPhysics.PlayMode
{
    /// 转发刚体碰撞 → GroundDetector。
    class GpForwarder : MonoBehaviour
    {
        public GroundDetector Detector;
        void OnCollisionEnter(Collision c) => Detector?.OnCollisionEnter(c);
        void OnCollisionStay(Collision c) => Detector?.OnCollisionStay(c);
        void OnCollisionExit(Collision c) => Detector?.OnCollisionExit(c);
    }

    /// 在 Unity FixedUpdate 内驱动（同 LocomotionRunner 的运行方式，保证 Time.deltaTime=fixedDeltaTime）。
    class GpStepper : MonoBehaviour
    {
        public Rigidbody Rb;
        public LocomotionGameplay Game;
        public GroundDetector Detector;
        void FixedUpdate()
        {
            Detector.Position = Rb.position;
            Detector.OnFixedUpdate();
            Game.Tick();
            Game.Core.Update();
        }
    }

    /// 基础三功能（移动 / 跳跃 / 冲刺）玩法测试：注入确定性输入 → 观测物理结果。
    public class Gameplay_PlayMode_Test
    {
        GameObject _floor;
        GameObject _body;
        Rigidbody _rb;
        GroundDetector _gd;
        LocomotionGameplay _game;

        IEnumerator SetupAsync()
        {
            _floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _floor.name = "Floor";
            _floor.transform.position = new Vector3(0f, -1f, 0f);
            _floor.transform.localScale = new Vector3(40f, 1f, 40f);

            _body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _body.name = "Player";
            _body.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            _body.transform.position = new Vector3(0f, 0.6f, 0f);

            _rb = _body.AddComponent<Rigidbody>();
            _rb.useGravity = true;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;

            // 玩法数值测试关注模块对速度的驱动，关掉地面滑动摩擦以免物理步持续消耗水平速度
            var zeroFriction = new PhysicMaterial
            {
                dynamicFriction = 0f,
                staticFriction = 0f,
                bounciness = 0f,
                frictionCombine = PhysicMaterialCombine.Minimum,
                bounceCombine = PhysicMaterialCombine.Minimum,
            };
            _floor.GetComponent<Collider>().material = zeroFriction;
            _body.GetComponent<Collider>().material = zeroFriction;

            _gd = new GroundDetector(45f, LayerMask.GetMask("Default")) { Enabled = true };
            _body.AddComponent<GpForwarder>().Detector = _gd;

            _game = new LocomotionGameplay(_rb, _gd);
            var stepper = _body.AddComponent<GpStepper>();
            stepper.Game = _game;
            stepper.Rb = _rb;
            stepper.Detector = _gd;

            yield return WaitUntilGrounded(2f);
        }

        IEnumerator WaitUntilGrounded(float timeout)
        {
            var t = 0f;
            while (t < timeout && !_gd.IsGrounded)
            {
                yield return new WaitForFixedUpdate();
                t += Time.fixedDeltaTime;
            }
            Assert.IsTrue(_gd.IsGrounded, "角色应在超时内落地接地");
        }

        [TearDown]
        public void Cleanup()
        {
            if (_body != null) Object.Destroy(_body);
            if (_floor != null) Object.Destroy(_floor);
        }

        [UnityTest]
        public IEnumerator BaseMovement_AcceleratesForward_OnFlatGround()
        {
            yield return SetupAsync();
            var startZ = _rb.position.z;
            float peakHorizontal = 0f;
            float maxVerticalAbs = 0f;
            _game.Input.MoveDirection = Vector3.forward;

            var t = 0f;
            while (t < 1.15f)
            {
                yield return new WaitForFixedUpdate();
                t += Time.fixedDeltaTime;
                var v = _rb.velocity;
                peakHorizontal = Mathf.Max(peakHorizontal, new Vector3(v.x, 0f, v.z).magnitude);
                maxVerticalAbs = Mathf.Max(maxVerticalAbs, Mathf.Abs(v.y));
            }

            var travelled = _rb.position.z - startZ;
            Assert.IsTrue(travelled > 4f, $"约1.15s 应前进明显距离，实际 z 位移 {travelled}");
            Assert.Greater(peakHorizontal, 5f, $"水平速度应接近 Move.MaxSpeed(6)，实测峰值 {peakHorizontal}");
            Assert.Less(maxVerticalAbs, 1.5f, "平地移动时竖直速度应被压平");
        }

        [UnityTest]
        public IEnumerator Jump_LeavesGround_ThenReturnsToGround()
        {
            yield return SetupAsync();
            var restY = _rb.position.y;

            _game.Input.Jump = true;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            _game.Input.Jump = false;

            var airborne = false;
            var apex = float.MinValue;
            var t = 0f;
            while (t < 3f)
            {
                yield return new WaitForFixedUpdate();
                t += Time.fixedDeltaTime;
                if (_rb.position.y > restY + 0.2f)
                {
                    airborne = true;
                    apex = Mathf.Max(apex, _rb.position.y - restY);
                }
                if (airborne && _gd.IsGrounded)
                    break;
            }

            Assert.IsTrue(airborne, "跳跃应使角色离地");
            Assert.Greater(apex, 0.8f, $"跳跃高度不足，实测最高 {apex}（JumpLocomotion(1.8) 期望 >0.8）");
            Assert.IsTrue(_gd.IsGrounded, "跳跃后应回到地面");
        }

        [UnityTest]
        public IEnumerator Dash_BurstSpeedExceedsMoveCap_AndStaysLow()
        {
            yield return SetupAsync();
            _game.Input.MoveDirection = Vector3.forward;
            var t = 0f;
            while (t < 0.3f)
            {
                yield return new WaitForFixedUpdate();
                t += Time.fixedDeltaTime;
            }

            _game.Input.Dash = true;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            _game.Input.Dash = false;

            float peakHorizontal = 0f;
            float maxVerticalAbs = 0f;
            t = 0f;
            while (t < 0.8f)
            {
                yield return new WaitForFixedUpdate();
                t += Time.fixedDeltaTime;
                var v = _rb.velocity;
                peakHorizontal = Mathf.Max(peakHorizontal, new Vector3(v.x, 0f, v.z).magnitude);
                maxVerticalAbs = Mathf.Max(maxVerticalAbs, Mathf.Abs(v.y));
            }

            Assert.Greater(peakHorizontal, 8f, $"冲刺应显著超过移动上限(6)，实测峰值 {peakHorizontal}");
            Assert.Less(maxVerticalAbs, 2f, "冲刺/移动时竖直分量应被压平");
        }
    }
}
