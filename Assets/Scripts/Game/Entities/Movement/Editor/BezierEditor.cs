using System.Collections.Generic;
using System.Xml.XPath;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class BezierEditor : PathEditor
    {
        WaypointPathBezier pathBezier;

        public void OnEnable()
        {
            pathBezier = (WaypointPathBezier)ScriptableObject.CreateInstance(typeof(WaypointPathBezier));
        }

        bool isMousePressed = false;
        bool isEndControlEnabled = false;
        //bool isStartControlEnabled = false;
        public override void PathOptions()
        {
            pathBezier.Properties.EndPosition = EditorGUILayout.Vector2Field("End", pathBezier.Properties.EndPosition);
            EditorGUI.BeginDisabledGroup(pathBezier.Properties.EndPosition == Vector2.zero);
                pathBezier.Properties.EndControl = EditorGUILayout.Vector2Field("1st control", pathBezier.Properties.EndControl);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(pathBezier.Properties.EndControl == Vector2.zero);
                pathBezier.Properties.StartControl = EditorGUILayout.Vector2Field("2nd control", pathBezier.Properties.StartControl);
            EditorGUI.EndDisabledGroup();
        }

        public override List<Vector2> MakePath(bool isReplace, float stepSize)
        {
            if (isReplace) pathBezier.Properties = (BezierProperties)pathBezier.Properties.GetNewAdjoinedPath(1);
            return pathBezier.GeneratePath(stepSize);
        }

        new public void DrawPath(ref List<Vector2> pathData, Event e, ref List<Vector2> tempPath, bool isReplace)
        {
            EditorGUI.BeginChangeCheck();

            base.DrawPath(ref pathData, e, ref tempPath, isReplace);

            Vector2 snap = Vector2.one * 0.2f;
            float size;
            Vector2 startControlHandle = pathBezier.Properties.StartPosition;
            Vector2 endControlHandle = pathBezier.Properties.StartPosition;

            if (e.type == EventType.MouseDown) isMousePressed = true;

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
            if (e.type == EventType.MouseUp && pathBezier.Properties.EndPosition != Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = true;
                isMousePressed = false;
            }
            else if (e.type == EventType.MouseUp && pathBezier.Properties.EndPosition == Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = false;
                isMousePressed = false;
            }
            //***

            //if (isStartControlEnabled)
            //{
            size = HandleUtility.GetHandleSize(pathBezier.Properties.StartControl) * 0.15f;
            Handles.color = Color.cyan;
            startControlHandle = Handles.FreeMoveHandle(pathBezier.Properties.StartControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            //}
            //else startControlHandle = Vector2.zero;

            if (isEndControlEnabled)
            {
                size = HandleUtility.GetHandleSize(pathBezier.Properties.EndControl) * 0.15f;
                Handles.color = Color.cyan;
                endControlHandle = Handles.FreeMoveHandle(pathBezier.Properties.EndControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            }
            else endControlHandle = Vector2.zero;

            size = HandleUtility.GetHandleSize(pathBezier.Properties.EndPosition) * 0.2f;
            Handles.color = Color.red;
            Vector2 endPositionHandle = Handles.FreeMoveHandle(pathBezier.Properties.EndPosition, Quaternion.identity, size, snap, Handles.SphereHandleCap);

            Handles.color = new Color(1, 0, 0, 0.5f);
            if (startControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(startControlHandle, pathBezier.Properties.StartPosition, 2);
                Handles.color = new Color(1, 0.92f, 0.016f, 0.5f);
                Handles.DrawLine(endControlHandle, startControlHandle);
            }
            else if (endControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(endControlHandle, pathBezier.Properties.StartPosition, 2);
            }

            if (EditorGUI.EndChangeCheck())
            {
                pathBezier.Properties.EndPosition = endPositionHandle;
                pathBezier.Properties.StartControl = startControlHandle;
                pathBezier.Properties.EndControl = endControlHandle;
                Undo.RecordObject(pathBezier, "Change Handle Position");
                tempPath = pathBezier.GeneratePath((isReplace)
                    ? pathBezier.Properties
                    : pathBezier.Properties.GetModifiedCurveCopy(pathBezier.Properties.EndControl, (x, y) => x + y)
                );
            }
        }
    }
}