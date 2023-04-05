using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WaypointPath;

[CustomEditor(typeof(CurrentStageEnemies))]
[CanEditMultipleObjects]
public class CurrentStageEnemiesEditor : Editor
{
    SerializedProperty enemies;

    WaypointPathData pathData = new();
    BezierControlPoints bezier = new(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
    ExpressionProperties expression = new("x", 20, 0);
    [Range(0.2f, 50f)]
    public float StepSize = 0.5f;

    List<Vector2> tempPath = new();
    bool isReplace = false;
    int pathTypeSelection = 0;

    private void OnEnable()
    {
        enemies = serializedObject.FindProperty("enemies");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CurrentStageEnemies stageEnemies = target as CurrentStageEnemies;

        bezier.StartPosition = EditorGUILayout.Vector2Field("Start Position", bezier.StartPosition);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");
            string[] pathOptions = new string[] { "Function", "Bezier" };

            pathTypeSelection = GUILayout.Toolbar(pathTypeSelection, pathOptions, EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        switch (pathTypeSelection)
        {
            case 0:

                expression.PathFormula = EditorGUILayout.DelayedTextField("Path Formula", expression.PathFormula);
                expression.Length = EditorGUILayout.FloatField("Length", expression.Length);
                expression.Angle = EditorGUILayout.FloatField("Angle", expression.Angle);

                break;
            case 1:

                bezier.EndPosition = EditorGUILayout.Vector2Field("End Position", bezier.EndPosition);
                EditorGUI.BeginDisabledGroup(bezier.EndPosition == Vector2.zero);
                    bezier.EndControl = EditorGUILayout.Vector2Field("End Control", bezier.EndControl);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(bezier.EndControl == Vector2.zero);
                    bezier.StartControl = EditorGUILayout.Vector2Field("Start Control", bezier.StartControl);
                EditorGUI.EndDisabledGroup();

                break;
        }
        StepSize = EditorGUILayout.FloatField("Step Size", StepSize);

        EditorGUILayout.BeginHorizontal();
        {
            isReplace = EditorGUILayout.ToggleLeft("Replace", isReplace);

            if (GUILayout.Button("Set path"))
            {
                WaypointPathCreator.SetWaypointPath(pathTypeSelection, !isReplace);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        tempPath = WaypointPathCreator.CreateWaypointPath((isReplace) ? pathData.StartPosition : pathData.Path.Last(), pathTypeSelection);
        SceneView.RepaintAll();

        //base.OnInspectorGUI();
    }
}
