using UnityEngine;
using UnityEditor;
using WaypointPath;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    bool isReplace = false;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WaypointPathData pathData = target as WaypointPathData;

        SerializedProperty pathTypeSelection = serializedObject.FindProperty("PathTypeSelection");
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");
            string[] pathOptions = new string[] { "Function", "Bezier" };

            pathTypeSelection.intValue = GUILayout.Toolbar(pathTypeSelection.intValue, pathOptions, EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        switch (pathTypeSelection.intValue)
        {
            case 0:
                SerializedProperty pathFormula = serializedObject.FindProperty("PathFormula");
                SerializedProperty length = serializedObject.FindProperty("Length");
                SerializedProperty angle = serializedObject.FindProperty("Angle");

                EditorGUILayout.PropertyField(pathFormula);
                EditorGUILayout.PropertyField(length);
                EditorGUILayout.PropertyField(angle);

                break;
            case 1:

                SerializedProperty startControl = serializedObject.FindProperty("StartControl");
                SerializedProperty endControl = serializedObject.FindProperty("EndControl");
                SerializedProperty endPosition = serializedObject.FindProperty("EndPosition");

                EditorGUILayout.PropertyField(endPosition);
                EditorGUI.BeginDisabledGroup(endPosition.vector2Value ==  Vector2.zero);
                    EditorGUILayout.PropertyField(startControl);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(startControl.vector2Value == Vector2.zero);
                    EditorGUILayout.PropertyField(endControl);
                EditorGUI.EndDisabledGroup();

                break;
        }
        SerializedProperty stepSize = serializedObject.FindProperty("StepSize");
        EditorGUILayout.PropertyField(stepSize);

        EditorGUILayout.BeginHorizontal();
        {
            isReplace = EditorGUILayout.ToggleLeft("Replace", isReplace);

            if (GUILayout.Button("Set path"))
            {
                pathData.ValidatePath(!isReplace, false);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        pathData.ValidatePath(!isReplace, true);
        SceneView.RepaintAll();
    }

    bool isMousePressed = false;
    bool isStartControlEnabled = false;
    bool isEndControlEnabled = false;
    public void OnSceneGUI()
    {
        WaypointPathData pathData = target as WaypointPathData;
        Event e = Event.current;

        Vector2 snap = Vector2.one * 0.2f;

        EditorGUI.BeginChangeCheck();
        {
            float size;
            Vector2 startControlHandle = pathData.StartPosition;
            Vector2 endControlHandle = pathData.StartPosition;

            if (e.type == EventType.MouseDown) isMousePressed = true;

            //If the Start Control handle is right under the End Position handle, the SC handle has priority in selecting it,
            //however this doesn't happen with the End Control for some reason, both can overlap it and it won't get selected
            //The commented-out code for the EC handle also doesn't work, even though it's the same as for the SC handle

            ////***Prevent End Control handle from getting selected when Start Control is zero
            //if (e.type == EventType.MouseUp && pathData.StartControl != Vector2.zero && isMousePressed)
            //{
            //    isEndControlEnabled = true;
            //    isMousePressed = false;
            //}
            //else if (e.type == EventType.MouseUp && pathData.StartControl == Vector2.zero && isMousePressed)
            //{
            //    isEndControlEnabled = false;
            //    isMousePressed = false;
            //}
            ////***

            //***Prevent Start Control handle from getting selected when End Position is zero
            if (e.type == EventType.MouseUp && pathData.EndPosition != Vector2.zero && isMousePressed)
            {
                isStartControlEnabled = true;
                isMousePressed = false;
            }
            else if (e.type == EventType.MouseUp && pathData.EndPosition == Vector2.zero && isMousePressed)
            {
                isStartControlEnabled = false;
                isMousePressed = false;
            }
            //***

            //if (isEndControlEnabled)
            //{
                size = HandleUtility.GetHandleSize(pathData.EndControl) * 0.15f;
                Handles.color = Color.cyan;
                endControlHandle = Handles.FreeMoveHandle(pathData.EndControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            //}
            //else endControlHandle = Vector2.zero;

            if (isStartControlEnabled)
            {
                size = HandleUtility.GetHandleSize(pathData.StartControl) * 0.15f;
                Handles.color = Color.cyan;
                startControlHandle = Handles.FreeMoveHandle(pathData.StartControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            }
            else startControlHandle = Vector2.zero;

            size = HandleUtility.GetHandleSize(pathData.EndPosition) * 0.2f;
            Handles.color = Color.red;
            Vector2 endPositionHandle = Handles.FreeMoveHandle(pathData.EndPosition, Quaternion.identity, size, snap, Handles.SphereHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pathData, "Change Handle Position");
                pathData.EndPosition = endPositionHandle;
                pathData.StartControl = startControlHandle;
                pathData.EndControl = endControlHandle;
            }
        }
    }
}
