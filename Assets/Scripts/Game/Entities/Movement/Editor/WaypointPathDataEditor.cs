using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Atn;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;

    //SerializedObject serialData;
    //SerializedProperty selectedPathIndex, isInsert, pathTypeSelection;
    //const string assetPath = "Assets/Editor Assets/WaypointPathEditorData.asset";
    //PathEditor PathEditor { get => WaypointPathEditorData.Options[(int)data.PathTypeSelection]; }

    WaypointPathData pathData;
    List<WaypointPathCreator> tempPath;

    SerializedProperty path;

    private void OnEnable()
    {
        data = WaypointPathEditorData.instance;
        pathData = target as WaypointPathData;
        path = serializedObject.FindProperty("Path");
        tempPath = pathData.Path.ToList();
        //data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        //if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        //serialData = new SerializedObject(data);
        PathEditor.objectTransform = pathData.transform;

        //selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        //isInsert = serialData.FindProperty("IsInsert");
        //pathTypeSelection = serialData.FindProperty("PathTypeSelection");
    }

    //private void OnDisable()
    //{
    //    if (!AssetDatabase.Contains(data)) AssetDatabase.CreateAsset(data, assetPath);
    //    AssetDatabase.SaveAssets();
    //}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //serialData.Update();

        if (pathData.transform.hasChanged) PathEditor.objectTransform = pathData.transform;

        data.SelectedOption.SelectPath(ref data.SelectedPathIndex, ref data.PathTypeSelection, ref pathData);

        data.SelectedPathIndex -= data.SelectedOption.DeletePath(ref path);
        if (data.SelectedPathIndex < 0) data.SelectedPathIndex = 0;

        EditorGUILayout.Space();
        PathEditor.PathTypes(ref data.PathTypeSelection);

        data.SelectedOption.PathOptions();

        if (data.IsInsert || tempPath.Count == 0 || data.SelectedPathIndex >= tempPath.Count)
            tempPath.Insert(data.SelectedPathIndex, data.SelectedOption.GetPathCreator().GetNewAdjoinedPath(0));
        else tempPath[data.SelectedPathIndex] = data.SelectedOption.GetPathCreator().GetNewAdjoinedPath(0);
        data.SelectedOption.ConnectPaths(ref tempPath, data.SelectedPathIndex);

        data.SelectedOption.SetPath(ref path, ref data.IsInsert, ref data.SelectedPathIndex);

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script");

        //serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        EventType e = Event.current.type;
        if (e != EventType.Repaint) return;
        //List<Vector2> vector2s = PathEditor.CreateVectorPath(in pathData.Path, 0);
        data.SelectedOption.DrawPath(ref pathData.Path, 0, e, false);
        data.SelectedOption.DrawPath(ref tempPath, data.SelectedPathIndex, e, true);
    }
}
