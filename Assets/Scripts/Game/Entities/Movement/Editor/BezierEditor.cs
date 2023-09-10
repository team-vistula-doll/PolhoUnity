using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class BezierEditor : PathEditor
    {
        [SerializeField]
        WaypointPathBezier pathBezier = new(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
        //SerializedObject serialPath;
        [SerializeField]
        Vector2 endPosition, startControl, endControl;
        //const string assetPath = "Assets/Editor Assets/BezierEditor.asset";

        bool isMousePressed = false;
        bool isEndControlEnabled = false;
        //bool isStartControlEnabled = false;

        bool isLocal = true;

        protected override void OnEnable()
        {
            base.OnEnable();

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
        public override void SetPathCreator(WaypointPathCreator pathCreator)
        {
            pathBezier = (WaypointPathBezier)pathCreator;
            base.SetPathCreator(pathBezier);
            endPosition = pathBezier.EndPosition;
            endControl = pathBezier.EndControl;
            startControl = pathBezier.StartControl;
        }

        //public override bool SelectPath(ref SerializedProperty selectedPathIndex, ref SerializedProperty pathTypeSelection,
        //    ref List<WaypointPathCreator> tempPath, int pathCount)
        //{
        //    bool pathExists = base.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref tempPath, pathCount);
        //    if (!pathExists) return false;

        //    //serialPath.Update();
        //    pathBezier = (WaypointPathBezier)tempPath[selectedPathIndex.intValue];
        //    startPosition = pathBezier.StartPosition;
        //    endPosition = pathBezier.EndPosition;
        //    startControl = pathBezier.StartControl;
        //    endControl = pathBezier.EndControl;
        //    //serialPath.ApplyModifiedProperties();
        //    return true;
        //}

        public override bool PathOptions()
        {
            bool changed = false;
            Undo.RecordObject(this, "Edit path options");

            EditorGUI.BeginChangeCheck();
            isLocal = EditorGUILayout.ToggleLeft("Local coords", isLocal);
            Vector2 localStart = isLocal ? startPosition : Vector2.zero;
            Vector2 endPos, endCon, startCon; //There's a bug with Undoing enemy movement if you use the class fields directly

            endPos = EditorGUILayout.Vector2Field("End Position", endPosition - localStart) + localStart;
            EditorGUI.BeginDisabledGroup(endPosition - startPosition == Vector2.zero);
                endCon = EditorGUILayout.Vector2Field("End Control", endControl - localStart) + localStart;
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(endControl - startPosition == Vector2.zero);
                startCon = EditorGUILayout.Vector2Field("Start Control", startControl - localStart) + localStart;
            EditorGUI.EndDisabledGroup();
            base.PathOptions();

            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
                endPosition = endPos;
                endControl = endCon;
                startControl = startCon;
                ApplyPathOptions();
            }


            return changed;
        }

        public override void ApplyPathOptions()
        {
            base.ApplyPathOptions();
            pathBezier.EndPosition = endPosition;
            pathBezier.EndControl = endControl;
            pathBezier.StartControl = startControl;
        }

        public override void DrawPath(List<WaypointPathCreator> path, int startIndex, EventType e, bool isTemp = false)
        {
            base.DrawPath(path, startIndex, e, isTemp);
            if (!isTemp) return;

            //WaypointPathBezier temp = pathBezier;
            //pathBezier = (WaypointPathBezier)path[startIndex];

            EditorGUI.BeginChangeCheck();

            Vector2 snap = Vector2.one * 0.2f;
            float size;
            Vector2 startControlHandle = startControl;
            Vector2 endControlHandle = endControl;
            if (endControlHandle - startPosition != Vector2.zero) isEndControlEnabled = true;

            if (e == EventType.MouseDown) isMousePressed = true;

            //If the End Control handle is right under the End Position handle, the EC handle has priority in selecting it,
            //however this doesn't happen with the Start Control for some reason, both can overlap it and it won't get selected
            //The commented-out code for the SC handle also doesn't work, even though it's the same as for the EC handle

            //***Prevent Start Control handle from getting selected when End Control is zero
            //if (e == EventType.MouseUp && endControl != Vector2.zero && isMousePressed)
            //{
            //    isStartControlEnabled = true;
            //    isMousePressed = false;
            //}
            //else if (e == EventType.MouseUp && endControl == Vector2.zero && isMousePressed)
            //{
            //    isStartControlEnabled = false;
            //    isMousePressed = false;
            //}
            //***

            //***Prevent End Control handle from getting selected when End Position is zero
            if (e == EventType.MouseUp && endPosition - startPosition != Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = true;
                isMousePressed = false;
            }
            else if (e == EventType.MouseUp && endPosition - startPosition == Vector2.zero && isMousePressed)
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
            Handles.color = Color.yellow;
            Vector2 endPositionHandle = Handles.FreeMoveHandle(endPosition, Quaternion.identity, size, snap, Handles.SphereHandleCap);

            Handles.color = new Color(1, 0, 0, 0.5f); //red
            if (startControlHandle - startPosition != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f); //blue
                Handles.DrawLine(startControlHandle, startPosition, 2);
                Handles.color = new Color(1, 0.92f, 0.016f, 0.5f); //yellow
                Handles.DrawLine(endControlHandle, startControlHandle);
            }
            else if (endControlHandle - startPosition != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f); //blue
                Handles.DrawLine(endControlHandle, startPosition, 2);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this, "Change Handle Position");
                endPosition = endPositionHandle;
                startControl = startControlHandle;
                endControl = endControlHandle;
                ApplyPathOptions();
                ConnectPaths(path, startIndex);
                //serialPath.ApplyModifiedProperties();
                //pathBezier = temp;
                //var path = (data.IsInsert) ? pathBezier : pathBezier.GetModifiedCurveCopy(pathBezier.EndControl, (x, y) => x + y);
                //data.TempPath = path.GeneratePath();
            }
        }
    }
}