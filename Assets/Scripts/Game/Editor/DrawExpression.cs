using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaypointPath;

public class DrawExpression : DrawPath
{
    WaypointPathExpression creator = new();
    public void Draw(ExpressionProperties properties, ref WaypointPathData pathData, Event e, ref WaypointPathData tempPath, bool isReplace)
    {
        base.Draw(properties, ref pathData, e, ref tempPath, isReplace);

        if(!isReplace) properties.StartPosition = WaypointPathExpression.GetPointVector(properties, properties.Length);

        tempPath.Path = creator.GeneratePath(properties);
    }
}
