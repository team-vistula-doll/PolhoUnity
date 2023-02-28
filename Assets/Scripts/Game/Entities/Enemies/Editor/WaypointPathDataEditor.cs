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
        WaypointPathData pathData = (WaypointPathData)target;

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

                EditorGUILayout.PropertyField(startControl);
                EditorGUILayout.PropertyField(endControl);
                EditorGUILayout.PropertyField(endPosition);

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

}
