using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public abstract class PathEditor : ScriptableObject
    {
        protected SerializedProperty stepSize;

        public virtual void PathOptions()
        {
            stepSize.floatValue = EditorGUILayout.Slider("Step size", stepSize.floatValue, 0.2f, 50);
        }

        public abstract List<Vector2> MakePath(bool isReplace);

        public virtual bool SelectPath(ref SerializedProperty selectedPathIndex, ref SerializedProperty pathTypeSelection,
            ref WaypointPathData pathData)
        {
            EditorGUILayout.LabelField("Selected Path:");
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            {
                GUIContent backIcon = (selectedPathIndex.intValue > 0) ? EditorGUIUtility.IconContent("back") :
                    EditorGUIUtility.IconContent("d_back");
                GUIContent forwardIcon = (selectedPathIndex.intValue < pathData.Path.Count) ? EditorGUIUtility.IconContent("forward") :
                    EditorGUIUtility.IconContent("d_forward");

                if (GUILayout.Button(backIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                { if (selectedPathIndex.intValue > 0) selectedPathIndex.intValue--; }

                selectedPathIndex.intValue = EditorGUILayout.IntField("", selectedPathIndex.intValue + 1) - 1;

                if (GUILayout.Button(forwardIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                { if (selectedPathIndex.intValue < pathData.Path.Count) selectedPathIndex.intValue++; }

                string outOfCount = "/" + pathData.Path.Count;
                EditorGUILayout.LabelField(outOfCount, GUILayout.MaxWidth(EditorStyles.label.CalcSize(new GUIContent(outOfCount)).x));

                if (pathData.Path.Count <= 0) return false;
                //Limit the range
                if (selectedPathIndex.intValue > pathData.Path.Count) selectedPathIndex.intValue = pathData.Path.Count - 1;
                if (0 > selectedPathIndex.intValue) selectedPathIndex.intValue = 0;
            }
            EditorGUILayout.EndHorizontal();

            var selectedPath = pathData.Path[selectedPathIndex.intValue];

            if (!EditorGUI.EndChangeCheck() || selectedPath == null) return false;

            stepSize.floatValue = selectedPath.StepSize;
            pathTypeSelection.intValue = WaypointPathEditorData.Options.FindIndex(
                kvp => kvp.GetType() == selectedPath.GetType()
                ); //Get index of used PathEditor child by comparing types

            return true;
        }

        public void DrawPath(in List<Vector2> pathData, Event e, in WaypointPathEditorData data)
        {
            if (e.type == EventType.Repaint)
            {
                if (data.TempPath != null)
                {
                    foreach (Vector2 point in data.TempPath)
                    {
                        // Draws a blue line from this transform to the target
                        Handles.color = Color.red;
                        Handles.SphereHandleCap(0, point, Quaternion.identity, 0.08f, EventType.Repaint);
                    }
                }
                if (pathData != null)
                {
                    foreach (Vector2 point in pathData)
                    {
                        // Draws a blue line from this transform to the target
                        Handles.color = Color.green;
                        Handles.SphereHandleCap(0, point, Quaternion.identity, 0.1f, EventType.Repaint);
                    }
                }
            }
        }
    }
}