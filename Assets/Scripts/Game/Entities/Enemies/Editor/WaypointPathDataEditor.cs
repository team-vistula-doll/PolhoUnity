using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.IO;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    SerializedProperty pathFormula;
    SerializedProperty length;
    SerializedProperty angle;
    SerializedProperty startControl;
    SerializedProperty endControl;
    SerializedProperty endPosition;

    bool isReplace = false;

    private void OnEnable()
    {
        pathFormula = serializedObject.FindProperty("PathFormula");
        length = serializedObject.FindProperty("Length");
        angle = serializedObject.FindProperty("Angle");
        startControl = serializedObject.FindProperty("StartControl");
        endControl = serializedObject.FindProperty("EndControl");
        endPosition = serializedObject.FindProperty("EndPosition");
    }

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
                EditorGUILayout.PropertyField(pathFormula);
                EditorGUILayout.PropertyField(length);
                EditorGUILayout.PropertyField(angle);

                break;
            case 1:
                EditorGUILayout.PropertyField(endPosition);
                EditorGUI.BeginDisabledGroup(endPosition.vector2Value ==  Vector2.zero);
                    EditorGUILayout.PropertyField(endControl);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(endControl.vector2Value == Vector2.zero);
                    EditorGUILayout.PropertyField(startControl);
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
    bool isEndControlEnabled = false;
    //bool isStartControlEnabled = false;
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
            if (e.type == EventType.MouseUp && pathData.EndPosition != Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = true;
                isMousePressed = false;
            }
            else if (e.type == EventType.MouseUp && pathData.EndPosition == Vector2.zero && isMousePressed)
            {
                isEndControlEnabled = false;
                isMousePressed = false;
            }
            //***

            //if (isStartControlEnabled)
            //{
                size = HandleUtility.GetHandleSize(pathData.StartControl) * 0.15f;
                Handles.color = Color.cyan;
                startControlHandle = Handles.FreeMoveHandle(pathData.StartControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            //}
            //else startControlHandle = Vector2.zero;

            if (isEndControlEnabled)
            {
                size = HandleUtility.GetHandleSize(pathData.EndControl) * 0.15f;
                Handles.color = Color.cyan;
                endControlHandle = Handles.FreeMoveHandle(pathData.EndControl, Quaternion.identity, size, snap, Handles.SphereHandleCap);
            }
            else endControlHandle = Vector2.zero;

            size = HandleUtility.GetHandleSize(pathData.EndPosition) * 0.2f;
            Handles.color = Color.red;
            Vector2 endPositionHandle = Handles.FreeMoveHandle(pathData.EndPosition, Quaternion.identity, size, snap, Handles.SphereHandleCap);

            Handles.color = new Color(1,0,0,0.5f);
            if (startControlHandle !=  Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(startControlHandle, pathData.StartPosition, 2);
                Handles.color = new Color(1, 0.92f, 0.016f, 0.5f);
                Handles.DrawLine(endControlHandle, startControlHandle);
            }
            else if (endControlHandle != Vector2.zero)
            {
                Handles.DrawLine(endControlHandle, endPositionHandle, 2);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.DrawLine(endControlHandle, pathData.StartPosition, 2);
            }

            if (pathData._tempPath != null)
            {
                foreach (Vector2 point in _tempPath)
                {
                    // Draws a blue line from this transform to the target
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(point, 0.03f);
                }
            }
            if (Path != null)
            {
                foreach (Vector2 point in Path)
                {
                    // Draws a blue line from this transform to the target
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(point, 0.05f);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pathData, "Change Handle Position");
                pathData.EndPosition = endPositionHandle;
                pathData.StartControl = startControlHandle;
                pathData.EndControl = endControlHandle;
                pathData.ValidatePath(!isReplace, true);
            }
        }
    }
}
