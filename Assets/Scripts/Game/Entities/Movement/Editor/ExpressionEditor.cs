using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class ExpressionEditor : PathEditor
    {
        WaypointPathExpression pathExpression;
        SerializedObject serialPath;
        SerializedProperty pathFormula, length, angle;
        const string assetPath = "Assets/Editor Assets/ExpressionEditor.asset";

        private void OnEnable()
        {
            pathExpression = (WaypointPathExpression)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathExpression));
            if (pathExpression == null) pathExpression = (WaypointPathExpression)ScriptableObject.
                    CreateInstance(typeof(WaypointPathExpression));
            serialPath = new SerializedObject(pathExpression);

            stepSize = serialPath.FindProperty("StepSize");
            pathFormula = serialPath.FindProperty("PathFormula");
            length = serialPath.FindProperty("Length");
            angle = serialPath.FindProperty("Angle");
        }

        private void OnDisable()
        {
            if (!AssetDatabase.Contains(pathExpression)) AssetDatabase.CreateAsset(pathExpression, assetPath);
            AssetDatabase.SaveAssets();
        }

        public void SelectPath(ref SerializedProperty selectedPathIndex, ref SerializedProperty pathTypeSelection,
            ref WaypointPathData pathData)
        {
            base.SelectPath(ref selectedPathIndex, ref pathData);

            var selectedPath = pathData.Path[selectedPathIndex.intValue];

            pathTypeSelection.intValue = WaypointPathEditorData.Options.FindIndex(
                kvp => kvp.Value.GetType() == selectedPath.GetType()
                ); //Get index of used PathEditor child by comparing types
            stepSize.floatValue = selectedPath.StepSize;
        }

        public override void PathOptions()
        {
            serialPath.Update();

            EditorGUILayout.PropertyField(pathFormula);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(angle);

            stepSize.floatValue = EditorGUILayout.Slider("Step size", stepSize.floatValue, 0.2f, 50);

            serialPath.ApplyModifiedProperties();
        }

        public override List<Vector2> MakePath(bool isReplace, float stepSize)
        {
            if (!isReplace)
            {
                var value = (WaypointPathExpression)pathExpression.GetNewAdjoinedPath(1);
                return value.GeneratePath();
            }
            return pathExpression.GeneratePath();
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