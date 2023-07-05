using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data = new();
    PathProperties properties { get { return data.Creator.Properties; } set { data.Creator.Properties = value; } }

    string[] pathOptions = new string[] { "Function", "Bezier" };
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WaypointPathData pathData = target as WaypointPathData;

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");

            data.PathTypeSelection = GUILayout.Toolbar(data.PathTypeSelection, data.Options.Keys.ToArray(), EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        data.Options.ElementAt(data.PathTypeSelection).Value.PathOptions();

        EditorGUILayout.Slider("Step size", data.StepSize, 0.2f, 50f);

        EditorGUILayout.BeginHorizontal();
        {
            data.IsReplace = EditorGUILayout.ToggleLeft("Replace", data.IsReplace);

            if (GUILayout.Button("Set path"))
            {
                List<Vector2> path = data.Options.ElementAt(data.PathTypeSelection).Value.
                    MakePath((data.IsReplace || pathData.Path.Count() == 0), data.StepSize);
                if (data.IsReplace || pathData.Path.Count() == 0) pathData.Path = path;
                else pathData.Path.AddRange(path);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        data.TempPath.Path = data.Options.ElementAt(data.PathTypeSelection).Value.
            MakePath((data.IsReplace || pathData.Path.Count() == 0), data.StepSize);
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        WaypointPathData pathData = target as WaypointPathData;
        Event e = Event.current;

        data.Options.ElementAt(data.PathTypeSelection).Value.DrawPath(ref pathData, e, ref data.TempPath, data.IsReplace);
    }
}
