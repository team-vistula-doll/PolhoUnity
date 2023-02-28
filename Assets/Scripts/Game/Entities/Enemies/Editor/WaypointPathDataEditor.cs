using UnityEngine;
using UnityEditor;
using WaypointPath;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    bool isReplace = false;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WaypointPathData pathData = target as WaypointPathData;

        SerializedProperty pathTypeSelection = serializedObject.FindProperty("PathTypeSelection");
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");
            string[] pathOptions = new string[] { "Function", "Bezier" };

            pathTypeSelection.intValue = GUILayout.Toolbar(pathTypeSelection.intValue, pathOptions, EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        switch (pathTypeSelection.intValue)
        {
            case 0:
                SerializedProperty pathFormula = serializedObject.FindProperty("PathFormula");
                SerializedProperty length = serializedObject.FindProperty("Length");
                SerializedProperty angle = serializedObject.FindProperty("Angle");

                EditorGUILayout.PropertyField(pathFormula);
                EditorGUILayout.PropertyField(length);
                EditorGUILayout.PropertyField(angle);

                break;
            case 1:

                SerializedProperty startControl = serializedObject.FindProperty("StartControl");
                SerializedProperty endControl = serializedObject.FindProperty("EndControl");
                SerializedProperty endPosition = serializedObject.FindProperty("EndPosition");

                EditorGUILayout.PropertyField(endPosition);
                EditorGUI.BeginDisabledGroup(endPosition.vector2Value ==  Vector2.zero);
                    EditorGUILayout.PropertyField(startControl);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(startControl.vector2Value == Vector2.zero);
                    EditorGUILayout.PropertyField(endControl);
                EditorGUI.EndDisabledGroup();

                break;
        }
        SerializedProperty stepSize = serializedObject.FindProperty("StepSize");
        EditorGUILayout.PropertyField(stepSize);

        EditorGUILayout.BeginHorizontal();
        {
            isReplace = EditorGUILayout.ToggleLeft("Replace", isReplace);

            if (GUILayout.Button("Set path"))
            {
                pathData.ValidatePath(!isReplace, false);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        pathData.ValidatePath(!isReplace, true);
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        WaypointPathData pathData = target as WaypointPathData;

        float size = HandleUtility.GetHandleSize(pathData.EndPosition) * 0.5f;
        Vector2 snap = Vector2.one * 0.5f;

        EditorGUI.BeginChangeCheck();
        Vector2 newTargetPosition = Handles.FreeMoveHandle(pathData.EndPosition, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(pathData, "Change Control Position");
            pathData.EndPosition = newTargetPosition;
        }
    }
}
