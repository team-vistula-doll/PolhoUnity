using System.Collections.Generic;
using UnityEngine;
using WaypointPath;

public class DrawBezier : DrawPath
{
    public DrawBezier(bool isMousePressed, bool isEndControlEnabled) : base(isMousePressed, isEndControlEnabled)
    { }

    public void Draw(BezierProperties properties, WaypointPathData pathData, Event e, ref List<Vector2> tempPath, bool isReplace)
    {
        base.Draw(properties, pathData, e, ref tempPath, isReplace);

    }
}
