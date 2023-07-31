using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public enum PathType
    {
        Function,
        Bezier
    }

    [FilePath("Editor Assets/WaypointPathEditorData.so", FilePathAttribute.Location.ProjectFolder)]
    public class WaypointPathEditorData : ScriptableSingleton<WaypointPathEditorData>
    {
        public static List<PathEditor> Options { get; private set; } = new()
        {
            new ExpressionEditor(),
            new BezierEditor()
        };

        [Min(1)]
        public int SelectedPathIndex = 0;
        //[System.NonSerialized]
        //public List<Vector2> TempPath = new();
        public bool IsInsert = false;
        public PathType PathTypeSelection = 0;

        public PathEditor SelectedOption { get { return Options[(int)PathTypeSelection]; } }

        //public void OnEnable()
        //{
        //    Options ??= new() {
        //    (ExpressionEditor)ScriptableObject.CreateInstance(typeof(ExpressionEditor)),
        //    (BezierEditor)ScriptableObject.CreateInstance(typeof(BezierEditor))
        //    };
        //}
    }
}