using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MNAC.Demo.TPhysics.Locomotion
{
    /// <summary>
    /// A 方案试验场的 IMGUI HUD：当前预设、模块启停、速度、接地、姿态、距离和峰值。
    /// F1 隐藏/显示。只读 LocomotionTestDriver，不参与运动逻辑。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LocomotionTestHud : MonoBehaviour
    {
        public LocomotionTestDriver driver;
        public LocomotionTestCamera cameraRig;
        public bool visible = true;

        GUIStyle _panel;
        GUIStyle _title;
        GUIStyle _label;
        GUIStyle _small;
        Texture2D _panelTexture;

        void Awake()
        {
            if (driver == null)
                driver = FindObjectOfType<LocomotionTestDriver>();
            if (cameraRig == null)
                cameraRig = FindObjectOfType<LocomotionTestCamera>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                visible = !visible;
        }

        void OnDestroy()
        {
            if (_panelTexture != null)
                Destroy(_panelTexture);
        }

        void OnGUI()
        {
            if (!visible || driver == null)
                return;

            EnsureStyles();

            float scale = Mathf.Clamp(Screen.height / 720f, 1f, 3f);
            var previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1f));

            const float width = 390f;
            const float height = 535f;
            GUI.Box(new Rect(8f, 8f, width, height), GUIContent.none, _panel);

            GUILayout.BeginArea(new Rect(18f, 16f, width - 20f, height - 24f));
            GUILayout.Label("LOCOMOTION TEST LAB", _title);
            GUILayout.Label($"预设 [{((int)driver.CurrentPreset) + 1}] {driver.CurrentPresetName}", _label);
            if (cameraRig != null)
                GUILayout.Label($"相机 {cameraRig.ModeName}  [C] 切换", _small);

            GUILayout.Space(4f);
            GUILayout.Label($"速度 {driver.Speed:F2}   H {driver.HorizontalSpeed:F2} / V {driver.VerticalSpeed:F2}", _label);
            GUILayout.Label($"接地 {(driver.IsGrounded ? "是" : "否")}   法线 {(driver.GroundAngle >= 0f ? $"{driver.GroundAngle:F1}°" : "—")}", _label);
            GUILayout.Label($"姿态 {driver.Posture}   Drag {driver.CurrentDrag:F2}", _label);
            GUILayout.Label($"距离 {driver.DistanceFromSpawn:F1}m   时间 {driver.TimeInRun:F2}s", _label);
            GUILayout.Label($"峰值 H {driver.PeakHorizontalSpeed:F2}   最高点 {driver.JumpApex:F2}m", _label);
            GUILayout.Label($"10m {FormatTime(driver.TimeTo10m)}   20m {FormatTime(driver.TimeTo20m)}   急停 {driver.LastStopDistance:F2}m", _label);

            GUILayout.Space(4f);
            GUILayout.Label("预设  " + BuildPresetLine(), _small);
            GUILayout.Label("模块  " + BuildModuleLine(driver.ActiveModules), _small);
            GUILayout.Label($"目标  {driver.TargetLabel}", _small);

            GUILayout.Space(4f);
            GUILayout.Label("WASD 移动   Space 跳跃   Shift 冲刺", _small);
            GUILayout.Label(driver.CurrentPreset == LocomotionTestDriver.Preset.Stop
                ? "Stop：松开 WASD 自动急停；Ctrl 手动急停"
                : "Ctrl 急停   Q 切换目标   E 鼠标目标", _small);
            GUILayout.Label("R 重置   C 相机   1-8 预设   F1 隐藏 HUD", _small);

            GUILayout.EndArea();
            GUI.matrix = previousMatrix;
        }

        void EnsureStyles()
        {
            if (_panel != null)
                return;

            _panel = new GUIStyle(GUI.skin.box);
            _panelTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
            };
            _panelTexture.SetPixel(0, 0, new Color(0.04f, 0.07f, 0.09f, 0.94f));
            _panelTexture.Apply();
            _panel.normal.background = _panelTexture;
            _title = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                richText = true,
            };
            _title.normal.textColor = new Color(0.5f, 0.95f, 0.85f);
            _label = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                richText = true,
            };
            _label.normal.textColor = new Color(0.88f, 0.93f, 0.96f);
            _small = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                richText = true,
                wordWrap = true,
            };
            _small.normal.textColor = new Color(0.68f, 0.78f, 0.84f);
        }

        static string BuildPresetLine()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < LocomotionTestDriver.PresetLabels.Length; i++)
            {
                sb.Append('[').Append(i + 1).Append(']')
                    .Append(LocomotionTestDriver.PresetLabels[i]);
                if (i < LocomotionTestDriver.PresetLabels.Length - 1)
                    sb.Append("  ");
            }
            return sb.ToString();
        }

        static string BuildModuleLine(IReadOnlyList<MNAC.TPhysics.Locomotion.ILocomotionModule> modules)
        {
            if (modules == null || modules.Count == 0)
                return "空链";

            var sb = new StringBuilder();
            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                if (module == null)
                    continue;
                if (i > 0)
                    sb.Append("  ");
                sb.Append(module.Enabled ? "●" : "○").Append(module.GetType().Name);
            }
            return sb.ToString();
        }

        static string FormatTime(float seconds) => seconds >= 0f ? $"{seconds:F2}s" : "—";
    }
}
