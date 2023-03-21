using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathData : MonoBehaviour
    {
        [Delayed]
        public string PathFormula = "x";
        public float Length = 20, Angle;

        public Vector2 StartPosition = Vector2.zero, StartControl, EndControl, EndPosition = Vector2.zero;

        [Range(0.2f, 50f)]
        public float StepSize = 0.5f;

        public List<Vector2> Path = new() { Vector2.zero };

        /// <summary>
        /// Creates path from argument or fields if null
        /// </summary>
        /// <param name="pathTypeSelection">0 for expression, 1 for Bezier curve</param>
        public List<Vector2> CreateWaypointPath(Vector2 startPos, int pathTypeSelection = 0)
        {
            return pathTypeSelection switch
            {
                1 => WaypointPathCreator.GeneratePathFromCurve(startPos, EndPosition, StartControl, EndControl, StepSize),
                _ => WaypointPathCreator.GeneratePathFromExpression(startPos, Length, PathFormula, Angle, StepSize),
            };
        }

        public void SetWaypointPath(int pathTypeSelection = 0, bool isAdd = false, List<Vector2> pathToSet = null)
        {
            if (isAdd) Path.AddRange(pathToSet ?? CreateWaypointPath(
                (isAdd && Path.Count() != 0) ? Path.Last() : StartPosition,
                pathTypeSelection));
            else Path = pathToSet ?? CreateWaypointPath(
                (isAdd && Path.Count() != 0) ? Path.Last() : StartPosition,
                pathTypeSelection);
        }
    }
}