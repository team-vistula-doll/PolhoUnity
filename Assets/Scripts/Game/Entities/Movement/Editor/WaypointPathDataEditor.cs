using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;
    const string assetPath = "Assets/Editor Assets/WaypointPathEditorData.asset";

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isInsert, pathTypeSelection;
    bool tempIsInsert = false;
    PathEditor PathEditor { get => WaypointPathEditorData.Options[(int)data.PathTypeSelection]; }

    WaypointPathData pathData;
    List<WaypointPathCreator> tempPath;

    SerializedProperty path;

    private void OnEnable()
    {
        pathData = target as WaypointPathData;
        path = serializedObject.FindProperty("Path");
        tempPath = pathData.Path.ToList();
        data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        serialData = new SerializedObject(data);
        PathEditor.objectTransform = pathData.transform;

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

        if (pathData.transform.hasChanged) PathEditor.objectTransform = pathData.transform;

        PathEditor.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref pathData);

        selectedPathIndex.intValue -= PathEditor.DeletePath(ref path);
        if (selectedPathIndex.intValue < 0) selectedPathIndex.intValue = 0;

        EditorGUILayout.Space();
        PathEditor.PathTypes(ref pathTypeSelection);

        PathEditor.PathOptions();

        PathEditor.SetPath(ref path, ref isInsert, ref selectedPathIndex);
        
        if (!isInsert.boolValue && tempIsInsert) tempIsInsert = false;
        if ((isInsert.boolValue && !tempIsInsert) || tempPath.Count == 0 || selectedPathIndex.intValue >= tempPath.Count)
        {
            tempPath.Insert(selectedPathIndex.intValue, Instantiate(PathEditor.GetPathCreator()));
            tempIsInsert = true;
        }
        else tempPath[selectedPathIndex.intValue] = Instantiate(PathEditor.GetPathCreator());
        PathEditor.ConnectPaths(ref tempPath, selectedPathIndex.intValue);

        EditorGUILayout.Space();

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        EventType e = Event.current.type;
        if (e != EventType.Repaint) return;
        List<Vector2> vector2s = PathEditor.CreateVectorPath(in pathData.Path, 0);
        PathEditor.DrawPath(ref pathData.Path, 0, e, false);
        PathEditor.DrawPath(ref tempPath, selectedPathIndex.intValue, e, true);
    }
}
