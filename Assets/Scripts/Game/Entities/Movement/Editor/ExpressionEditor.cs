using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class ExpressionEditor : PathEditor
    {
        [SerializeField]
        WaypointPathExpression pathExpression = new(Vector2.zero, "x", 20, 0);
        //SerializedObject serialPath;
        [SerializeField]
        string pathFormula;
        [SerializeField]
        float length, angle;
        //const string assetPath = "Assets/Editor Assets/ExpressionEditor.asset";

        //private void Awake()
        //{
        //    pathExpression = (WaypointPathExpression)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathExpression));
        //    if (pathExpression == null) pathExpression = (WaypointPathExpression)ScriptableObject.
        //            CreateInstance(typeof(WaypointPathExpression));
        //}

        private void OnEnable()
        {
            //pathExpression = (WaypointPathExpression)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathExpression));
            //if (pathExpression == null) pathExpression = (WaypointPathExpression)ScriptableObject.
            //        CreateInstance(typeof(WaypointPathExpression));
            //serialPath = new SerializedObject(pathExpression);
            Undo.undoRedoPerformed += ApplyPathOptions;

            stepSize = pathExpression.StepSize;
            pathFormula = pathExpression.PathFormula;
            length = pathExpression.Length;
            angle = pathExpression.Angle;
        }

        //private void OnDisable()
        //{
        //    if (!AssetDatabase.Contains(pathExpression)) AssetDatabase.CreateAsset(pathExpression, assetPath);
        //    AssetDatabase.SaveAssets();
        //}

        public override WaypointPathCreator GetPathCreator() => pathExpression;

        public override bool SelectPath(ref SerializedProperty selectedPathIndex, ref SerializedProperty pathTypeSelection,
            ref WaypointPathData pathData)
        {
            bool pathExists = base.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref pathData);
            if (!pathExists) return false;

            //serialPath.Update();
            WaypointPathExpression selectedPath = (WaypointPathExpression)pathData.Path[selectedPathIndex.intValue];
            startPosition = selectedPath.StartPosition;
            pathFormula = selectedPath.PathFormula.ToString();
            length = selectedPath.Length;
            angle = selectedPath.Angle;
            //serialPath.ApplyModifiedProperties();
            return true;
        }

        public override void PathOptions()
        {
            //serialPath.Update();
            Undo.RecordObject(this, "Edit path options");

            EditorGUI.BeginChangeCheck();
            pathFormula = EditorGUILayout.TextField("Path Formula", pathFormula);
            length = EditorGUILayout.FloatField("Length", length);
            angle = EditorGUILayout.FloatField("Angle", angle);

            if (EditorGUI.EndChangeCheck()) ApplyPathOptions();

            base.PathOptions();

            //serialPath.ApplyModifiedProperties();
        }

        protected override void ApplyPathOptions()
        {
            base.ApplyPathOptions();
            pathExpression.PathFormula = pathFormula;
            pathExpression.Length = length;
            pathExpression.Angle = angle;
        }

        protected override void SetPathProperties(ref SerializedProperty sp)
        {
            sp.FindPropertyRelative(nameof(WaypointPathExpression.StartPosition)).vector2Value = startPosition;
            sp.FindPropertyRelative(nameof(WaypointPathExpression.PathFormula)).stringValue = pathFormula;
            sp.FindPropertyRelative(nameof(WaypointPathExpression.Length)).floatValue = length;
            sp.FindPropertyRelative(nameof(WaypointPathExpression.Angle)).floatValue = angle;
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