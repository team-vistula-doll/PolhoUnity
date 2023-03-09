using UnityEditor;
using UnityEngine;
using WaypointPath;

[CustomEditor(typeof(CurrentStageEnemies))]
[CanEditMultipleObjects]
public class CurrentStageEnemiesEditor : Editor
{
    SerializedProperty enemies;
    bool isReplace = false;
    WaypointPathData pathData = new();

    private void OnEnable()
    {
        enemies = serializedObject.FindProperty("enemies");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CurrentStageEnemies stageEnemies = target as CurrentStageEnemies;

        pathData.StartPosition = EditorGUILayout.Vector2Field("Start Position", pathData.StartPosition);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");
            string[] pathOptions = new string[] { "Function", "Bezier" };

            pathData.PathTypeSelection = GUILayout.Toolbar(pathData.PathTypeSelection, pathOptions, EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        switch (pathData.PathTypeSelection)
        {
            case 0:

                pathData.PathFormula = EditorGUILayout.DelayedTextField("Path Formula", pathData.PathFormula);
                pathData.Length = EditorGUILayout.FloatField("Length", pathData.Length);
                pathData.Angle = EditorGUILayout.FloatField("Angle", pathData.Angle);

                break;
            case 1:

                pathData.EndPosition = EditorGUILayout.Vector2Field("End Position", pathData.EndPosition);
                EditorGUI.BeginDisabledGroup(pathData.EndPosition == Vector2.zero);
                    pathData.EndControl = EditorGUILayout.Vector2Field("End Control", pathData.EndControl);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(pathData.EndControl == Vector2.zero);
                    pathData.StartControl = EditorGUILayout.Vector2Field("Start Control", pathData.StartControl);
                EditorGUI.EndDisabledGroup();

                break;
        }
        pathData.StepSize = EditorGUILayout.FloatField("Step Size", pathData.StepSize);

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

        //base.OnInspectorGUI();
    }
}
