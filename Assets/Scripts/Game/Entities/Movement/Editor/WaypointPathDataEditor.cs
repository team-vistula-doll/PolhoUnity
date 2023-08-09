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

    WaypointPathData pathData;
    SerializedObject serialPath;
    List<WaypointPathCreator> tempPath;

    private void Awake()
    {
        pathData = target as WaypointPathData;
        tempPath = new();
        foreach (var creator in pathData.Path)
            tempPath.Add(creator.GetNewAdjoinedPath(0));
        if (pathData.Path.Count > 0) tempPath.Add(pathData.Path.Last().GetNewAdjoinedPath(0));
        else tempPath.Add(new WaypointPathExpression());
    }

    private void OnEnable()
    {
        serialPath = serializedObject;
        data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        if (data.SelectedPathIndex < tempPath.Count) data.SelectedOption.SetPathCreator(tempPath[data.SelectedPathIndex]);
        serialData = new SerializedObject(data);
        data.SelectedOption.objectTransform = pathData.transform;
        pathData.transform.hasChanged = false;

        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isInsert = serialData.FindProperty("IsInsert");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
    }

    private void OnDisable()
    {
        if (!AssetDatabase.Contains(data)) AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();
        //if (PrefabStageUtility.GetPrefabStage(pathData.gameObject) == null) return;
        ////    PathEditor.SavePrefab(ref pathData);
        //if (pathData != null && pathData.Path != null)
        //{
        //    foreach (WaypointPathCreator wpc in pathData.Path)
        //        EditorUtility.SetDirty(wpc);
        //}
        //if (PrefabUtility.HasPrefabInstanceAnyOverrides(pathData.gameObject, false))
        //{
        //    PrefabUtility.ApplyPrefabInstance(pathData.gameObject, InteractionMode.AutomatedAction);
        //    EditorSceneManager.MarkSceneDirty(pathData.gameObject.scene);
        //}
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serialData.Update();
        var pathCreator = data.SelectedOption.GetPathCreator();

        if (pathData.transform.hasChanged)
        {
            data.SelectedOption.objectTransform = pathData.transform;
            data.SelectedOption.ConnectPaths(ref serialPath, in pathData.Path, 0);
            data.SelectedOption.ConnectPaths(ref tempPath, 0);
            pathData.transform.hasChanged = false;
        }

        if (data.SelectedOption.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref tempPath, pathData.Path.Count))
        {
            if (selectedPathIndex.intValue < pathData.Path.Count)
                tempPath[selectedPathIndex.intValue] = pathCreator.GetNewAdjoinedPath(0);
            data.SelectedOption.startDeleteIndex = data.SelectedOption.endDeleteIndex = selectedPathIndex.intValue;
        }

        selectedPathIndex.intValue -= data.SelectedOption.DeletePath(ref serialPath, in pathData.Path, ref tempPath);
        if (selectedPathIndex.intValue < 0) selectedPathIndex.intValue = 0;

        EditorGUILayout.Space();
        PathEditor.PathTypes(ref pathTypeSelection);

        bool pathOptionsChanged = data.SelectedOption.PathOptions();

        if (!isInsert.boolValue && tempIsInsert && tempPath.Count > 0 && selectedPathIndex.intValue < tempPath.Count)
        {
            tempPath.RemoveAt(selectedPathIndex.intValue);
            tempIsInsert = false;
        }
        if ((isInsert.boolValue && !tempIsInsert) || tempPath.Count == 0 || selectedPathIndex.intValue >= tempPath.Count)
        {
            tempPath.Insert(selectedPathIndex.intValue, pathCreator.GetNewAdjoinedPath(0));
            if ((isInsert.boolValue && !tempIsInsert)) tempIsInsert = true;
        }
        //else if (pathOptionsChanged) tempPath[selectedPathIndex.intValue] = pathCreator;
        if (pathOptionsChanged) data.SelectedOption.ConnectPaths(ref tempPath, selectedPathIndex.intValue);

        data.SelectedOption.SetPath(ref serialPath, in pathData.Path, ref isInsert, ref selectedPathIndex, ref tempPath);

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script");

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        EventType e = Event.current.type;
        if (e != EventType.Repaint) return;
        //List<Vector2> vector2s = PathEditor.CreateVectorPath(in pathData.Path, 0);
        data.SelectedOption.DrawPath(ref pathData.Path, 0, e, false);
        data.SelectedOption.DrawPath(ref tempPath, selectedPathIndex.intValue, e, true);
    }
}
