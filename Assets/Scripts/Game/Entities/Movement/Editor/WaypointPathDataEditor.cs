using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;
    PathEditor pathEditor { get { return data.Options.ElementAt(data.PathTypeSelection).Value; } }

    WaypointPathData pathData;

    public void OnEnable()
    {
        pathData = target as WaypointPathData;
        data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");

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
                List<Vector2> path = pathEditor.MakePath(data.IsReplace, data.StepSize);
                if (data.IsReplace || pathData.Path.Count() == 0) pathData.Path = path;
                else pathData.Path.AddRange(path);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        data.TempPath = pathEditor.MakePath(data.IsReplace, data.StepSize);
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        Event e = Event.current;

        pathEditor.DrawPath(ref pathData.Path, e, ref data.TempPath, data.IsReplace);
    }
}
