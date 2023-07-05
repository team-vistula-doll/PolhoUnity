using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    public abstract class PathEditor
    {
        public abstract void PathOptions();

        public abstract List<Vector2> MakePath(bool isReplace, float stepSize);

        public abstract void DrawPath(ref WaypointPathData pathData, Event e, ref WaypointPathData tempPath, bool isReplace);
    }
}