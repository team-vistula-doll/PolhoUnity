using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaypointPath
{
    public enum PathType
    {
        Function,
        Bezier
    }

    public class WaypointPathEditorData : ScriptableObject
    {
        public static List<PathEditor> Options { get; private set; } = null;

        //public WaypointPathBezier PathBezier = new();
        //public WaypointPathExpression PathExpression = new();
        //public WaypointPathCreator Creator; //base path creator class

        //public DrawBezier DrawBezier = new();
        //public DrawExpression DrawExpression = new();
        //public DrawPath DrawPath; //base Editor drawer class

        [Min(1)]
        public int SelectedPathIndex = 0;
        [System.NonSerialized]
        public List<Vector2> TempPath = new();
        public bool IsInsert = false;
        public PathType PathTypeSelection = 0;

        public void OnEnable()
        {
            Options ??= new() {
            (ExpressionEditor)ScriptableObject.CreateInstance(typeof(ExpressionEditor)),
            (BezierEditor)ScriptableObject.CreateInstance(typeof(BezierEditor))
            };
        }
    }
}