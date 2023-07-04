using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathEditorData
    {
        public WaypointPathBezier PathBezier = new();
        public WaypointPathExpression PathExpression = new();
        public WaypointPathCreator Creator; //base path creator class

        public DrawBezier DrawBezier = new(false, false);
        public DrawExpression DrawExpression = new();
        public DrawPath DrawPath; //base Editor drawer class

        [Range(0.2f, 50f)]
        public float StepSize = 0.5f;
        public List<Vector2> TempPath = new();
        public bool IsReplace = false;
        public int PathTypeSelection = 0;
    }
}