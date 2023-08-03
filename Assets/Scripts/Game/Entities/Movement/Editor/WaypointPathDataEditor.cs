using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;
    const string assetPath = "Assets/Editor Assets/WaypointPathEditorData.asset";

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isInsert, pathTypeSelection;
    bool tempIsInsert = false;

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
        data.SelectedOption.objectTransform = pathData.transform;

        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isInsert = serialData.FindProperty("IsInsert");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
    }

    private void OnDisable()
    {
        if (!AssetDatabase.Contains(data)) AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();
        if (PrefabStageUtility.GetPrefabStage(pathData.gameObject) == null) return;
        //    PathEditor.SavePrefab(ref pathData);
        if (pathData != null && pathData.Path != null)
        {
            foreach (WaypointPathCreator wpc in pathData.Path)
                EditorUtility.SetDirty(wpc);
        }
        if (PrefabUtility.HasPrefabInstanceAnyOverrides(pathData.gameObject, false))
            PrefabUtility.ApplyPrefabInstance(pathData.gameObject, InteractionMode.AutomatedAction);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serialData.Update();

        if (pathData.transform.hasChanged) data.SelectedOption.objectTransform = pathData.transform;

        if (data.SelectedOption.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref pathData))
            data.SelectedOption.startDeleteIndex = data.SelectedOption.endDeleteIndex = selectedPathIndex.intValue;

        selectedPathIndex.intValue -= data.SelectedOption.DeletePath(ref path);
        if (selectedPathIndex.intValue < 0) selectedPathIndex.intValue = 0;

        EditorGUILayout.Space();
        PathEditor.PathTypes(ref pathTypeSelection);

        data.SelectedOption.PathOptions();

        data.SelectedOption.SetPath(ref path, ref isInsert, ref selectedPathIndex);
        
        if (!isInsert.boolValue && tempIsInsert && tempPath.Count > 0 && selectedPathIndex.intValue < tempPath.Count)
        {
            tempPath.RemoveAt(selectedPathIndex.intValue);
            tempIsInsert = false;
        }
        if ((isInsert.boolValue && !tempIsInsert) || tempPath.Count == 0 || selectedPathIndex.intValue >= tempPath.Count)
        {
            tempPath.Insert(selectedPathIndex.intValue, Instantiate(data.SelectedOption.GetPathCreator()));
            tempIsInsert = true;
        }
        else tempPath[selectedPathIndex.intValue] = Instantiate(data.SelectedOption.GetPathCreator());
        data.SelectedOption.ConnectPaths(ref tempPath, selectedPathIndex.intValue);

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
        List<Vector2> vector2s = PathEditor.CreateVectorPath(in pathData.Path, 0);
        data.SelectedOption.DrawPath(ref pathData.Path, 0, e, false);
        data.SelectedOption.DrawPath(ref tempPath, selectedPathIndex.intValue, e, true);
    }
}
