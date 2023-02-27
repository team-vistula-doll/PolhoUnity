using UnityEngine;
using UnityEditor;
using WaypointPath;

[CustomEditor(typeof(WaypointWalker))]
public class WaypointWalkerEditor : Editor
{
    bool isReplace = false;

    private WaypointPathData waypointPathData = new();
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WaypointWalker waypointWalker = (WaypointWalker)target;

        EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Path type: ");
            string[] pathOptions = new string[] { "Function", "Bezier" };
            waypointPathData.PathTypeSelection = GUILayout.Toolbar(waypointPathData.PathTypeSelection, pathOptions, EditorStyles.radioButton);
        EditorGUILayout.EndHorizontal();

        switch (waypointPathData.PathTypeSelection)
        {
            case 0:
                EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Path Formula");
                    GUILayout.FlexibleSpace();
                    waypointPathData.PathFormula = EditorGUILayout.DelayedTextField(waypointPathData.PathFormula);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Length");
                    GUILayout.FlexibleSpace();
                    waypointPathData.Length = EditorGUILayout.FloatField(waypointPathData.Length);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Angle");
                    GUILayout.FlexibleSpace();
                    waypointPathData.Angle = EditorGUILayout.FloatField(waypointPathData.Angle);
                EditorGUILayout.EndHorizontal();

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
                waypointPathData.ValidatePath(!isReplace, false);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        waypointPathData.ValidatePath(!isReplace, true);
        SceneView.RepaintAll();
    }

}
