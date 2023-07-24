using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isInsert, pathTypeSelection;
    const string assetPath = "Assets/Editor Assets/WaypointPathEditorData.asset";
    PathEditor PathEditor { get => WaypointPathEditorData.Options[(int)data.PathTypeSelection]; }

    WaypointPathData pathData;
    SerializedProperty path;

    private void OnEnable()
    {
        pathData = target as WaypointPathData;
        path = serializedObject.FindProperty("Path");
        data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        serialData = new SerializedObject(data);

        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isInsert = serialData.FindProperty("IsInsert");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
    }

    private void OnDisable()
    {
        if (!AssetDatabase.Contains(data)) AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serialData.Update();


        PathEditor.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref pathData);

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");

            pathTypeSelection.intValue = GUILayout.Toolbar(
                pathTypeSelection.intValue, Enum.GetNames(typeof(PathType)), EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        PathEditor.PathOptions();

        EditorGUILayout.BeginHorizontal();
        {
            isInsert.boolValue = EditorGUILayout.ToggleLeft("Insert after", isInsert.boolValue);

            if (GUILayout.Button("Set path"))
            {
                //List<Vector2> path = PathEditor.MakePath(data.IsReplace || serialPathData.arraySize == 1);

                //Setting the new path in the edited object through serializedObject
                if (path.arraySize <= 1)
                {
                    path.ClearArray();
                    path.arraySize++;
                }
                if (isInsert.boolValue)
                {
                    path.InsertArrayElementAtIndex(selectedPathIndex.intValue);
                }

                path.GetArrayElementAtIndex(selectedPathIndex.intValue).objectReferenceValue = PathEditor.GetPathCreator();

                //If isInsert true, then start from inserted element
                for (int i = selectedPathIndex.intValue + Convert.ToInt32(!isInsert.boolValue);  i < path.arraySize; i++)
                {
                    WaypointPathCreator x = (WaypointPathCreator)path.GetArrayElementAtIndex(i - 1).objectReferenceValue;
                    path.GetArrayElementAtIndex(i).FindPropertyRelative("StartPosition").vector2Value =
                        x.GetEndVector();
                }
                
                //foreach (Vector2 v in path)
                //{
                //    serialPathData.arraySize++;
                //    serialPathData.GetArrayElementAtIndex(serialPathData.arraySize - 1) = v;
                //}
            }
        }
        EditorGUILayout.EndHorizontal();

        data.TempPath = PathEditor.MakePath(path.arraySize <= 1);

        //DrawDefaultInspector();
        DrawPropertiesExcluding(serializedObject, "m_Script");

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        Event e = Event.current;

        List<Vector2> vector2s = new();
        foreach (var path in pathData.Path) vector2s.AddRange(path.GeneratePath());
        PathEditor.DrawPath(in vector2s, e, in data);
    }
}
