using UnityEngine;

namespace MNAC.Demo.TPhysics.Locomotion
{
    /// <summary>
    /// 测试场景相机：跟随 / 侧视 / 顶视三种视角，按 C 切换。
    /// 侧视用于看跳跃高度，顶视用于看转向和横向位移。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LocomotionTestCamera : MonoBehaviour
    {
        public enum ViewMode
        {
            Follow,
            Side,
            Top,
        }

        [SerializeField] Transform target;
        [SerializeField] Vector3 followOffset = new Vector3(0f, 10f, -16f);
        [SerializeField] float lookHeight = 1.2f;

        public ViewMode Mode { get; private set; } = ViewMode.Follow;
        public string ModeName => Mode switch
        {
            ViewMode.Follow => "跟随",
            ViewMode.Side => "侧视",
            ViewMode.Top => "顶视",
            _ => "?",
        };

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
                Mode = (ViewMode)(((int)Mode + 1) % 3);
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            switch (Mode)
            {
                case ViewMode.Follow:
                {
                    var wanted = target.position + followOffset;
                    transform.position = Vector3.Lerp(transform.position, wanted, 1f - Mathf.Exp(-10f * Time.unscaledDeltaTime));
                    transform.LookAt(target.position + Vector3.up * lookHeight);
                    break;
                }
                case ViewMode.Side:
                {
                    var wanted = new Vector3(22f, 7f, target.position.z);
                    transform.position = Vector3.Lerp(transform.position, wanted, 1f - Mathf.Exp(-10f * Time.unscaledDeltaTime));
                    transform.LookAt(new Vector3(0f, lookHeight, target.position.z));
                    break;
                }
                case ViewMode.Top:
                {
                    var wanted = new Vector3(0f, 30f, target.position.z);
                    transform.position = Vector3.Lerp(transform.position, wanted, 1f - Mathf.Exp(-10f * Time.unscaledDeltaTime));
                    transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    break;
                }
            }
        }
    }
}
