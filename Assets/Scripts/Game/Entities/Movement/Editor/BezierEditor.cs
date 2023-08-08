using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class BezierEditor : PathEditor
    {
        [SerializeField]
        WaypointPathBezier pathBezier = new(Vector2.zero, Vector2.zero, Vector2.zero ,Vector2.zero);
        //SerializedObject serialPath;
        [SerializeField]
        Vector2 endPosition, startControl, endControl;
        //const string assetPath = "Assets/Editor Assets/BezierEditor.asset";

        bool isMousePressed = false;
        bool isEndControlEnabled = false;
        //bool isStartControlEnabled = false;

        //private void Awake()
        //{
        //    pathBezier = (WaypointPathBezier)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathBezier));
        //    if (pathBezier == null) pathBezier = (WaypointPathBezier)ScriptableObject.CreateInstance(typeof(WaypointPathBezier));
        //}

        private void OnEnable()
        {
            //pathBezier = (WaypointPathBezier)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathBezier));
            //if (pathBezier == null) pathBezier = (WaypointPathBezier)ScriptableObject.CreateInstance(typeof(WaypointPathBezier));
            //serialPath = new SerializedObject(pathBezier);
            Undo.undoRedoPerformed += ApplyPathOptions;

            stepSize = pathBezier.StepSize;
            startPosition = pathBezier.StartPosition;
            endPosition = pathBezier.EndPosition;
            startControl = pathBezier.StartControl;
            endControl = pathBezier.EndControl;
        }

        //private void OnDisable()
        //{
        //    if (!AssetDatabase.Contains(pathBezier)) AssetDatabase.CreateAsset(pathBezier, assetPath);
        //    AssetDatabase.SaveAssets();
        //}

        public override WaypointPathCreator GetPathCreator() => pathBezier;

        public override bool SelectPath(ref SerializedProperty selectedPathIndex, ref SerializedProperty pathTypeSelection,
            ref WaypointPathData pathData)
        {
            bool pathExists = base.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref pathData);
            if (!pathExists) return false;

            //serialPath.Update();
            WaypointPathBezier selectedPath = (WaypointPathBezier)pathData.Path[selectedPathIndex.intValue];
            startPosition = selectedPath.StartPosition;
            endPosition = selectedPath.EndPosition;
            startControl = selectedPath.StartControl;
            endControl = selectedPath.EndControl;
            //serialPath.ApplyModifiedProperties();
            return true;
        }

        public override bool PathOptions()
        {
            bool changed = false;
            Undo.RecordObject(this, "Edit path options");

            EditorGUI.BeginChangeCheck();
            endPosition = EditorGUILayout.Vector2Field("End Position", endPosition);
            EditorGUI.BeginDisabledGroup(endPosition == Vector2.zero);
                endControl = EditorGUILayout.Vector2Field("End Control", endControl);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(endControl == Vector2.zero);
                startControl = EditorGUILayout.Vector2Field("Start Control", startControl);
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
                ApplyPathOptions();
            }

            base.PathOptions();

            return changed;
        }

        protected override void ApplyPathOptions()
        {
            base.ApplyPathOptions();
            pathBezier.EndPosition = endPosition;
            pathBezier.StartControl = startControl;
            pathBezier.EndControl = endControl;
        }

        protected override void SetPathProperties(ref SerializedProperty sp)
        {
            sp.FindPropertyRelative(nameof(WaypointPathBezier.StartPosition)).vector2Value = startPosition;
            sp.FindPropertyRelative(nameof(WaypointPathBezier.EndPosition)).vector2Value = endPosition;
            sp.FindPropertyRelative(nameof(WaypointPathBezier.StartControl)).vector2Value = startControl;
            sp.FindPropertyRelative(nameof(WaypointPathBezier.EndControl)).vector2Value = endControl;
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
            if (!isTemp) return;

            WaypointPathBezier temp = pathBezier;
            pathBezier = (WaypointPathBezier)path[startIndex];

            EditorGUI.BeginChangeCheck();

            Vector2 snap = Vector2.one * 0.2f;
            float size;
            Vector2 startControlHandle = startPosition;
            Vector2 endControlHandle = startPosition;

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
            if (e == EventType.MouseUp && endPosition != Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = true;
                isMousePressed = false;
            }
            else if (e == EventType.MouseUp && endPosition == Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = false;
                isMousePressed = false;
            }
            //***

            //if (isStartControlEnabled)
            //{
            size = HandleUtility.GetHandleSize(startControl) * 0.15f;
            Handles.color = Color.cyan;
            startControlHandle = Handles.FreeMoveHandle(startControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            //}
            //else startControlHandle = Vector2.zero;

            if (isEndControlEnabled)
            {
                size = HandleUtility.GetHandleSize(endControl) * 0.15f;
                Handles.color = Color.cyan;
                endControlHandle = Handles.FreeMoveHandle(endControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            }
            else endControlHandle = Vector2.zero;

            size = HandleUtility.GetHandleSize(endPosition) * 0.2f;
            Handles.color = Color.red;
            Vector2 endPositionHandle = Handles.FreeMoveHandle(endPosition, Quaternion.identity, size, snap, Handles.SphereHandleCap);

            Handles.color = new Color(1, 0, 0, 0.5f);
            if (startControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(startControlHandle, startPosition, 2);
                Handles.color = new Color(1, 0.92f, 0.016f, 0.5f);
                Handles.DrawLine(endControlHandle, startControlHandle);
            }
            else if (endControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(endControlHandle, startPosition, 2);
            }

            if (EditorGUI.EndChangeCheck())
            {
                //Undo.RecordObject(pathBezier, "Change Handle Position");
                endPosition = endPositionHandle;
                startControl = startControlHandle;
                endControl = endControlHandle;
                //serialPath.ApplyModifiedProperties();
                pathBezier = temp;
                //var path = (data.IsInsert) ? pathBezier : pathBezier.GetModifiedCurveCopy(pathBezier.EndControl, (x, y) => x + y);
                //data.TempPath = path.GeneratePath();
            }
        }
    }
}