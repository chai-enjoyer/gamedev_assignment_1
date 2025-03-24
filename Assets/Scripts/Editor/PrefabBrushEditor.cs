using UnityEngine;
using UnityEditor;
using System.Collections.Generic; 

[CustomEditor(typeof(EnhancedPrefabBrush))]
public class EnhancedPrefabBrushEditor : Editor
{
    private EnhancedPrefabBrush brush;
    private bool isPainting = false;
    private bool isErasing = false;

    void OnEnable()
    {
        brush = (EnhancedPrefabBrush)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Controls:", EditorStyles.boldLabel);
        GUILayout.Label("- Left Mouse: Paint Prefabs");
        GUILayout.Label("- Right Mouse: Erase Prefabs");

        GUILayout.Space(10);
        if (GUILayout.Button("Clear All Painted Prefabs"))
        {
            Undo.RegisterCompleteObjectUndo(brush, "Clear Prefabs");
            brush.ClearPrefabs();
        }


        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            Handles.color = isErasing ? Color.red : Color.green;
            Handles.DrawWireDisc(hit.point, hit.normal, brush.brushSize);


            if (e.type == EventType.MouseDown && e.button == 0) 
            {
                isPainting = true;
                isErasing = false;
                PaintWithUndo(hit.point, hit.normal);
                e.Use();
            }
            else if (e.type == EventType.MouseUp && e.button == 0)
            {
                isPainting = false;
            }


            if (e.type == EventType.MouseDown && e.button == 1)
            {
                isErasing = true;
                isPainting = false;
                EraseWithUndo(hit.point);
                e.Use();
            }
            else if (e.type == EventType.MouseUp && e.button == 1)
            {
                isErasing = false;
            }


            if (e.type == EventType.MouseDrag)
            {
                if (isPainting && e.button == 0)
                {
                    PaintWithUndo(hit.point, hit.normal);
                    e.Use();
                }
                else if (isErasing && e.button == 1)
                {
                    EraseWithUndo(hit.point);
                    e.Use();
                }
            }
        }

        SceneView.RepaintAll(); 
    }

    private void PaintWithUndo(Vector3 position, Vector3 normal)
    {

        Undo.RegisterCompleteObjectUndo(brush, "Paint Prefabs");


        brush.PaintPrefabs(position, normal);


        List<GameObject> painted = brush.GetPaintedPrefabs();
        for (int i = painted.Count - brush.prefabsPerStroke; i < painted.Count; i++)
        {
            if (i >= 0 && painted[i] != null)
                Undo.RegisterCreatedObjectUndo(painted[i], "Paint Prefab");
        }
    }

    private void EraseWithUndo(Vector3 position)
    {
        Undo.RegisterCompleteObjectUndo(brush, "Erase Prefabs");
        brush.ErasePrefabs(position);
    }
}