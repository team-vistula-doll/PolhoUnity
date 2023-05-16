using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WaypointPath;


public struct DrawPathHandles
{
    bool isMousePressed;
    bool isEndControlEnabled;
    //bool isStartControlEnabled = false;
    public DrawPathHandles(bool isMousePressed, bool isEndControlEnabled)
    {
        this.isMousePressed = isMousePressed;
        this.isEndControlEnabled = isEndControlEnabled;
    }

    public void Draw(BezierProperties properties, WaypointPathData pathData, Event e, ref List<Vector2> tempPath, bool isReplace)
    {
        Vector2 snap = Vector2.one * 0.2f;

        EditorGUI.BeginChangeCheck();
            float size;
            Vector2 startControlHandle = properties.StartPosition;
            Vector2 endControlHandle = properties.StartPosition;

            if (e.type == EventType.Repaint)
            {
                if (tempPath != null)
                {
                    foreach (Vector2 point in tempPath)
                    {
                        // Draws a blue line from this transform to the target
                        Handles.color = Color.red;
                        Handles.SphereHandleCap(0, point, Quaternion.identity, 0.08f, EventType.Repaint);
                    }
                }
                if (pathData.Path != null)
                {
                    foreach (Vector2 point in pathData.Path)
                    {
                        // Draws a blue line from this transform to the target
                        Handles.color = Color.green;
                        Handles.SphereHandleCap(0, point, Quaternion.identity, 0.1f, EventType.Repaint);
                    }
                }
            }

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
            if (e.type == EventType.MouseUp && properties.EndPosition != Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = true;
                isMousePressed = false;
            }
            else if (e.type == EventType.MouseUp && properties.EndPosition == Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = false;
                isMousePressed = false;
            }
            //***

            //if (isStartControlEnabled)
            //{
            size = HandleUtility.GetHandleSize(properties.StartControl) * 0.15f;
            Handles.color = Color.cyan;
            startControlHandle = Handles.FreeMoveHandle(properties.StartControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            //}
            //else startControlHandle = Vector2.zero;

            if (isEndControlEnabled)
            {
                size = HandleUtility.GetHandleSize(properties.EndControl) * 0.15f;
                Handles.color = Color.cyan;
                endControlHandle = Handles.FreeMoveHandle(properties.EndControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            }
            else endControlHandle = Vector2.zero;

            size = HandleUtility.GetHandleSize(properties.EndPosition) * 0.2f;
            Handles.color = Color.red;
            Vector2 endPositionHandle = Handles.FreeMoveHandle(properties.EndPosition, Quaternion.identity, size, snap, Handles.SphereHandleCap);

            Handles.color = new Color(1, 0, 0, 0.5f);
            if (startControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(startControlHandle, properties.StartPosition, 2);
                Handles.color = new Color(1, 0.92f, 0.016f, 0.5f);
                Handles.DrawLine(endControlHandle, startControlHandle);
            }
            else if (endControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(endControlHandle, properties.StartPosition, 2);
            }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(pathData, "Change Handle Position");
            properties.EndPosition = endPositionHandle;
            properties.StartControl = startControlHandle;
            properties.EndControl = endControlHandle;
            tempPath = pathData.CreateWaypointPath((isReplace) ? properties.StartPosition : properties.Path.Last(), pathTypeSelection);
        }
    }
}
