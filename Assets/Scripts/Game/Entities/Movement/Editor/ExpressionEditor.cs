using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class ExpressionEditor : PathEditor
    {
        [SerializeField]
        WaypointPathExpression pathExpression = new(new(Vector2.zero, 0, 0), "x", 20, 0);
        [SerializeField, Delayed]
        string pathFormula;
        [SerializeField, Min(0f)]
        float length;
        [SerializeField]
        float angle;

        protected override void OnEnable()
        {
            base.OnEnable();

            stepSize = pathExpression.StepSize;
            pathFormula = pathExpression.PathFormula;
            length = pathExpression.Length;
            angle = pathExpression.Angle;
        }

        public override WaypointPathCreator GetPathCreator() => pathExpression;
        public override void SetPathCreator(WaypointPathCreator pathCreator)
        {
            pathExpression = (WaypointPathExpression)pathCreator;
            base.SetPathCreator(pathExpression);
            pathFormula = pathExpression.PathFormula.ToString();
            length = pathExpression.Length;
            angle = pathExpression.Angle;
        }

        public override bool PathOptions()
        {
            bool changed = false;
            Undo.RecordObject(this, "Edit path options");

            EditorGUI.BeginChangeCheck();
            pathFormula = EditorGUILayout.TextField("Path Formula", pathFormula);
            length = EditorGUILayout.FloatField("Length (x)", length); if (length < 0f) length = 0f;
            angle = EditorGUILayout.FloatField("Angle", angle);
            base.PathOptions();

            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
                ApplyPathOptions();
            }

            return changed;
        }

        public override void ApplyPathOptions()
        {
            base.ApplyPathOptions();
            pathExpression.StepSize = stepSize;
            pathExpression.PathFormula = pathFormula.ToString();
            pathExpression.Length = length;
            pathExpression.Angle = angle;
        }

        //protected override void SetPathProperties(SerializedProperty sp)
        //{
        //    sp.FindPropertyRelative(nameof(WaypointPathExpression.StartPosition)).vector2Value = startPosition;
        //    sp.FindPropertyRelative(nameof(WaypointPathExpression.PathFormula)).stringValue = pathFormula;
        //    sp.FindPropertyRelative(nameof(WaypointPathExpression.Length)).floatValue = length;
        //    sp.FindPropertyRelative(nameof(WaypointPathExpression.Angle)).floatValue = angle;
        //}

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