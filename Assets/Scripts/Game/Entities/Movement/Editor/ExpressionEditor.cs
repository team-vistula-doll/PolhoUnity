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

        public override WaypointPathCreator GetPathCreator() => pathExpression;

        public override bool SelectPath(ref SerializedProperty selectedPathIndex, ref SerializedProperty pathTypeSelection,
            ref WaypointPathData pathData)
        {
            bool pathExists = base.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref pathData);
            if (!pathExists) return false;

            WaypointPathExpression selectedPath = (WaypointPathExpression)pathData.Path[selectedPathIndex.intValue];
            //stepSize.floatValue = selectedPath.StepSize;
            //pathTypeSelection.intValue = WaypointPathEditorData.Options.FindIndex(
            //    kvp => kvp.GetType() == selectedPath.GetType()
            //    ); //Get index of used PathEditor child by comparing types
            pathFormula.stringValue = selectedPath.PathFormula;
            length.floatValue = selectedPath.Length;
            angle.floatValue = selectedPath.Angle;
            return true;
        }

        public override void PathOptions()
        {
            serialPath.Update();

            EditorGUILayout.PropertyField(pathFormula);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(angle);

            base.PathOptions();

            serialPath.ApplyModifiedProperties();
        }

        public override List<Vector2> MakePath(bool isAddedAtEnd = false)
        {
            if (isAddedAtEnd)
            {
                var value = (WaypointPathExpression)pathExpression.GetNewAdjoinedPath(1);
                return value.GeneratePath();
            }
            return pathExpression.GeneratePath();
        }

        //public override void DrawPath(in List<Vector2> pathData, bool isTemp, in WaypointPathEditorData data)
        //{
        //    base.DrawPath(in pathData, isTemp);

        //    var start = pathExpression.StartPosition;
        //    if (!data.IsInsert) pathExpression.StartPosition = pathExpression.GetPointVector(pathExpression.Length);

        //    data.TempPath = pathExpression.GeneratePath();
        //    pathExpression.StartPosition = start;
        //}
    }
}