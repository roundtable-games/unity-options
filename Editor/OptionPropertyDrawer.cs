using UnityEditor;
using UnityEngine;

namespace Roundtable.Utilities
{
    /// <summary>
    ///     Custom <see cref="PropertyDrawer"/> for <see cref="Option{T}"/> types.
    /// </summary>
    [CustomPropertyDrawer(typeof(Option<>))]
    public sealed class OptionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hasValueProperty = property.FindPropertyRelative("_hasValue");

            var toggleArea = position;
            toggleArea.width = EditorGUIUtility.labelWidth;
            toggleArea.height = EditorGUIUtility.singleLineHeight;

            hasValueProperty.boolValue = EditorGUI.ToggleLeft(toggleArea, label, hasValueProperty.boolValue);
            if (!hasValueProperty.boolValue)
                return;

            var valueProperty = property.FindPropertyRelative("_value");
            if (IsSingleLineProperty(valueProperty))
            {
                DrawValuePropertyInline(position, valueProperty);
            }
            else
            {
                DrawValuePropertyUnderneath(position, valueProperty);
            }
        }

        private static void DrawValuePropertyInline(Rect position, SerializedProperty property)
        {
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;
            EditorGUI.PropertyField(position, property, GUIContent.none);
        }

        private static void DrawValuePropertyUnderneath(Rect position, SerializedProperty property)
        {
            EditorGUI.indentLevel++;

            position.y += EditorGUIUtility.singleLineHeight;
            position.height -= EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property);

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hasValueProperty = property.FindPropertyRelative("_hasValue");

            var height = EditorGUIUtility.singleLineHeight;
            if (!hasValueProperty.boolValue)
                return height;

            var valueProperty = property.FindPropertyRelative("_value");
            if (IsSingleLineProperty(valueProperty))
                return height;

            height += EditorGUI.GetPropertyHeight(valueProperty, includeChildren: true);
            return height;
        }

        private static bool IsSingleLineProperty(SerializedProperty property)
        {
            if (property.isArray)
                return false;

            var height = EditorGUI.GetPropertyHeight(property, includeChildren: true);
            bool result = (height == EditorGUIUtility.singleLineHeight);
            return result;
        }
    }
}
