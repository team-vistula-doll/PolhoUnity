using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isInsert, pathTypeSelection;
    const string assetPath = "Assets/Editor Assets/WaypointPathEditorData.asset";
    PathEditor PathEditor { get => WaypointPathEditorData.Options[(int)data.PathTypeSelection]; }

    WaypointPathData pathData, tempPathData;

    SerializedProperty path;

    private void OnEnable()
    {
        pathData = target as WaypointPathData;
        tempPathData = Object.Instantiate(pathData);
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

        PathEditor.DeletePath(ref path);

        EditorGUILayout.Space();
        PathEditor.PathTypes(ref pathTypeSelection);

        PathEditor.PathOptions();

        if (isInsert.boolValue || tempPathData.Path.Count == 0 || selectedPathIndex.intValue >= tempPathData.Path.Count)
            tempPathData.Path.Insert(selectedPathIndex.intValue, PathEditor.GetPathCreator());
        else tempPathData.Path[selectedPathIndex.intValue] = PathEditor.GetPathCreator();
        PathEditor.ConnectPaths(ref tempPathData.Path, selectedPathIndex.intValue);

        PathEditor.SetPath(ref path, ref isInsert, ref selectedPathIndex);

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script");

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        EventType e = Event.current.type;
        List<Vector2> vector2s = PathEditor.CreateVectorPath(in pathData.Path, 0);
        PathEditor.DrawPath(ref pathData.Path, 0, e, false);
        PathEditor.DrawPath(ref tempPathData.Path, selectedPathIndex.intValue, e, true);
    }
}
