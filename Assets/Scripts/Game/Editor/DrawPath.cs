using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WaypointPath;

public class DrawPath
{
    public void Draw(PathProperties properties, WaypointPathData pathData, Event e, ref List<Vector2> tempPath, bool isReplace)
    {
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

    }
}
