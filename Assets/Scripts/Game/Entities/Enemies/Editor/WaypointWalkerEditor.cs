using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointWalker))]
public class WaypointWalkerEditor : Editor
{
    bool isReplace = false;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        int pathTypeSelection = serializedObject.FindProperty("PathTypeSelection").intValue;
        WaypointWalker waypointWalker = (WaypointWalker)target;

        EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Path type: ");
            string[] pathOptions = new string[] { "Function", "Bezier" };
            pathTypeSelection = GUILayout.Toolbar(pathTypeSelection, pathOptions, EditorStyles.radioButton);
        EditorGUILayout.EndHorizontal();

        switch (pathTypeSelection)
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
                SerializedProperty endPos = serializedObject.FindProperty("EndPos");


                EditorGUILayout.PropertyField(startControl);
                EditorGUILayout.PropertyField(endControl);
                EditorGUILayout.PropertyField(endPos);

                break;
        }
        SerializedProperty stepSize = serializedObject.FindProperty("StepSize");
        EditorGUILayout.PropertyField(stepSize);

        EditorGUILayout.BeginHorizontal();
        {
            isReplace = EditorGUILayout.ToggleLeft("Replace", isReplace);

            if (GUILayout.Button("Set path"))
            {
                waypointWalker.ValidatePath(!isReplace, false);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.FindProperty("PathTypeSelection").intValue = pathTypeSelection;
        serializedObject.ApplyModifiedProperties();

        waypointWalker.ValidatePath(!isReplace, true);
        SceneView.RepaintAll();
    }

}
