using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(JuicySlider))]
[CanEditMultipleObjects]
public class JuicySliderEditor : SliderEditor
{
    SerializedProperty _spring;

    protected override void OnEnable()
    {
        base.OnEnable();
        _spring = serializedObject.FindProperty("spring");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_spring, true);
        serializedObject.ApplyModifiedProperties();
    }
}
