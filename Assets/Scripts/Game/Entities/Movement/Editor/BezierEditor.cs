using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class BezierEditor : PathEditor
    {
        WaypointPathBezier pathBezier;
        SerializedObject serialPath;
        SerializedProperty startPosition, endPosition, startControl, endControl;
        const string assetPath = "Assets/Editor Assets/BezierEditor.asset";

        private void OnEnable()
        {
            pathBezier = (WaypointPathBezier)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathBezier))
                ?? (WaypointPathBezier)ScriptableObject.CreateInstance(typeof(WaypointPathBezier));
            serialPath = new SerializedObject(pathBezier);

            startPosition = serialPath.FindProperty("StartPosition");
            endPosition = serialPath.FindProperty("EndPosition");
            startControl = serialPath.FindProperty("StartControl");
            endControl = serialPath.FindProperty("EndControl");
        }

        private void OnDisable()
        {
            if (!AssetDatabase.Contains(pathBezier)) AssetDatabase.CreateAsset(pathBezier, assetPath);
            AssetDatabase.SaveAssets();
        }

        bool isMousePressed = false;
        bool isEndControlEnabled = false;
        //bool isStartControlEnabled = false;
        public override void PathOptions()
        {
            serialPath.Update();

            EditorGUILayout.PropertyField(endPosition);
            EditorGUI.BeginDisabledGroup(endPosition.vector2Value == Vector2.zero);
                EditorGUILayout.PropertyField(endControl);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(endControl.vector2Value == Vector2.zero);
                EditorGUILayout.PropertyField(startControl);
            EditorGUI.EndDisabledGroup();

            serialPath.ApplyModifiedProperties();
        }

        public override List<Vector2> MakePath(bool isReplace, float stepSize)
        {
            if (!isReplace)
            {
                var value = (WaypointPathBezier)pathBezier.GetNewAdjoinedPath(1);
                return value.GeneratePath(stepSize);
            }
            return pathBezier.GeneratePath(stepSize);
        }

        public new void DrawPath(in List<Vector2> pathData, Event e, in WaypointPathEditorData data)
        {
            EditorGUI.BeginChangeCheck();

            base.DrawPath(in pathData, e, in data);

            Vector2 snap = Vector2.one * 0.2f;
            float size;
            Vector2 startControlHandle = startPosition.vector2Value;
            Vector2 endControlHandle = startPosition.vector2Value;

            if (e.type == EventType.MouseDown) isMousePressed = true;

            //If the End Control handle is right under the End Position handle, the EC handle has priority in selecting it,
            //however this doesn't happen with the Start Control for some reason, both can overlap it and it won't get selected
            //The commented-out code for the SC handle also doesn't work, even though it's the same as for the EC handle

            ////***Prevent Start Control handle from getting selected when End Control is zero
            //if (e.type == EventType.MouseUp && pathData.EndControl != Vector2.zero && isMousePressed)
            //{
            //    isStartControlEnabled = true;
            //    isMousePressed = false;
            //}
            //else if (e.type == EventType.MouseUp && pathData.EndControl == Vector2.zero && isMousePressed)
            //{
            //    isStartControlEnabled = false;
            //    isMousePressed = false;
            //}
            ////***

            //***Prevent End Control handle from getting selected when End Position is zero
            if (e.type == EventType.MouseUp && endPosition.vector2Value != Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = true;
                isMousePressed = false;
            }
            else if (e.type == EventType.MouseUp && endPosition.vector2Value == Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = false;
                isMousePressed = false;
            }
            //***

            //if (isStartControlEnabled)
            //{
            size = HandleUtility.GetHandleSize(startControl.vector2Value) * 0.15f;
            Handles.color = Color.cyan;
            startControlHandle = Handles.FreeMoveHandle(startControl.vector2Value, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            //}
            //else startControlHandle = Vector2.zero;

            if (isEndControlEnabled)
            {
                size = HandleUtility.GetHandleSize(endControl.vector2Value) * 0.15f;
                Handles.color = Color.cyan;
                endControlHandle = Handles.FreeMoveHandle(endControl.vector2Value, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            }
            else endControlHandle = Vector2.zero;

            size = HandleUtility.GetHandleSize(endPosition.vector2Value) * 0.2f;
            Handles.color = Color.red;
            Vector2 endPositionHandle = Handles.FreeMoveHandle(endPosition.vector2Value, Quaternion.identity, size, snap, Handles.SphereHandleCap);

            Handles.color = new Color(1, 0, 0, 0.5f);
            if (startControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(startControlHandle, startPosition.vector2Value, 2);
                Handles.color = new Color(1, 0.92f, 0.016f, 0.5f);
                Handles.DrawLine(endControlHandle, startControlHandle);
            }
            else if (endControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(endControlHandle, startPosition.vector2Value, 2);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pathBezier, "Change Handle Position");
                endPosition.vector2Value = endPositionHandle;
                startControl.vector2Value = startControlHandle;
                endControl.vector2Value = endControlHandle;
                var path = (data.IsReplace) ? pathBezier : pathBezier.GetModifiedCurveCopy(pathBezier.EndControl, (x, y) => x + y);
                    data.TempPath = path.GeneratePath(data.StepSize);
            }
        }
    }
}