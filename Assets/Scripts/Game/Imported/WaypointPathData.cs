using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathData
    {
        public int PathTypeSelection = 0;

        [Delayed]
        public string PathFormula = "x";
        public float Length = 20, Angle;

        public Vector2 StartPosition, StartControl, EndControl, EndPosition = Vector2.zero;

        [Range(0.2f, 50f)]
        public float StepSize = 0.5f;

        public List<Vector2> Path = new() { Vector2.zero };
        private List<Vector2> _tempPath = new() { Vector2.zero };

        /// <summary>
        /// Validates and sets the path
        /// </summary>
        /// <param name="isAdd">If true the path is added to the existing path, if false the path replaces the existing path</param>
        /// <param name="isTemp">If true sets the temp path, if false sets the real path</param>
        public void ValidatePath(bool isAdd = false, bool isTemp = true)
        {
            if (isTemp)
                SetTempPath(isAdd: isAdd);
            else
                SetWaypointPath(isAdd: isAdd);
        }

        /// <summary>
        /// Creates path from argument or from temp path if null
        /// </summary>
        public void SetWaypointPath(List<Vector2> pathToSet = null, bool isAdd = false)
        {
            if (isAdd)
                Path.AddRange(pathToSet ?? _tempPath);
            else
                Path = pathToSet ?? _tempPath;
            if (pathToSet != null) _tempPath = new() { Path.Last() };
        }

        public void SetTempPath(List<Vector2> pathToSet = null, bool isAdd = false)
        {
            if (pathToSet != null)
                _tempPath = pathToSet;
            else
            {
                Vector2 startPos = (isAdd && Path.Count() != 0) ? Path.Last() : StartPosition;
                switch (PathTypeSelection)
                {
                    case 1:
                        _tempPath = WaypointPathCreator.GeneratePathFromCurve(startPos, EndPosition, StartControl, EndControl, StepSize);
                        break;
                    default:
                        _tempPath = WaypointPathCreator.GeneratePathFromExpression(startPos, Length, PathFormula, Angle, StepSize);
                        break;
                }
            }
        }
    }
}