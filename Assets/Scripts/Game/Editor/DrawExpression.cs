using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaypointPath;

public class DrawExpression : DrawPath
{
    WaypointPathExpression creator = new();
    public void Draw(ExpressionProperties properties, WaypointPathData pathData, Event e, ref List<Vector2> tempPath, bool isReplace)
    {
        base.Draw(properties, pathData, e, ref tempPath, isReplace);

        if(!isReplace) properties.StartPosition = WaypointPathExpression.GetPointVector(properties, properties.Length);

        tempPath = creator.GeneratePath(properties);
    }
}
