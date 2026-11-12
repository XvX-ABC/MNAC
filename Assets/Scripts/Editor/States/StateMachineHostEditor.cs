using System;
using System.Collections.Generic;
using MNAC.States;
using UnityEditor;
using UnityEngine;

namespace MNAC.Editor.States
{
    /// <summary>
    /// StateMachineHost 的最小 Inspector：状态 × 状态转移矩阵 + 当前过渡 + 条件求值结果。
    /// 只读，不修改状态机；运行时以 10Hz 刷新。
    /// </summary>
    [CustomEditor(typeof(StateMachineHost), true)]
    public class StateMachineHostEditor : UnityEditor.Editor
    {
        const int MaxMatrixStates = 8;
        const float MatrixLabelWidth = 84f;
        const float MinCellWidth = 38f;

        static readonly Color CurrentTint = new Color(0.24f, 0.46f, 0.42f);
        static readonly Color ActiveTint = new Color(0.55f, 0.38f, 0.18f);
        static readonly Color EvenRowTint = new Color(0.80f, 0.80f, 0.80f);

        GUIStyle _title;
        GUIStyle _subtle;
        GUIStyle _currentState;
        GUIStyle _ok;
        GUIStyle _bad;
        GUIStyle _muted;
        GUIStyle _header;
        GUIStyle _headerCurrent;
        GUIStyle _rowHeader;
        GUIStyle _rowHeaderCurrent;
        GUIStyle _cell;
        GUIStyle _cellDim;
        GUIStyle _cellEmpty;

        string _selectedKey;
        bool _showHostFields;
        double _nextRepaint;

        public override void OnInspectorGUI()
        {
            EnsureStyles();

            var host = target as StateMachineHost;
            if (host == null)
                return;

            var inspection = host.Inspection;
            if (inspection == null)
            {
                EditorGUILayout.HelpBox("StateMachineHost.Inspection 返回 null；请先在宿主里创建状态机。", MessageType.Info);
                return;
            }

            var states = inspection.StateSnapshots;
            var transitions = inspection.TransitionSnapshots;

            DrawHeader(inspection, states, transitions);
            EditorGUILayout.Space(4f);
            DrawMatrix(states, transitions);
            EditorGUILayout.Space(4f);
            DrawSelected(inspection, transitions);
            EditorGUILayout.Space(4f);
            DrawActive(inspection);
            EditorGUILayout.Space(4f);
            DrawConditions(inspection, transitions);

            EditorGUILayout.Space(6f);
            _showHostFields = EditorGUILayout.Foldout(_showHostFields, "宿主字段", true);
            if (_showHostFields)
            {
                serializedObject.Update();
                DrawDefaultInspector();
                serializedObject.ApplyModifiedProperties();
            }

            ScheduleRepaint();
        }

