using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathEditorData : ScriptableObject
    {
        public static List<KeyValuePair<string, PathEditor>> Options { get; private set; } = null;

        //public WaypointPathBezier PathBezier = new();
        //public WaypointPathExpression PathExpression = new();
        //public WaypointPathCreator Creator; //base path creator class

        //public DrawBezier DrawBezier = new();
        //public DrawExpression DrawExpression = new();
        //public DrawPath DrawPath; //base Editor drawer class

        [Min(1)]
        public int SelectedPathIndex = 1;
        //[Range(0.2f, 50f)]
        public List<Vector2> TempPath = new();
        public bool IsReplace = false;
        public int PathTypeSelection = 0;

        public void OnEnable()
        {
            Options ??= new() {
            new KeyValuePair<string, PathEditor>("Function", (ExpressionEditor)ScriptableObject.CreateInstance(typeof(ExpressionEditor))),
            new KeyValuePair<string, PathEditor>("Bezier",(BezierEditor) ScriptableObject.CreateInstance(typeof(BezierEditor)))
        };
        }
    }
}