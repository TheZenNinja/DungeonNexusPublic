using Player;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZenEditor
{
    //[CustomPropertyDrawer(typeof(PlayerSkillController.AbilityDrawer))]
    public class AbilityDrawerGUI : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var amountRect = new Rect(position.x, position.y, 30, position.height);
            var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
            var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);
            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("input"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("skillSO"), GUIContent.none);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("cooldownUI"), GUIContent.none);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("skill"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
        //public override VisualElement CreatePropertyGUI(SerializedProperty property)
        //{
        //    var container = new VisualElement();
        //    container.Add(new Label(property.displayName));
        //    //var box = new Box();
        //    //container.Add(box);
        //
        //    var input = new PropertyField(property.FindPropertyRelative("input"));
        //    var skillSO = new PropertyField(property.FindPropertyRelative("skillSO"));
        //    var cooldownUI = new PropertyField(property.FindPropertyRelative("cooldownUI"));
        //    var skill = new PropertyField(property.FindPropertyRelative("skill"));
        //
        //    container.Add(input);
        //    container.Add(skillSO);
        //    container.Add(cooldownUI);
        //    container.Add(skill);
        //
        //    return container;
        //}
    }
}
