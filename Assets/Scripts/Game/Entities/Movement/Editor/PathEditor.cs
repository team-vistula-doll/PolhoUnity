using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public abstract class PathEditor
    {
        public abstract void PathOptions();

        public abstract List<Vector2> MakePath(bool isReplace, float stepSize);

        public void DrawPath(ref WaypointPathData pathData, Event e, ref WaypointPathData tempPath, bool isReplace)
        {
            if (e.type == EventType.Repaint)
            {
                if (tempPath.Path != null)
                {
                    foreach (Vector2 point in tempPath.Path)
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
        }
}