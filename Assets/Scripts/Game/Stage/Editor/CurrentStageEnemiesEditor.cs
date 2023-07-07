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

    WaypointPathEditorData data = new();
    PathEditor pathEditor { get { return data.Options.ElementAt(data.PathTypeSelection).Value; } }

    WaypointPathData pathData = new WaypointPathData();
    Vector2 startPosition = Vector2.zero;

    private void OnEnable()
    {
        enemies = serializedObject.FindProperty("enemies");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        startPosition = EditorGUILayout.Vector2Field("Start Position", startPosition);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");
            string[] pathOptions = new string[] { "Function", "Bezier" };

            data.PathTypeSelection = GUILayout.Toolbar(data.PathTypeSelection, data.Options.Keys.ToArray(), EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        pathEditor.PathOptions();

        data.StepSize = EditorGUILayout.Slider("Step size", data.StepSize, 0.2f, 50f);

        EditorGUILayout.BeginHorizontal();
        {
            data.IsReplace = EditorGUILayout.ToggleLeft("Replace", data.IsReplace);

            if (GUILayout.Button("Set path"))
            {
                List<Vector2> path = pathEditor.MakePath((data.IsReplace || pathData.Path.Count() == 0), data.StepSize);
                if (data.IsReplace || pathData.Path.Count() == 0) pathData.Path = path;
                else pathData.Path.AddRange(path);

                
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        data.TempPath.Path = pathEditor.MakePath((data.IsReplace || pathData.Path.Count() == 0), data.StepSize);
        SceneView.RepaintAll();

        //base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        Event e = Event.current;

        pathEditor.DrawPath(ref pathData, e, ref data.TempPath, data.IsReplace);
    }
}
