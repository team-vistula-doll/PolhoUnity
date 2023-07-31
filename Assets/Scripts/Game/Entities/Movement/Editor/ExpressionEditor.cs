using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class ExpressionEditor : PathEditor
    {
        WaypointPathExpression pathExpression = new();
        //SerializedObject serialPath;
        //SerializedProperty pathFormula, length, angle;
        //const string assetPath = "Assets/Editor Assets/ExpressionEditor.asset";

        //private void Awake()
        //{
        //    if (pathExpression == null) pathExpression = new();
        //    //serialPath = new SerializedObject(pathExpression);

        //    //stepSize = serialPath.FindProperty("StepSize");
        //    //pathFormula = serialPath.FindProperty("PathFormula");
        //    //length = serialPath.FindProperty("Length");
        //    //angle = serialPath.FindProperty("Angle");
        //}

        //private void OnEnable()
        //{
        //    pathExpression = (WaypointPathExpression)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathExpression));
        //    if (pathExpression == null)
        //        pathExpression = (WaypointPathExpression)ScriptableObject.CreateInstance(typeof(WaypointPathExpression));
        //    serialPath = new SerializedObject(pathExpression);

        //    stepSize = serialPath.FindProperty("StepSize");
        //    pathFormula = serialPath.FindProperty("PathFormula");
        //    length = serialPath.FindProperty("Length");
        //    angle = serialPath.FindProperty("Angle");
        //}

        //private void OnDisable()
        //{
        //    if (!AssetDatabase.Contains(pathExpression)) AssetDatabase.CreateAsset(pathExpression, assetPath);
        //    AssetDatabase.SaveAssets();
        //}

        public override WaypointPathCreator GetPathCreator() => pathExpression;

        public override bool SelectPath(ref int selectedPathIndex, ref PathType pathTypeSelection,
            ref WaypointPathData pathData)
        {
            bool pathExists = base.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref pathData);
            if (!pathExists) return false;

            //serialPath.Update();
            WaypointPathExpression selectedPath = (WaypointPathExpression)pathData.Path[selectedPathIndex];
            pathExpression.PathFormula = selectedPath.PathFormula.ToString();
            pathExpression.Length = selectedPath.Length;
            pathExpression.Angle = selectedPath.Angle;
            //serialPath.ApplyModifiedProperties();
            return true;
        }

        public override void PathOptions()
        {
            //serialPath.Update();

            pathExpression.PathFormula = EditorGUILayout.TextField("Path Formula", pathExpression.PathFormula);
            pathExpression.Length = EditorGUILayout.FloatField("Length", pathExpression.Length);
            pathExpression.Angle = EditorGUILayout.FloatField("Angle", pathExpression.Angle);

            base.PathOptions();

            //serialPath.ApplyModifiedProperties();
        }

        //public override List<Vector2> MakePath(bool isAddedAtEnd = false)
        //{
        //    if (isAddedAtEnd)
        //    {
        //        var value = (WaypointPathExpression)pathExpression.GetNewAdjoinedPath(1);
        //        return value.GeneratePath();
        //    }
        //    return pathExpression.GeneratePath();
        //}
    }
}