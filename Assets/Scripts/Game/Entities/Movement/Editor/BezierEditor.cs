using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class BezierEditor : PathEditor
    {
        WaypointPathBezier pathBezier = new();
        //SerializedObject serialPath;
        //SerializedProperty startPosition, endPosition, startControl, endControl;
        //const string assetPath = "Assets/Editor Assets/BezierEditor.asset";

        bool isMousePressed = false;
        bool isEndControlEnabled = false;
        //bool isStartControlEnabled = false;

        //private void Awake()
        //{
        //    if (pathBezier == null) pathBezier ??= new();
        //    //serialPath = new SerializedObject(pathBezier);

        //    //stepSize = serialPath.FindProperty("StepSize");
        //    //startPosition = serialPath.FindProperty("StartPosition");
        //    //endPosition = serialPath.FindProperty("EndPosition");
        //    //startControl = serialPath.FindProperty("StartControl");
        //    //endControl = serialPath.FindProperty("EndControl");
        //}

        //private void OnEnable()
        //{
        //    pathBezier = (WaypointPathBezier)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathBezier));
        //    if (pathBezier == null)
        //        pathBezier = (WaypointPathBezier)ScriptableObject.CreateInstance(typeof(WaypointPathBezier));
        //    serialPath = new SerializedObject(pathBezier);

        //    stepSize = serialPath.FindProperty("StepSize");
        //    startPosition = serialPath.FindProperty("StartPosition");
        //    endPosition = serialPath.FindProperty("EndPosition");
        //    startControl = serialPath.FindProperty("StartControl");
        //    endControl = serialPath.FindProperty("EndControl");
        //}

        //private void OnDisable()
        //{
        //    if (!AssetDatabase.Contains(pathBezier)) AssetDatabase.CreateAsset(pathBezier, assetPath);
        //    AssetDatabase.SaveAssets();
        //}

        public override WaypointPathCreator GetPathCreator() => pathBezier;

        public override bool SelectPath(ref int selectedPathIndex, ref PathType pathTypeSelection,
            ref WaypointPathData pathData)
        {
            bool pathExists = base.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref pathData);
            if (!pathExists) return false;

            //serialPath.Update();
            WaypointPathBezier selectedPath = (WaypointPathBezier)pathData.Path[selectedPathIndex];
            pathBezier.StartPosition = selectedPath.StartPosition;
            pathBezier.EndPosition = selectedPath.EndPosition;
            pathBezier.StartControl = selectedPath.StartControl;
            pathBezier.EndControl = selectedPath.EndControl;
            //serialPath.ApplyModifiedProperties();
            return true;
        }

        public override void PathOptions()
        {
            //serialPath.Update();

            pathBezier.EndPosition = EditorGUILayout.Vector2Field("End Position", pathBezier.EndPosition);
            EditorGUI.BeginDisabledGroup(pathBezier.EndPosition == Vector2.zero);
                pathBezier.EndControl = EditorGUILayout.Vector2Field("End Control", pathBezier.EndControl);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(pathBezier.EndControl == Vector2.zero);
                pathBezier.StartControl = EditorGUILayout.Vector2Field("Start Control", pathBezier.StartControl);
            EditorGUI.EndDisabledGroup();

            base.PathOptions();

            //serialPath.ApplyModifiedProperties();
        }

        //public override List<Vector2> MakePath(bool isAddedAtEnd = false)
        //{
        //    if (isAddedAtEnd)
        //    {
        //        var value = (WaypointPathBezier)pathBezier.GetNewAdjoinedPath(1);
        //        return value.GeneratePath();
        //    }
        //    return pathBezier.GeneratePath();
        //}

        public override void DrawPath(ref List<WaypointPathCreator> path, int startIndex, EventType e, bool isTemp = false)
        {
            base.DrawPath(ref path, startIndex, e, isTemp);
            if (!isTemp || path.Count == 0) return;

            WaypointPathBezier temp = pathBezier;
            pathBezier = (WaypointPathBezier)path[startIndex];

            EditorGUI.BeginChangeCheck();

            Vector2 snap = Vector2.one * 0.2f;
            float size;
            Vector2 startControlHandle = pathBezier.StartPosition;
            Vector2 endControlHandle = pathBezier.StartPosition;

            if (e == EventType.MouseDown) isMousePressed = true;

            //If the End Control handle is right under the End Position handle, the EC handle has priority in selecting it,
            //however this doesn't happen with the Start Control for some reason, both can overlap it and it won't get selected
            //The commented-out code for the SC handle also doesn't work, even though it's the same as for the EC handle

            ////***Prevent Start Control handle from getting selected when End Control is zero
            //if (e.type == EventType.MouseUp && pathData.EndControl != Vector2.zero && isMousePressed)
            //{
            //    isStartControlEnabled = true;
            //    isMousePressed = false;
            //}
            //else if (e.type == EventType.MouseUp && pathData.EndControl == Vector2.zero && isMousePressed)
            //{
            //    isStartControlEnabled = false;
            //    isMousePressed = false;
            //}
            ////***

            //***Prevent End Control handle from getting selected when End Position is zero
            if (e == EventType.MouseUp && pathBezier.EndPosition != Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = true;
                isMousePressed = false;
            }
            else if (e == EventType.MouseUp && pathBezier.EndPosition == Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = false;
                isMousePressed = false;
            }
            //***

            //if (isStartControlEnabled)
            //{
            size = HandleUtility.GetHandleSize(pathBezier.StartControl) * 0.15f;
            Handles.color = Color.cyan;
            startControlHandle = Handles.FreeMoveHandle(pathBezier.StartControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            //}
            //else startControlHandle = Vector2.zero;

            if (isEndControlEnabled)
            {
                size = HandleUtility.GetHandleSize(pathBezier.EndControl) * 0.15f;
                Handles.color = Color.cyan;
                endControlHandle = Handles.FreeMoveHandle(pathBezier.EndControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            }
            else endControlHandle = Vector2.zero;

            size = HandleUtility.GetHandleSize(pathBezier.EndPosition) * 0.2f;
            Handles.color = Color.red;
            Vector2 endPositionHandle = Handles.FreeMoveHandle(pathBezier.EndPosition, Quaternion.identity, size, snap, Handles.SphereHandleCap);

            Handles.color = new Color(1, 0, 0, 0.5f);
            if (startControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(startControlHandle, pathBezier.StartPosition, 2);
                Handles.color = new Color(1, 0.92f, 0.016f, 0.5f);
                Handles.DrawLine(endControlHandle, startControlHandle);
            }
            else if (endControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(endControlHandle, pathBezier.StartPosition, 2);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pathBezier, "Change Handle Position");
                pathBezier.EndPosition = endPositionHandle;
                pathBezier.StartControl = startControlHandle;
                pathBezier.EndControl = endControlHandle;
                pathBezier = temp;
                //serialPath.ApplyModifiedProperties();
                //var path = (data.IsInsert) ? pathBezier : pathBezier.GetModifiedCurveCopy(pathBezier.EndControl, (x, y) => x + y);
                //data.TempPath = path.GeneratePath();
            }
        }
    }
}