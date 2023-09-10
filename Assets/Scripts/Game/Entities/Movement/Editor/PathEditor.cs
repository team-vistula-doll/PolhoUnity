using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public abstract class PathEditor : ScriptableObject
    {
        [SerializeField]
        protected float stepSize;
        public static int StartDeleteIndex = 0, EndDeleteIndex = 0;
        [SerializeField, SerializeReference]
        protected Vector2 startPosition;
        public Transform ObjectTransform;
        //private bool tempAddedOnEnd = true;

        protected virtual void OnEnable()
        { }

        public abstract WaypointPathCreator GetPathCreator();
        public virtual void SetPathCreator(WaypointPathCreator pathCreator)
        {
            stepSize = pathCreator.StepSize;
            startPosition = pathCreator.StartPosition;
        }

        public void ConnectPaths(SerializedProperty pathData, int startIndex)
        {
            List<WaypointPathCreator> path = new();
            for (int i = 0; i < pathData.arraySize; i++)
                path.Add((WaypointPathCreator)pathData.GetArrayElementAtIndex(i).managedReferenceValue);
            ConnectPaths(path, startIndex);
            pathData.arraySize = path.Count;
            for (int i = 0; i < path.Count; i++)
                pathData.GetArrayElementAtIndex(i).managedReferenceValue = path[i];
        }

        public void ConnectPaths(List<WaypointPathCreator> path, int startIndex)
        {
            if (path.Count == 0) return;
            //if (startIndex == 0)
            //{
            //    Vector2? vector2 = ObjectTransform.position;
            //    if (vector2 == null) return;
            //    vector2 -= path[startIndex].StartPosition;
            //    path[startIndex].ModifyPath((Vector2)vector2, (x, y) => x + y);
            //    startIndex++;
            //}
            for (; startIndex < path.Count; startIndex++)
            {
                Vector2? vector2 = startIndex == 0 ? ObjectTransform.position : path[startIndex - 1].GetVectorAt(1);
                if (vector2 == null) return;
                vector2 -= path[startIndex].StartPosition;
                path[startIndex].ModifyPath((Vector2)vector2, (x, y) => x + y);
            }
        }

        public bool SelectPath(SerializedProperty selectedPathIndex, SerializedProperty pathTypeSelection, SerializedProperty isInsert,
            SerializedProperty serialTempPath, List<WaypointPathCreator> path)
        {
            List<WaypointPathCreator> tempPath = new();
            for (int i = 0; i < serialTempPath.arraySize; i++)
                tempPath.Add((WaypointPathCreator)serialTempPath.GetArrayElementAtIndex(i).managedReferenceValue);

            if (selectedPathIndex.intValue > tempPath.Count) selectedPathIndex.intValue = tempPath.Count - 1;
            int wasSelectedIndex = selectedPathIndex.intValue;
            EditorGUILayout.LabelField("Select Path:");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            {
                GUIContent backIcon = (selectedPathIndex.intValue > 0) ? EditorGUIUtility.IconContent("back") :
                    EditorGUIUtility.IconContent("d_back");
                GUIContent forwardIcon = (selectedPathIndex.intValue < tempPath.Count - 1) ? EditorGUIUtility.IconContent("forward") :
                    EditorGUIUtility.IconContent("d_forward");

                if (GUILayout.Button(backIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                {
                    if (selectedPathIndex.intValue > 0) selectedPathIndex.intValue--;
                    GUI.FocusControl(null);
                }

                selectedPathIndex.intValue = EditorGUILayout.IntField("", selectedPathIndex.intValue + 1) - 1;

                if (GUILayout.Button(forwardIcon, GUILayout.MaxWidth(22), GUILayout.MinHeight(18)))
                {
                    if (selectedPathIndex.intValue < tempPath.Count - 1) selectedPathIndex.intValue++;
                    GUI.FocusControl(null);
                }

                string outOfCount = "/" + path.Count;
                EditorGUILayout.LabelField(outOfCount, GUILayout.MaxWidth(EditorStyles.label.CalcSize(new GUIContent(outOfCount)).x));
            }
            EditorGUILayout.EndHorizontal();
            //if (wasSelectedIndex != selectedPathIndex.intValue) SetPathCreator(tempPath[selectedPathIndex.intValue]);

            if (tempPath.Count <= 0) return false;
            //Limit the range
            if (selectedPathIndex.intValue >= tempPath.Count) selectedPathIndex.intValue = tempPath.Count - 1;
            if (0 > selectedPathIndex.intValue) selectedPathIndex.intValue = 0;

            int selectedPath = (tempPath.Count > selectedPathIndex.intValue) ? selectedPathIndex.intValue : -1;

            if (!EditorGUI.EndChangeCheck() || selectedPath < 0) return false;
            StartDeleteIndex = EndDeleteIndex = selectedPath;
            //TODO: delete last path in list when it's not selected
            //if (selectedPathIndex.intValue < tempPath.Count - 1 && tempAddedOnEnd)
            //{
            //    tempPath.RemoveAt(tempPath.Count - 1);
            //    tempAddedOnEnd = false;
            //}
            //else if (selectedPathIndex.intValue >= tempPath.Count - 1 && !tempAddedOnEnd)
            //{
            //    tempPath.Add(tempPath[^1].GetNewAdjoinedPath(1));
            //    tempAddedOnEnd = true;

            //}

            if (wasSelectedIndex < path.Count)
                tempPath[wasSelectedIndex] = path[wasSelectedIndex].GetNewAdjoinedPath(0);
            if (isInsert.boolValue)
            {
                if (wasSelectedIndex < tempPath.Count - 1)
                    tempPath.RemoveAt(wasSelectedIndex);
                if (selectedPathIndex.intValue < tempPath.Count - 1)
                    tempPath.Insert(selectedPathIndex.intValue, tempPath[selectedPathIndex.intValue].GetNewAdjoinedPath(1));
            }
            ConnectPaths(tempPath, wasSelectedIndex < selectedPathIndex.intValue ? wasSelectedIndex : selectedPathIndex.intValue);
            //pathTypeSelection.intValue = WaypointPathEditorData.Options.FindIndex(
            //    option => option.GetPathCreator().GetType() == tempPath[selectedPath].GetType()
            //    ); //Get index of used PathEditor child by comparing types
            pathTypeSelection.intValue = (int)WaypointPathEditorData.GetSelectedOption(tempPath[selectedPath]);

            WaypointPathEditorData.Options[pathTypeSelection.intValue].SetPathCreator(tempPath[selectedPath]);
            serialTempPath.arraySize = tempPath.Count;
            for (int i = 0; i < tempPath.Count; i++)
                serialTempPath.GetArrayElementAtIndex(i).managedReferenceValue = tempPath[i];

            return true;
        }

        public int DeletePath(SerializedProperty pathData, SerializedProperty serialTempPath)
        {
            List<WaypointPathCreator> tempPath = new();
            for (int i = 0; i < serialTempPath.arraySize; i++)
                tempPath.Add((WaypointPathCreator)serialTempPath.GetArrayElementAtIndex(i).managedReferenceValue);

            List<WaypointPathCreator> path = new();
            for (int i = 0; i < pathData.arraySize; i++)
                path.Add((WaypointPathCreator)pathData.GetArrayElementAtIndex(i).managedReferenceValue);
            int result = 0;
            EditorGUILayout.LabelField("Delete Paths:");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("From ")).x;
                StartDeleteIndex = EditorGUILayout.IntField("From", StartDeleteIndex + 1) - 1;
                if (StartDeleteIndex > path.Count) StartDeleteIndex = EndDeleteIndex = path.Count - 1;
                if (StartDeleteIndex < 0) StartDeleteIndex = 0;
                if (StartDeleteIndex > EndDeleteIndex) StartDeleteIndex = EndDeleteIndex;

                //EditorGUILayout.LabelField("-", GUILayout.MaxWidth(EditorStyles.label.CalcSize(
                //    new GUIContent("-")).x));

                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("To ")).x;
                EndDeleteIndex = EditorGUILayout.IntField("To", EndDeleteIndex + 1) - 1;
                if (EndDeleteIndex < StartDeleteIndex) EndDeleteIndex = StartDeleteIndex;
                if (EndDeleteIndex >= path.Count) EndDeleteIndex = path.Count - 1;
                if (StartDeleteIndex > EndDeleteIndex) StartDeleteIndex = EndDeleteIndex;

                EditorGUI.BeginDisabledGroup(path.Count == 0);
                {
                    if (GUILayout.Button("Delete"))
                    {
                        result = EndDeleteIndex - StartDeleteIndex + 1;
                        for (int i = StartDeleteIndex; i <= EndDeleteIndex; i++) 
                        {
                            path.RemoveAt(StartDeleteIndex);
                            if (tempPath.Count > 1) tempPath.RemoveAt(StartDeleteIndex);
                        }
                        if (path.Count > 0) ConnectPaths(path, StartDeleteIndex);
                        pathData.arraySize = path.Count;
                        for (int i = 0; i < path.Count; i++)
                            pathData.GetArrayElementAtIndex(i).managedReferenceValue = path[i];
                        ConnectPaths(tempPath, StartDeleteIndex);
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUIUtility.labelWidth = 0;
            }
            EditorGUILayout.EndHorizontal();

            serialTempPath.arraySize = tempPath.Count;
            for (int i = 0; i < tempPath.Count; i++)
                serialTempPath.GetArrayElementAtIndex(i).managedReferenceValue = tempPath[i];
            return result;
        }

        public static bool PathTypes(SerializedProperty pathTypeSelection, SerializedProperty selectedPathIndex,
            SerializedProperty serialTempPath)
        {
            bool result = false;
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Path type: ");
                EditorGUI.BeginChangeCheck();
                pathTypeSelection.intValue = GUILayout.Toolbar(
                    pathTypeSelection.intValue, System.Enum.GetNames(typeof(PathType)), EditorStyles.radioButton);
                if (EditorGUI.EndChangeCheck())
                {
                    result = true;
                    serialTempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue =
                        WaypointPathEditorData.Options[pathTypeSelection.intValue].GetPathCreator().GetNewAdjoinedPath(0);
                    WaypointPathEditorData.Options[pathTypeSelection.intValue].ConnectPaths(serialTempPath, selectedPathIndex.intValue);
                    WaypointPathEditorData.Options[pathTypeSelection.intValue].SetPathCreator(
                        (WaypointPathCreator)serialTempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
                }
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }

        public virtual bool PathOptions()
        {
            stepSize = EditorGUILayout.Slider("Step Size", stepSize, 0.2f, 50);
            return true;
        }

        public virtual void ApplyPathOptions()
        {
            GetPathCreator().StartPosition = startPosition;
            GetPathCreator().StepSize = stepSize;
        }

        public bool SetPath(SerializedProperty pathData, SerializedProperty isInsert,
            SerializedProperty selectedPathIndex, SerializedProperty serialTempPath)
        {
            bool result = false;
            List<WaypointPathCreator> tempPath = new();
            for (int i = 0; i < serialTempPath.arraySize; i++)
                tempPath.Add((WaypointPathCreator)serialTempPath.GetArrayElementAtIndex(i).managedReferenceValue);

            List<WaypointPathCreator> path = new();
            for (int i = 0; i < pathData.arraySize; i++)
                path.Add((WaypointPathCreator)pathData.GetArrayElementAtIndex(i).managedReferenceValue);

            EditorGUILayout.BeginHorizontal();
            {
                isInsert.boolValue = EditorGUILayout.ToggleLeft("Insert (before)", isInsert.boolValue);
                if (!isInsert.boolValue && tempPath.Count > path.Count + 1
                    && tempPath.Count > 1 && selectedPathIndex.intValue < tempPath.Count - 1)
                {
                    tempPath.RemoveAt(selectedPathIndex.intValue);
                    //tempPath.Add(tempPath[^1].GetNewAdjoinedPath(1));
                    ConnectPaths(tempPath, selectedPathIndex.intValue);
                    SetPathCreator(tempPath[selectedPathIndex.intValue]);
                }
                if (selectedPathIndex.intValue < path.Count &&
                    ((isInsert.boolValue && tempPath.Count <= path.Count + 1)
                    || tempPath.Count == 0 || selectedPathIndex.intValue >= tempPath.Count - 1)
                    )
                {
                    //tempPath[selectedPathIndex.intValue] = path[selectedPathIndex.intValue].GetNewAdjoinedPath(0);
                    tempPath.Insert(selectedPathIndex.intValue, path[selectedPathIndex.intValue].GetNewAdjoinedPath(0));
                    //tempPath.RemoveAt(tempPath.Count - 1);
                    ConnectPaths(tempPath, selectedPathIndex.intValue);
                    SetPathCreator(tempPath[selectedPathIndex.intValue]);
                }

                if (GUILayout.Button("Set path"))
                {
                    //Setting the new path in the edited object through serializedObject
                    int startSetIndex = selectedPathIndex.intValue;
                    if (isInsert.boolValue || startSetIndex >= path.Count)
                        path.Insert(startSetIndex, tempPath[startSetIndex++].GetNewAdjoinedPath(0));
                    for (; startSetIndex < path.Count; startSetIndex++)
                    {
                        path[startSetIndex] = tempPath[startSetIndex].GetNewAdjoinedPath(0);
                    }
                    //else if (isInsert.boolValue)
                    //{
                    //    path.InsertArrayElementAtIndex(selectedPathIndex.intValue);
                    //}

                    //path.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue =
                    //    tempPath[selectedPathIndex.intValue].GetNewAdjoinedPath(0);

                    //pathData.ApplyModifiedProperties();
                    //If isInsert true, then start from inserted element; if selected index is new then start one back
                    //int startIndex = selectedPathIndex.intValue + (isInsert.boolValue ? 0 : 1)
                    //    - (selectedPathIndex.intValue > path.Count ? 1 : 0);
                    ConnectPaths(path, selectedPathIndex.intValue);
                    pathData.arraySize = path.Count;
                    for (int i = 0; i < path.Count; i++)
                        pathData.GetArrayElementAtIndex(i).managedReferenceValue = path[i];
                    StartDeleteIndex = EndDeleteIndex = selectedPathIndex.intValue++;

                    //int index = isInsert.boolValue && selectedPathIndex.intValue < tempPath.Count - 1
                    //    ? selectedPathIndex.intValue
                    //    : tempPath.Count - 1;

                    if (tempPath.Count == path.Count)
                        tempPath.Insert(selectedPathIndex.intValue, tempPath[selectedPathIndex.intValue - 1].GetNewAdjoinedPath(1));
                    ConnectPaths(tempPath, selectedPathIndex.intValue);
                    //if (tempPath[selectedPathIndex.intValue].GetType() == this.GetType())
                    //    SetPathCreator(tempPath[selectedPathIndex.intValue]);

                    result = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            serialTempPath.arraySize = tempPath.Count;
            for (int i = 0; i < tempPath.Count; i++)
                serialTempPath.GetArrayElementAtIndex(i).managedReferenceValue = tempPath[i];
            return result;
        }

        //protected abstract void SetPathProperties(SerializedProperty sp);

        //public static List<Vector2> CreateVectorPath(List<WaypointPathCreator> path, int startIndex)
        //{
        //    List<Vector2> vector2s = new();
        //    for (;  startIndex < path.Count; startIndex++)
        //        vector2s.AddRange(path[startIndex].GeneratePath());
        //    return vector2s;
        //}


        public virtual void DrawPath(List<WaypointPathCreator> path, int startIndex, EventType e, bool isTemp = false)
        {
            if (path.Count == 0) return;
            Handles.color = isTemp ? new Color(1, 0.843f, 0) : Color.green;

            for (; startIndex < path.Count; startIndex++)
            {
                WaypointPathCreator x = path[startIndex];
                List<Vector2> vector2s = x.GeneratePath();
                foreach (Vector2 point in vector2s)
                {
                    Handles.SphereHandleCap(0, point, Quaternion.identity, isTemp ? 0.1f : 0.08f, EventType.Repaint);
                }
                if (isTemp) Handles.color = Color.red;
            }
        }
    }
}