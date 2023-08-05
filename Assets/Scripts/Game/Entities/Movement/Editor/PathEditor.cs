using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public abstract class PathEditor : ScriptableObject
    {
        protected float stepSize;
        public int startDeleteIndex = 0, endDeleteIndex = 0;
        protected Vector2 startPosition;
        public Transform objectTransform;

        public abstract WaypointPathCreator GetPathCreator();

        //public static void SavePrefab(ref WaypointPathData pathData)
        //{

        //    if (pathData != null && pathData.Path != null)
        //    {
        //        foreach (WaypointPathCreator wpc in pathData.Path)
        //            EditorUtility.SetDirty(wpc);
        //    }
        //}

        public void ConnectPaths(ref SerializedProperty path, in List<WaypointPathCreator> pathList, int startIndex)
        {
            //for (int i = startIndex; i < path.arraySize; i++)
            //{
            //    Debug.Log(path.GetArrayElementAtIndex(i).FindPropertyRelative("StartPosition"));
            //}
            if (startIndex == 0)
            {
                SerializedObject serializedObject = new(path.GetArrayElementAtIndex(startIndex).objectReferenceValue);
                serializedObject.FindProperty("StartPosition").vector2Value = objectTransform.position;
                serializedObject.ApplyModifiedProperties();
                startIndex++;
            }
            for (; startIndex < path.arraySize; startIndex++)
            {
                SerializedObject serializedObject = new(path.GetArrayElementAtIndex(startIndex).objectReferenceValue);
                WaypointPathCreator x = pathList[startIndex];
                serializedObject.FindProperty("StartPosition").vector2Value = x.GetEndVector();
                serializedObject.ApplyModifiedProperties();
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

        public virtual bool SelectPath(ref SerializedProperty selectedPathIndex, ref SerializedProperty pathTypeSelection,
            ref WaypointPathData pathData)
        {
            if (selectedPathIndex.intValue > pathData.Path.Count) selectedPathIndex.intValue = pathData.Path.Count;
            EditorGUILayout.LabelField("Selected Path:");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            {
                GUIContent backIcon = (selectedPathIndex.intValue > 0) ? EditorGUIUtility.IconContent("back") :
                    EditorGUIUtility.IconContent("d_back");
                GUIContent forwardIcon = (selectedPathIndex.intValue < pathData.Path.Count) ? EditorGUIUtility.IconContent("forward") :
                    EditorGUIUtility.IconContent("d_forward");

                if (GUILayout.Button(backIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                { 
                    if (selectedPathIndex.intValue > 0) selectedPathIndex.intValue--;
                    GUI.FocusControl(null);
                }

                selectedPathIndex.intValue = EditorGUILayout.IntField("", selectedPathIndex.intValue + 1) - 1;

                if (GUILayout.Button(forwardIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                {
                    if (selectedPathIndex.intValue < pathData.Path.Count) selectedPathIndex.intValue++;
                    GUI.FocusControl(null);
                }

                string outOfCount = "/" + pathData.Path.Count;
                EditorGUILayout.LabelField(outOfCount, GUILayout.MaxWidth(EditorStyles.label.CalcSize(new GUIContent(outOfCount)).x));
            }
            EditorGUILayout.EndHorizontal();

            if (pathData.Path.Count <= 0) return false;
            //Limit the range
            if (selectedPathIndex.intValue > pathData.Path.Count) selectedPathIndex.intValue = pathData.Path.Count - 1;
            if (0 > selectedPathIndex.intValue) selectedPathIndex.intValue = 0;

            var selectedPath = (pathData.Path.Count > selectedPathIndex.intValue) ? pathData.Path[selectedPathIndex.intValue]
                : null;

            if (!EditorGUI.EndChangeCheck() || selectedPath == null) return false;

            stepSize = selectedPath.StepSize;
            pathTypeSelection.intValue = WaypointPathEditorData.Options.FindIndex(
                kvp => kvp.GetPathCreator().GetType() == selectedPath.GetType()
                ); //Get index of used PathEditor child by comparing types

            return true;
        }

        public int DeletePath(ref SerializedProperty path, in List<WaypointPathCreator> pathList)
        {
            int result = 0;
            EditorGUILayout.LabelField("Delete Paths:");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("From ")).x;
                startDeleteIndex = EditorGUILayout.IntField("From", startDeleteIndex + 1) - 1;
                if (startDeleteIndex > path.arraySize) startDeleteIndex = endDeleteIndex = path.arraySize - 1;
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
                        if (path.arraySize > 0) ConnectPaths(ref path, in pathList, startDeleteIndex);
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUIUtility.labelWidth = 0;
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }

        public static void PathTypes(ref SerializedProperty pathTypeSelection)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Path type: ");

                pathTypeSelection.intValue = GUILayout.Toolbar(
                    pathTypeSelection.intValue, System.Enum.GetNames(typeof(PathType)), EditorStyles.radioButton);
            }
            EditorGUILayout.EndHorizontal();
        }

        public virtual void PathOptions()
        {
            stepSize = EditorGUILayout.Slider("Step size", stepSize, 0.2f, 50);
        }

        public bool SetPath(ref SerializedProperty path, in List<WaypointPathCreator> pathList, ref SerializedProperty isInsert,
            ref SerializedProperty selectedPathIndex)
        {
            bool result = false;
            EditorGUILayout.BeginHorizontal();
            {
                isInsert.boolValue = EditorGUILayout.ToggleLeft("Insert (before)", isInsert.boolValue);

                if (GUILayout.Button("Set path"))
                {
                    //Setting the new path in the edited object through serializedObject
                    if (path.arraySize <= selectedPathIndex.intValue) path.arraySize++;
                    else if (isInsert.boolValue)
                    {
                        path.InsertArrayElementAtIndex(selectedPathIndex.intValue);
                    }

                    SerializedObject so = new(path.GetArrayElementAtIndex(selectedPathIndex.intValue).objectReferenceValue);
                    SetPathProperties(ref so);

                    //If isInsert true, then start from inserted element
                    int startIndex = selectedPathIndex.intValue + (isInsert.boolValue ? 0 : 1)
                        - (selectedPathIndex.intValue >= path.arraySize - 1 ? 1 : 0);
                    ConnectPaths(ref path, in pathList, startIndex);
                    startDeleteIndex = endDeleteIndex = selectedPathIndex.intValue++;
                    result = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }

        public abstract void SetPathProperties(ref SerializedObject so);

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