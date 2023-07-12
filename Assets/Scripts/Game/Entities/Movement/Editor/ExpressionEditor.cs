using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class ExpressionEditor : PathEditor
    {
        WaypointPathExpression pathExpression;
        SerializedObject serialPath;
        SerializedProperty startPosition, pathFormula, length, angle;
        const string assetPath = "Assets/Editor Assets/ExpressionEditor.asset";

        private void OnEnable()
        {
            pathExpression = (WaypointPathExpression)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathExpression))
                ?? (WaypointPathExpression)ScriptableObject.CreateInstance(typeof(WaypointPathExpression));
            serialPath = new SerializedObject(pathExpression);

            startPosition = serialPath.FindProperty("StartPosition");
            pathFormula = serialPath.FindProperty("PathFormula");
            length = serialPath.FindProperty("Length");
            angle = serialPath.FindProperty("Angle");
        }

        private void OnDisable()
        {
            if (!AssetDatabase.Contains(pathExpression)) AssetDatabase.CreateAsset(pathExpression, assetPath);
            AssetDatabase.SaveAssets();
        }

        public override void PathOptions()
        {
            serialPath.Update();

            EditorGUILayout.PropertyField(pathFormula);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(angle);

            serialPath.ApplyModifiedProperties();
        }

        public override List<Vector2> MakePath(bool isReplace, float stepSize)
        {
            if (!isReplace)
            {
                var value = (WaypointPathExpression)pathExpression.GetNewAdjoinedPath(1);
                return value.GeneratePath(stepSize);
            }
            return pathExpression.GeneratePath(stepSize);
        }

        public new void DrawPath(in List<Vector2> pathData, Event e, in WaypointPathEditorData data)
        {
            base.DrawPath(in pathData, e, in data);

            var start = pathExpression.StartPosition;
            if (!data.IsReplace) pathExpression.StartPosition = pathExpression.GetPointVector(pathExpression.Length);

            data.TempPath = pathExpression.GeneratePath(data.StepSize);
            pathExpression.StartPosition = start;
        }
    }
}