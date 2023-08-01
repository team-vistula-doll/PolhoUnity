using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public abstract class PathEditor : ScriptableObject
    {
        protected float stepSize;
        private int startDeleteIndex = 0, endDeleteIndex = 0;
        public static Transform objectTransform;

        public abstract WaypointPathCreator GetPathCreator();

        public void ConnectPaths(ref SerializedProperty path, int startIndex)
        {
            //for (int i = startIndex; i < path.arraySize; i++)
            //{
            //    Debug.Log(path.GetArrayElementAtIndex(i).FindPropertyRelative("StartPosition"));
            //}
            if (startIndex == 0)
            {
                SerializedObject serializedObject = new(path.GetArrayElementAtIndex(startIndex).objectReferenceValue);
                serializedObject.FindProperty("StartPosition").vector2Value = objectTransform.position;
                startIndex++;
            }
            for (; startIndex < path.arraySize; startIndex++)
            {
                WaypointPathCreator x = (WaypointPathCreator)path.GetArrayElementAtIndex(startIndex - 1).objectReferenceValue;
                SerializedObject serializedObject = new(path.GetArrayElementAtIndex(startIndex).objectReferenceValue);
                serializedObject.FindProperty("StartPosition").vector2Value = x.GetEndVector();
            }
        }

        public void ConnectPaths(ref List<WaypointPathCreator> path, int startIndex)
        {
            if (startIndex == 0)
            {
                path[startIndex].StartPosition = objectTransform.position;
                startIndex++;
            }
            for (; startIndex < path.Count; startIndex++)
                path[startIndex].StartPosition = path[startIndex - 1].GetEndVector();
        }

        public virtual bool SelectPath(ref int selectedPathIndex, ref PathType pathTypeSelection,
            ref WaypointPathData pathData)
        {
            EditorGUILayout.LabelField("Selected Path:");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            {
                GUIContent backIcon = (selectedPathIndex > 0) ? EditorGUIUtility.IconContent("back") :
                    EditorGUIUtility.IconContent("d_back");
                GUIContent forwardIcon = (selectedPathIndex < pathData.Path.Count) ? EditorGUIUtility.IconContent("forward") :
                    EditorGUIUtility.IconContent("d_forward");

                if (GUILayout.Button(backIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                { if (selectedPathIndex > 0) selectedPathIndex--; }

                selectedPathIndex = EditorGUILayout.IntField("", selectedPathIndex + 1) - 1;

                if (GUILayout.Button(forwardIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                { if (selectedPathIndex < pathData.Path.Count) selectedPathIndex++; }

                string outOfCount = "/" + pathData.Path.Count;
                EditorGUILayout.LabelField(outOfCount, GUILayout.MaxWidth(EditorStyles.label.CalcSize(new GUIContent(outOfCount)).x));
            }
            EditorGUILayout.EndHorizontal();

            if (pathData.Path.Count <= 0) return false;
            //Limit the range
            if (selectedPathIndex > pathData.Path.Count) selectedPathIndex = pathData.Path.Count - 1;
            if (0 > selectedPathIndex) selectedPathIndex = 0;

            var selectedPath = (pathData.Path.Count > selectedPathIndex) ? pathData.Path[selectedPathIndex]
                : null;

            if (!EditorGUI.EndChangeCheck() || selectedPath == null) return false;

            stepSize = selectedPath.StepSize;
            pathTypeSelection = (PathType)WaypointPathEditorData.Options.FindIndex(
                kvp => kvp.GetPathCreator().GetType() == selectedPath.GetType()
                ); //Get index of used PathEditor child by comparing types

            return true;
        }

        public int DeletePath(ref SerializedProperty path)
        {
            int result = 0;
            EditorGUILayout.LabelField("Delete Paths:");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("From ")).x;
                startDeleteIndex = EditorGUILayout.IntField("From", startDeleteIndex + 1) - 1;
                if (startDeleteIndex < 0) startDeleteIndex = 0;
                if (startDeleteIndex > endDeleteIndex) startDeleteIndex = endDeleteIndex;

                //EditorGUILayout.LabelField("-", GUILayout.MaxWidth(EditorStyles.label.CalcSize(
                //    new GUIContent("-")).x));

                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("To ")).x;
                endDeleteIndex = EditorGUILayout.IntField("To", endDeleteIndex + 1) - 1;
                if (endDeleteIndex >= path.arraySize) endDeleteIndex = path.arraySize - 1;
                if (endDeleteIndex < startDeleteIndex) endDeleteIndex = startDeleteIndex;

                EditorGUI.BeginDisabledGroup(path.arraySize == 0);
                {
                    if (GUILayout.Button("Delete"))
                    {
                        result = endDeleteIndex - startDeleteIndex + 1;
                        for (int i = startDeleteIndex; i <= endDeleteIndex; i++) 
                        {
                            path.GetArrayElementAtIndex(startDeleteIndex).objectReferenceValue = null;
                            path.DeleteArrayElementAtIndex(startDeleteIndex);
                        }
                        if (path.arraySize > 0) ConnectPaths(ref path, startDeleteIndex);
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUIUtility.labelWidth = 0;
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }

        public static void PathTypes(ref PathType pathTypeSelection)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Path type: ");

                pathTypeSelection = (PathType)GUILayout.Toolbar(
                    (int)pathTypeSelection, System.Enum.GetNames(typeof(PathType)), EditorStyles.radioButton);
            }
            EditorGUILayout.EndHorizontal();
        }

        public virtual void PathOptions()
        {
            stepSize = EditorGUILayout.Slider("Step size", stepSize, 0.2f, 50);
        }

        public void SetPath(ref SerializedProperty path, ref bool isInsert, ref int selectedPathIndex)
        {
            EditorGUILayout.BeginHorizontal();
            {
                isInsert = EditorGUILayout.ToggleLeft("Insert (before)", isInsert);

                if (GUILayout.Button("Set path"))
                {

                    //Setting the new path in the edited object through serializedObject
                    if (path.arraySize <= selectedPathIndex) path.arraySize++;
                    else if (isInsert)
                    {
                        path.InsertArrayElementAtIndex(selectedPathIndex);
                    }
                    path.GetArrayElementAtIndex(selectedPathIndex).objectReferenceValue = Instantiate(GetPathCreator());
                    //Debug.Log(path.GetArrayElementAtIndex(selectedPathIndex).objectReferenceValue);

                    //If isInsert true, then start from inserted element
                    ConnectPaths(ref path, selectedPathIndex + (isInsert ? 0 : 1));
                    selectedPathIndex++;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        //public abstract List<Vector2> MakePath(bool isAddedAtEnd = false);

        public static List<Vector2> CreateVectorPath(in List<WaypointPathCreator> path, int startIndex)
        {
            List<Vector2> vector2s = new();
            for (;  startIndex < path.Count; startIndex++)
                vector2s.AddRange(path[startIndex].GeneratePath());
            return vector2s;
        }


        public virtual void DrawPath(ref List<WaypointPathCreator> path, int startIndex, EventType e, bool isTemp = false)
        {
            //List<Vector2> path = GetPathCreator().GeneratePath();
            if (path.Count == 0) return;
            Handles.color = isTemp ? Color.red : Color.green;

            //if (data.TempPath != null)
            //{
            //    foreach (Vector2 point in data.TempPath)
            //    {
            //        // Draws a blue line from this transform to the target
            //        Handles.color = Color.red;
            //        Handles.SphereHandleCap(0, point, Quaternion.identity, 0.08f, EventType.Repaint);
            //    }
            //}
            for (; startIndex < path.Count; startIndex++)
            {
                WaypointPathCreator x = (WaypointPathCreator)path[startIndex];
                List<Vector2> vector2s = x.GeneratePath();
                foreach (Vector2 point in vector2s)
                {
                    Handles.SphereHandleCap(0, point, Quaternion.identity, isTemp ? 0.08f : 0.1f, EventType.Repaint);
                }
            }
        }
    }
}