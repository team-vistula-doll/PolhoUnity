using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public abstract class PathEditor : ScriptableObject
    {
        [SerializeField]
        protected float stepSize;
        public int startDeleteIndex = 0, endDeleteIndex = 0;
        [SerializeField]
        protected Vector2 startPosition;
        public Transform objectTransform;
        private bool tempIsInsert = false;

        protected virtual void OnEnable()
        {
            tempIsInsert = false;
        }

        public abstract WaypointPathCreator GetPathCreator();
        public virtual void SetPathCreator(WaypointPathCreator pathCreator)
        {
            stepSize = pathCreator.StepSize;
            startPosition = pathCreator.StartPosition;
        }

        //public void ConnectPaths(SerializedObject pathData, List<WaypointPathCreator> pathList, int startIndex)
        //{
        //    pathData.UpdateIfRequiredOrScript();
        //    SerializedProperty path = pathData.FindProperty("Path");
        //    if (startIndex == 0)
        //    {
        //        SerializedProperty sp = path.GetArrayElementAtIndex(startIndex);
        //        sp.FindPropertyRelative(nameof(WaypointPathCreator.StartPosition)).vector2Value = objectTransform.position;
        //        pathData.ApplyModifiedPropertiesWithoutUndo();
        //        startIndex++;
        //    }
        //    for (; startIndex < path.arraySize; startIndex++)
        //    {
        //        pathData.UpdateIfRequiredOrScript();
        //        SerializedProperty sp = path.GetArrayElementAtIndex(startIndex);
        //        WaypointPathCreator x = pathList[startIndex - 1];
        //        sp.FindPropertyRelative(nameof(WaypointPathCreator.StartPosition)).vector2Value = x.GetVectorAt(1);
        //        pathData.ApplyModifiedPropertiesWithoutUndo();
        //    }
        //}

        public void ConnectPaths(List<WaypointPathCreator> path, int startIndex)
        {
            if (startIndex == 0)
            {
                path[startIndex].StartPosition = objectTransform.position;
                startIndex++;
            }
            for (; startIndex < path.Count; startIndex++)
            {
                Vector2? vector2 = path[startIndex - 1].GetVectorAt(1);
                if (vector2 != null) path[startIndex].StartPosition = (Vector2)vector2;
                else return;
            }
        }

        public bool SelectPath(SerializedProperty selectedPathIndex, SerializedProperty pathTypeSelection, SerializedProperty isInsert,
            List<WaypointPathCreator> tempPath, List<WaypointPathCreator> path)
        {
            if (selectedPathIndex.intValue > path.Count) selectedPathIndex.intValue = path.Count;
            int wasSelectedIndex = selectedPathIndex.intValue;
            EditorGUILayout.LabelField("Select Path: End = " + GetPathCreator().GetVectorAt(1));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            {
                GUIContent backIcon = (selectedPathIndex.intValue > 0) ? EditorGUIUtility.IconContent("back") :
                    EditorGUIUtility.IconContent("d_back");
                GUIContent forwardIcon = (selectedPathIndex.intValue < path.Count) ? EditorGUIUtility.IconContent("forward") :
                    EditorGUIUtility.IconContent("d_forward");

                if (GUILayout.Button(backIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                { 
                    if (selectedPathIndex.intValue > 0) selectedPathIndex.intValue--;
                    GUI.FocusControl(null);
                }

                selectedPathIndex.intValue = EditorGUILayout.IntField("", selectedPathIndex.intValue + 1) - 1;

                if (GUILayout.Button(forwardIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                {
                    if (selectedPathIndex.intValue < path.Count) selectedPathIndex.intValue++;
                    GUI.FocusControl(null);
                }

                string outOfCount = "/" + path.Count;
                EditorGUILayout.LabelField(outOfCount, GUILayout.MaxWidth(EditorStyles.label.CalcSize(new GUIContent(outOfCount)).x));
            }
            EditorGUILayout.EndHorizontal();

            if (path.Count <= 0) return false;
            //Limit the range
            if (selectedPathIndex.intValue > path.Count) selectedPathIndex.intValue = path.Count - 1;
            if (0 > selectedPathIndex.intValue) selectedPathIndex.intValue = 0;

            WaypointPathCreator selectedPath = (tempPath.Count > selectedPathIndex.intValue) ? tempPath[selectedPathIndex.intValue] : null;

            if (!EditorGUI.EndChangeCheck() || selectedPath == null) return false;

            if (wasSelectedIndex < path.Count)
                tempPath[wasSelectedIndex] = path[wasSelectedIndex].GetNewAdjoinedPath(0);
            if (isInsert.boolValue && tempIsInsert)
            {
                tempPath.RemoveAt(wasSelectedIndex);
                tempIsInsert = false;
            }
            if (isInsert.boolValue && !tempIsInsert)
            {
                tempPath.Insert(selectedPathIndex.intValue, GetPathCreator().GetNewAdjoinedPath(0));
                tempIsInsert = true;
            }
            ConnectPaths(tempPath, wasSelectedIndex < selectedPathIndex.intValue ? wasSelectedIndex : selectedPathIndex.intValue);
            pathTypeSelection.intValue = WaypointPathEditorData.Options.FindIndex(
                option => option.GetPathCreator().GetType() == selectedPath.GetType()
                ); //Get index of used PathEditor child by comparing types

            WaypointPathEditorData.Options[pathTypeSelection.intValue].SetPathCreator(selectedPath);

            return true;
        }

        public int DeletePath(SerializedObject pathData, List<WaypointPathCreator> pathList,
            List<WaypointPathCreator> tempPath)
        {
            pathData.Update();
            SerializedProperty path = pathData.FindProperty("Path");
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
                if (endDeleteIndex < startDeleteIndex) endDeleteIndex = startDeleteIndex;
                if (endDeleteIndex >= path.arraySize) endDeleteIndex = path.arraySize - 1;
                if (startDeleteIndex > endDeleteIndex) startDeleteIndex = endDeleteIndex;

                EditorGUI.BeginDisabledGroup(path.arraySize == 0);
                {
                    if (GUILayout.Button("Delete"))
                    {
                        result = endDeleteIndex - startDeleteIndex + 1;
                        for (int i = startDeleteIndex; i <= endDeleteIndex; i++) 
                        {
                            path.DeleteArrayElementAtIndex(startDeleteIndex);
                            if (tempPath.Count > 1) tempPath.RemoveAt(startDeleteIndex);
                        }
                        pathData.ApplyModifiedProperties();
                        if (path.arraySize > 0) ConnectPaths(pathList, startDeleteIndex);
                        ConnectPaths(tempPath, startDeleteIndex);
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUIUtility.labelWidth = 0;
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }

        public static void PathTypes(SerializedProperty pathTypeSelection)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Path type: ");

                pathTypeSelection.intValue = GUILayout.Toolbar(
                    pathTypeSelection.intValue, System.Enum.GetNames(typeof(PathType)), EditorStyles.radioButton);
            }
            EditorGUILayout.EndHorizontal();
        }

        public virtual bool PathOptions()
        {
            stepSize = EditorGUILayout.Slider("Step Size", stepSize, 0.2f, 50); //Sliders currently aren't recorded to Undo history
            //stepSize = EditorGUILayout.FloatField("Step Size", stepSize);
            //if (stepSize < 0.2f) stepSize = 0.2f;
            //else if (stepSize > 50f) stepSize = 50f;
            return true;
        }

        protected virtual void ApplyPathOptions()
        {
            GetPathCreator().StartPosition = startPosition;
            GetPathCreator().StepSize = stepSize;
        }

        public bool SetPath(SerializedObject pathData, List<WaypointPathCreator> pathList, SerializedProperty isInsert,
            SerializedProperty selectedPathIndex, List<WaypointPathCreator> tempPath)
        {
            bool result = false;
            pathData.Update();
            SerializedProperty path = pathData.FindProperty("Path");
            EditorGUILayout.BeginHorizontal();
            {
                isInsert.boolValue = EditorGUILayout.ToggleLeft("Insert (before)", isInsert.boolValue);
                if (!isInsert.boolValue && tempIsInsert && tempPath.Count > 1 && selectedPathIndex.intValue < tempPath.Count)
                {
                    tempPath.RemoveAt(selectedPathIndex.intValue);
                    ConnectPaths(tempPath, selectedPathIndex.intValue);
                    SetPathCreator(tempPath[selectedPathIndex.intValue]);
                    tempIsInsert = false;
                }
                if (selectedPathIndex.intValue < pathList.Count &&
                    ((isInsert.boolValue && !tempIsInsert) || tempPath.Count == 0 || selectedPathIndex.intValue >= tempPath.Count)
                    )
                {
                    tempPath[selectedPathIndex.intValue] = pathList[selectedPathIndex.intValue].GetNewAdjoinedPath(0);
                    tempPath.Insert(selectedPathIndex.intValue, GetPathCreator().GetNewAdjoinedPath(0));
                    ConnectPaths(tempPath, selectedPathIndex.intValue);
                    SetPathCreator(tempPath[selectedPathIndex.intValue]);
                    if (isInsert.boolValue && !tempIsInsert) tempIsInsert = true;
                }

                if (GUILayout.Button("Set path"))
                {
                    //Setting the new path in the edited object through serializedObject
                    if (path.arraySize <= selectedPathIndex.intValue) path.arraySize++;
                    else if (isInsert.boolValue)
                    {
                        path.InsertArrayElementAtIndex(selectedPathIndex.intValue);
                    }

                    path.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue =
                        tempPath[selectedPathIndex.intValue].GetNewAdjoinedPath(0);

                    pathData.ApplyModifiedProperties();
                    //If isInsert true, then start from inserted element; if selected index is new then start one back
                    int startIndex = selectedPathIndex.intValue + (isInsert.boolValue ? 0 : 1)
                        - (selectedPathIndex.intValue > path.arraySize ? 1 : 0);
                    ConnectPaths(pathList, startIndex);
                    startDeleteIndex = endDeleteIndex = selectedPathIndex.intValue++;

                    tempPath.Add(tempPath[isInsert.boolValue ? selectedPathIndex.intValue : ^1].GetNewAdjoinedPath(0));
                    ConnectPaths(tempPath, isInsert.boolValue ? selectedPathIndex.intValue : tempPath.Count - 2);
                    SetPathCreator(tempPath[selectedPathIndex.intValue]);

                    result = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }

        protected abstract void SetPathProperties(SerializedProperty sp);

        //public static List<Vector2> CreateVectorPath(List<WaypointPathCreator> path, int startIndex)
        //{
        //    List<Vector2> vector2s = new();
        //    for (;  startIndex < path.Count; startIndex++)
        //        vector2s.AddRange(path[startIndex].GeneratePath());
        //    return vector2s;
        //}


        public virtual void DrawPath(List<WaypointPathCreator> path, int startIndex, EventType e, bool isTemp = false)
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