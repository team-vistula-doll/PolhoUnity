using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public abstract class PathEditor : ScriptableObject
    {
        public abstract void PathOptions();

        public abstract List<Vector2> MakePath(bool isReplace, float stepSize);

        public void SelectPath(ref SerializedProperty selectedPathIndex, ref WaypointPathData pathData)
        {
            EditorGUILayout.LabelField("Selected Path:");
            EditorGUILayout.BeginHorizontal();
            {
                GUIContent backIcon = (selectedPathIndex.intValue > 1) ? EditorGUIUtility.IconContent("back") :
                    EditorGUIUtility.IconContent("d_back");
                GUIContent forwardIcon = (selectedPathIndex.intValue < pathData.Path.Count + 1) ? EditorGUIUtility.IconContent("forward") :
                    EditorGUIUtility.IconContent("d_forward");

                if (GUILayout.Button(backIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                { if (selectedPathIndex.intValue > 1) selectedPathIndex.intValue--; }

                EditorGUILayout.PropertyField(selectedPathIndex, GUIContent.none);

                if (GUILayout.Button(forwardIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                { if (selectedPathIndex.intValue < pathData.Path.Count + 1) selectedPathIndex.intValue++; }

                string outOfCount = "/" + pathData.Path.Count;
                EditorGUILayout.LabelField(outOfCount, GUILayout.MaxWidth(EditorStyles.label.CalcSize(new GUIContent(outOfCount)).x));

                //Limit the range
                if (1 > selectedPathIndex.intValue) selectedPathIndex.intValue = 1;
                if (selectedPathIndex.intValue > pathData.Path.Count + 1) selectedPathIndex.intValue = pathData.Path.Count;
            }
            EditorGUILayout.EndHorizontal();
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