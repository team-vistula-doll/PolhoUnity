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

    WaypointPathEditorData data;
    PathEditor PathEditor { get { return data.Options.ElementAt(data.PathTypeSelection).Value; } }

    List<Vector2> pathData = new();
    Vector2 startPosition = Vector2.zero;

    private void OnEnable()
    {
        enemies = serializedObject.FindProperty("enemies");
        data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
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

        PathEditor.PathOptions();

        data.StepSize = EditorGUILayout.Slider("Step size", data.StepSize, 0.2f, 50f);

        EditorGUILayout.BeginHorizontal();
        {
            data.IsReplace = EditorGUILayout.ToggleLeft("Replace", data.IsReplace);

            if (GUILayout.Button("Set path"))
            {
                List<Vector2> path = PathEditor.MakePath(data.IsReplace || pathData.Count() == 1, data.StepSize);
                if (data.IsReplace || pathData.Count() == 1) pathData = path;
                else pathData.AddRange(path);

                
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        data.TempPath = PathEditor.MakePath(data.IsReplace || pathData.Count() == 1, data.StepSize);
        SceneView.RepaintAll();

        //base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        Event e = Event.current;

        PathEditor.DrawPath(in pathData, e, in data);
    }
}
