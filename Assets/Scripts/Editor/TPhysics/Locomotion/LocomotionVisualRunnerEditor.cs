using System;
using System.Linq;
using MNAC.TPhysics.Locomotion;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MNAC.Editor.TPhysics.Locomotion
{
    /// <see cref="LocomotionVisualRunner"/> 的可视装配面板：模块清单增删/拖拽排序/启停 + 选中模块参数编辑。
    /// 顺序即运行时执行顺序（LocomotionCore.SetModules 不做自动排序）。
    [CustomEditor(typeof(LocomotionVisualRunner))]
    [CanEditMultipleObjects]
    public class LocomotionVisualRunnerEditor : UnityEditor.Editor
    {
        static readonly Type[] ModuleTypes = CollectModuleTypes();

        ReorderableList _modulesList;
        SerializedProperty _modulesProp;
        int _selected = -1;

        void OnEnable()
        {
            _modulesProp = serializedObject.FindProperty("modules");
            _modulesList = new ReorderableList(serializedObject, _modulesProp,
                draggable: true, displayHeader: false, displayAddButton: false, displayRemoveButton: false)
            {
                drawElementCallback = DrawModuleRow,
                onAddDropdownCallback = OnAddModuleDropdown,
                onRemoveCallback = RemoveModule,
                onSelectCallback = l => _selected = l.index,
                elementHeightCallback = i => 21f,
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetRigidbody"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("environmentSettings"), true);

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("运动模块（顺序 = 执行顺序，勾选 = 启用）", EditorStyles.boldLabel);

            if (_modulesList == null)
                OnEnable();

            if (_modulesProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("尚未装配任何模块。点下方 [+ Add Module] 添加。", MessageType.Info);
            }
            else
            {
                _modulesList.DoLayoutList();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Module"))
            {
                ShowAddMenu(GUILayoutUtility.GetLastRect());
            }
            if (_modulesList.count > 0 && GUILayout.Button("Clear"))
            {
                _modulesProp.ClearArray();
                _selected = -1;
            }
            EditorGUILayout.EndHorizontal();

            DrawSelectedModuleDetails();

            serializedObject.ApplyModifiedProperties();

            if (target == null)
                return;
            var runner = (LocomotionVisualRunner)target;
            if (runner.Core == null)
                EditorGUILayout.HelpBox("运行时（Play）后，此清单会喂给 LocomotionCore。", MessageType.None);
        }

        void DrawModuleRow(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _modulesProp.GetArrayElementAtIndex(index);
            var module = element.managedReferenceValue as ILocomotionModule;
            var typeName = module != null ? Nicify(module.GetType().Name) : "<null>";

            float toggleWidth = 18f;
            float removeWidth = 18f;
            var toggleRect = new Rect(rect.x, rect.y, toggleWidth, EditorGUIUtility.singleLineHeight);
            var nameRect = new Rect(rect.x + toggleWidth, rect.y, rect.width - toggleWidth - removeWidth, EditorGUIUtility.singleLineHeight);
            var removeRect = new Rect(rect.xMax - removeWidth, rect.y, removeWidth, EditorGUIUtility.singleLineHeight);

            bool enabled = module != null && module.Enabled;
            bool newEnabled = GUI.Toggle(toggleRect, enabled, GUIContent.none);
            if (module != null && newEnabled != enabled)
            {
                module.Enabled = newEnabled;
                element.managedReferenceValue = module;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            var wasSelected = _selected == index;
            if (GUI.Button(nameRect, typeName, wasSelected ? EditorStyles.boldLabel : EditorStyles.label))
            {
                _selected = wasSelected ? -1 : index;
            }

            if (GUI.Button(removeRect, "x"))
                RemoveModule(_modulesList);
        }

        void DrawSelectedModuleDetails()
        {
            if (_selected < 0 || _selected >= _modulesProp.arraySize)
                return;
            var element = _modulesProp.GetArrayElementAtIndex(_selected);
            if (element.managedReferenceValue == null)
                return;

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField($"参数 — {Nicify(element.managedReferenceValue.GetType().Name)}", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            var child = element.Copy();
            bool enterChildren = true;
            while (child.NextVisible(enterChildren))
            {
                EditorGUILayout.PropertyField(child, true);
                enterChildren = false;
            }
            EditorGUI.indentLevel--;
        }

        void RemoveModule(ReorderableList list)
        {
            if (list.index < 0 || list.index >= _modulesProp.arraySize)
                return;
            var element = _modulesProp.GetArrayElementAtIndex(list.index);
            if (element.managedReferenceValue != null)
                _modulesProp.DeleteArrayElementAtIndex(list.index); // 删对象元素
            _modulesProp.DeleteArrayElementAtIndex(list.index);    // 清掉空位
            if (_selected >= _modulesProp.arraySize)
                _selected = _modulesProp.arraySize - 1;
            serializedObject.ApplyModifiedProperties();
        }

        void OnAddModuleDropdown(Rect buttonRect, ReorderableList list)
        {
            ShowAddMenu(buttonRect);
        }

        void ShowAddMenu(Rect position)
        {
            var menu = new GenericMenu();
            foreach (var type in ModuleTypes)
                menu.AddItem(new GUIContent(Nicify(type.Name)), false, () => AppendModule(type));
            if (ModuleTypes.Length == 0)
                menu.AddDisabledItem(new GUIContent("无可用模块"));
            menu.DropDown(position);
        }

        void AppendModule(Type type)
        {
            var module = Activator.CreateInstance(type) as ILocomotionModule;
            if (module == null)
                return;

            int index = _modulesProp.arraySize;
            _modulesProp.InsertArrayElementAtIndex(index);
            var element = _modulesProp.GetArrayElementAtIndex(index);
            element.managedReferenceValue = module;
            _selected = index;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        static Type[] CollectModuleTypes()
        {
            return TypeCache.GetTypesDerivedFrom<ILocomotionModule>()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => Attribute.IsDefined(t, typeof(SerializableAttribute), false))
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        static string Nicify(string name) => ObjectNames.NicifyVariableName(name);
    }
}
