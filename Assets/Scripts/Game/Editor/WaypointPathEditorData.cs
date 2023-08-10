#if UNITY_EDITOR
using System.Collections.Generic;
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
        public static List<PathEditor> Options { get; private set; }
        public PathEditor SelectedOption { get => Options[(int)PathTypeSelection]; }

        [Min(0)]
        public int SelectedPathIndex = 0;
        //[System.NonSerialized]
        //public List<Vector2> TempPath = new();
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
#endif