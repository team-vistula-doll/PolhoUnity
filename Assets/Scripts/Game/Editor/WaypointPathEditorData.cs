#if UNITY_EDITOR
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

    public class WaypointPathEditorData : ScriptableObject
    {
        const string expressionAssetPath = "Assets/Editor Default Resources/ExpressionEditor.asset";
        const string bezierAssetPath = "Assets/Editor Default Resources" +
            "/BezierEditor.asset";
        public int PrefabID = 0;
        public static List<PathEditor> Options { get; private set; } = new() { null, null };
        public PathEditor SelectedOption { get => Options[(int)PathTypeSelection]; }

        [Min(0)]
        public int SelectedPathIndex = 0;
        [SerializeReference]
        public List<WaypointPathCreator> TempPath = new();
        public bool IsInsert = false;
        public PathType PathTypeSelection = 0;

        public void Init()
        {
            Options[0] = (ExpressionEditor)EditorGUIUtility.Load(expressionAssetPath);
            if (Options[0] == null) Options[0] = (ExpressionEditor)ScriptableObject.CreateInstance(typeof(ExpressionEditor));

            Options[1] = (BezierEditor)EditorGUIUtility.Load(bezierAssetPath);
            if (Options[1] == null) Options[1] = (BezierEditor)ScriptableObject.CreateInstance(typeof(BezierEditor));
        }

        private void OnDisable()
        {
            if (Options[0] != null && !AssetDatabase.Contains(Options[0]))
                AssetDatabase.CreateAsset(Options[0], expressionAssetPath);
            if (Options[1] != null && !AssetDatabase.Contains(Options[1]))
                AssetDatabase.CreateAsset(Options[1], bezierAssetPath);
            AssetDatabase.SaveAssets();
        }

        public static PathType GetSelectedOption(WaypointPathCreator creator)
            => (PathType)Options.FindIndex(option => option.GetPathCreator().GetType() == creator.GetType());
    }
}
#endif //UNITY_EDITOR