        void DrawHeader(IStateMachineInspection inspection, IReadOnlyList<StateSnapshot> states, IReadOnlyList<TransitionSnapshot> transitions)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(inspection.Name, _title);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(
                        $"{states.Count} states · {transitions.Count} transitions",
                        _subtle,
                        GUILayout.Width(160f));
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("当前", _subtle, GUILayout.Width(30f));
                    var current = string.IsNullOrEmpty(inspection.CurrentStateName) ? "(未进入)" : inspection.CurrentStateName;
                    EditorGUILayout.LabelField(current, string.IsNullOrEmpty(inspection.CurrentStateName) ? _muted : _currentState);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(inspection.Enabled ? "Enabled" : "Disabled", inspection.Enabled ? _ok : _bad, GUILayout.Width(70f));
                }
            }
        }

        void DrawMatrix(IReadOnlyList<StateSnapshot> states, IReadOnlyList<TransitionSnapshot> transitions)
        {
            EditorGUILayout.LabelField("转移矩阵（行 → 列）", EditorStyles.boldLabel);

            if (states.Count == 0)
            {
                EditorGUILayout.HelpBox("状态机还没有注册任何状态。", MessageType.Info);
                return;
            }

            if (!CanDrawMatrix(states.Count))
            {
                EditorGUILayout.HelpBox($"状态数 {states.Count} 较多，矩阵改为转移列表。", MessageType.None);
                DrawFallbackList(transitions);
                return;
            }

            var width = Mathf.Max(260f, EditorGUIUtility.currentViewWidth);
            var cellWidth = Mathf.Clamp((width - MatrixLabelWidth - 28f) / states.Count, MinCellWidth, 64f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("源 \\ 目标", _header, GUILayout.Width(MatrixLabelWidth));
                foreach (var state in states)
                {
                    var style = state.IsCurrent ? _headerCurrent : _header;
                    GUILayout.Label(
                        new GUIContent(ShortName(state.Name), $"{state.Name}\n{state.TypeName}\n{(state.Enabled ? "Enabled" : "Disabled")}"),
                        style,
                        GUILayout.Width(cellWidth));
                }
            }

            for (var row = 0; row < states.Count; row++)
            {
                var source = states[row];
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(
                        new GUIContent(ShortName(source.Name), $"{source.Name}\n{source.TypeName}\n{(source.Enabled ? "Enabled" : "Disabled")}"),
                        source.IsCurrent ? _rowHeaderCurrent : _rowHeader,
                        GUILayout.Width(MatrixLabelWidth),
                        GUILayout.Height(26f));

                    for (var column = 0; column < states.Count; column++)
                    {
                        var destination = states[column];
                        var transition = Find(transitions, source.Name, destination.Name);

                        if (source.Name == destination.Name)
                        {
                            DrawCell("—", null, _cellDim, row, cellWidth);
                            continue;
                        }

                        if (!transition.HasValue)
                        {
                            DrawCell("·", null, _cellEmpty, row, cellWidth);
                            continue;
                        }

                        var value = transition.Value;
                        var label = CellLabel(value);
                        var tooltip = $"{value.SourceName} → {value.DestinationName}\n" +
                                      $"时长 {FormatDuration(value.Duration)}\n" +
                                      $"打断 {value.InterruptionMode}\n" +
                                      $"条件 {value.TriggerCount}";

                        var previous = GUI.backgroundColor;
                        GUI.backgroundColor = value.IsActive
                            ? ActiveTint
                            : (source.IsCurrent || destination.IsCurrent ? CurrentTint : Color.white);

                        if (GUILayout.Button(new GUIContent(label, tooltip), _cell, GUILayout.Width(cellWidth), GUILayout.Height(26f)))
                            _selectedKey = Key(value.SourceName, value.DestinationName);

                        GUI.backgroundColor = previous;
                    }
                }
            }

            EditorGUILayout.LabelField("格内为过渡时长；橙色格 = 当前过渡，点击格子查看详情。", _subtle);
        }

        void DrawCell(string label, string tooltip, GUIStyle style, int row, float cellWidth)
        {
            var previous = GUI.backgroundColor;
            if (row % 2 == 1)
                GUI.backgroundColor = EvenRowTint;
            GUILayout.Button(new GUIContent(label, tooltip), style, GUILayout.Width(cellWidth), GUILayout.Height(26f));
            GUI.backgroundColor = previous;
        }

        bool CanDrawMatrix(int stateCount)
        {
            if (stateCount > MaxMatrixStates)
                return false;
            var width = Mathf.Max(260f, EditorGUIUtility.currentViewWidth);
            var cellWidth = (width - MatrixLabelWidth - 28f) / stateCount;
            return cellWidth >= MinCellWidth;
        }

        void DrawFallbackList(IReadOnlyList<TransitionSnapshot> transitions)
        {
            if (transitions.Count == 0)
            {
                EditorGUILayout.LabelField("没有转移。", _subtle);
                return;
            }

            foreach (var transition in transitions)
            {
                var selected = _selectedKey == Key(transition.SourceName, transition.DestinationName);
                var label = $"{transition.SourceName} → {transition.DestinationName}   {FormatDuration(transition.Duration)}   {transition.InterruptionMode}";
                if (GUILayout.Button(label, selected ? EditorStyles.miniButtonMid : EditorStyles.miniButton))
                    _selectedKey = Key(transition.SourceName, transition.DestinationName);
            }
        }

        void DrawSelected(IStateMachineInspection inspection, IReadOnlyList<TransitionSnapshot> transitions)
        {
            if (string.IsNullOrEmpty(_selectedKey))
            {
                if (inspection.IsTransitioning)
                    _selectedKey = Key(inspection.ActiveTransitionSource, inspection.ActiveTransitionDestination);
                else
                    return;
            }

            var selected = FindByKey(transitions, _selectedKey);
            if (!selected.HasValue)
            {
                _selectedKey = null;
                return;
            }

            var value = selected.Value;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("选中转移", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"{value.SourceName} → {value.DestinationName}", _currentState);
                EditorGUILayout.LabelField(
                    $"时长 {FormatDuration(value.Duration)} · 打断 {value.InterruptionMode} · 条件 {value.TriggerCount} · 进度 {Mathf.RoundToInt(value.Progress * 100f)}%",
                    _subtle);
            }
        }

        void DrawActive(IStateMachineInspection inspection)
        {
            if (!inspection.IsTransitioning)
            {
                EditorGUILayout.LabelField("当前没有进行中的过渡。", _subtle);
                return;
            }

            EditorGUILayout.LabelField("当前过渡", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"{inspection.ActiveTransitionSource} → {inspection.ActiveTransitionDestination}", _currentState);
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField($"{Mathf.RoundToInt(inspection.ActiveTransitionProgress * 100f)}%", _ok);
            }

            var rect = GUILayoutUtility.GetRect(10f, 18f, GUILayout.ExpandWidth(true));
            EditorGUI.ProgressBar(rect, Mathf.Clamp01(inspection.ActiveTransitionProgress),
                $"{inspection.ActiveTransitionProgress:0.00}");
        }

        void DrawConditions(IStateMachineInspection inspection, IReadOnlyList<TransitionSnapshot> transitions)
        {
            EditorGUILayout.LabelField("当前状态的出边", EditorStyles.boldLabel);

            if (inspection.IsTransitioning)
            {
                EditorGUILayout.LabelField("过渡中：当前是临时过渡状态，条件结果见上方“选中转移”。", _subtle);
                return;
            }

            var current = inspection.CurrentStateName;
            if (string.IsNullOrEmpty(current))
            {
                EditorGUILayout.LabelField("尚未进入初始状态。", _subtle);
                return;
            }

            var count = 0;
            foreach (var transition in transitions)
            {
                if (transition.SourceName != current)
                    continue;
                count++;

                var result = !transition.HasTriggerResult
                    ? "–"
                    : (transition.LastTriggerResult ? "✓" : "✗");
                var style = !transition.HasTriggerResult ? _muted : (transition.LastTriggerResult ? _ok : _bad);
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField($"{result} {transition.SourceName} → {transition.DestinationName}", style);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(
                        transition.TriggerCount == 0 ? "无条件" : $"{transition.TriggerCount} 条件",
                        _subtle,
                        GUILayout.Width(60f));
                }
            }

            if (count == 0)
                EditorGUILayout.LabelField("当前状态没有出边。", _subtle);
            else
                EditorGUILayout.LabelField("✓/✗ 为状态机上一次实际求值结果，调试器不重复调用触发器。", _subtle);
        }

        static string CellLabel(TransitionSnapshot transition)
        {
            if (transition.IsActive)
                return $"{Mathf.RoundToInt(transition.Progress * 100f)}%";
            if (transition.Duration <= 0f && transition.InterruptionMode == InterruptionMode.None)
                return "直切";
            return $"{transition.Duration:0.##}s";
        }

        static string FormatDuration(float duration) => duration <= 0f ? "直切" : $"{duration:0.##}s";

        static TransitionSnapshot? Find(IReadOnlyList<TransitionSnapshot> transitions, string source, string destination)
        {
            foreach (var transition in transitions)
            {
                if (transition.SourceName == source && transition.DestinationName == destination)
                    return transition;
            }
            return null;
        }

        static TransitionSnapshot? FindByKey(IReadOnlyList<TransitionSnapshot> transitions, string key)
        {
            foreach (var transition in transitions)
            {
                if (Key(transition.SourceName, transition.DestinationName) == key)
                    return transition;
            }
            return null;
        }

        static string Key(string source, string destination) => $"{source}->{destination}";

        static string ShortName(string name)
        {
            const int max = 7;
            if (string.IsNullOrEmpty(name))
                return "?";
            return name.Length <= max ? name : name.Substring(0, max - 1) + "…";
        }

        void ScheduleRepaint()
        {
            if (!Application.isPlaying)
                return;
            var now = EditorApplication.timeSinceStartup;
            if (now < _nextRepaint)
                return;
            _nextRepaint = now + 0.1d;
            Repaint();
        }

        void EnsureStyles()
        {
            if (_title != null)
                return;

            _title = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };
            _subtle = new GUIStyle(EditorStyles.miniLabel) { wordWrap = true };
            _subtle.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.62f, 0.62f, 0.66f)
                : new Color(0.35f, 0.35f, 0.38f);

            _currentState = new GUIStyle(EditorStyles.boldLabel);
            _currentState.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.42f, 0.88f, 0.78f)
                : new Color(0.10f, 0.45f, 0.38f);

            _ok = new GUIStyle(EditorStyles.miniLabel);
            _ok.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.47f, 0.80f, 0.55f)
                : new Color(0.10f, 0.45f, 0.16f);

            _bad = new GUIStyle(EditorStyles.miniLabel);
            _bad.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.90f, 0.45f, 0.45f)
                : new Color(0.60f, 0.10f, 0.10f);

            _muted = new GUIStyle(EditorStyles.miniLabel);
            _muted.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.48f, 0.48f, 0.52f)
                : new Color(0.55f, 0.55f, 0.58f);

            _header = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                wordWrap = false,
            };
            _headerCurrent = new GUIStyle(_header);
            _headerCurrent.normal.textColor = _currentState.normal.textColor;

            _rowHeader = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 10,
            };
            _rowHeaderCurrent = new GUIStyle(_rowHeader);
            _rowHeaderCurrent.normal.textColor = _currentState.normal.textColor;

            _cell = new GUIStyle(EditorStyles.miniButton)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                wordWrap = false,
                padding = new RectOffset(2, 2, 2, 2),
            };
            _cellDim = new GUIStyle(_cell);
            _cellDim.normal.textColor = _muted.normal.textColor;
            _cellEmpty = new GUIStyle(_cell);
            _cellEmpty.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.32f, 0.32f, 0.35f)
                : new Color(0.65f, 0.65f, 0.68f);
        }
    }
}
