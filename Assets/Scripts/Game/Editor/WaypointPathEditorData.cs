using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathEditorData : ScriptableObject
    {
        public Dictionary<string, PathEditor> Options;

        //public WaypointPathBezier PathBezier = new();
        //public WaypointPathExpression PathExpression = new();
        //public WaypointPathCreator Creator; //base path creator class

        //public DrawBezier DrawBezier = new();
        //public DrawExpression DrawExpression = new();
        //public DrawPath DrawPath; //base Editor drawer class

        [Range(0.2f, 50f)]
        public float StepSize = 0.5f;
        public List<Vector2> TempPath = new();
        public bool IsReplace = false;
        public int PathTypeSelection = 0;

        public void OnEnable()
        {
            Options = new() {
            { "Function", (ExpressionEditor)ScriptableObject.CreateInstance(typeof(ExpressionEditor)) },
            { "Bezier", (BezierEditor)ScriptableObject.CreateInstance(typeof(BezierEditor)) }
        };
        }
    }
}