using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabBrushWindow : EditorWindow
{
    private EnhancedPrefabBrush activeBrush;
    private Vector2 scrollPos;

    [MenuItem("Tools/Prefab Brush Window")]
    public static void ShowWindow()
    {
        GetWindow<PrefabBrushWindow>("Prefab Brush");
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab Brush Tool", EditorStyles.boldLabel);
        activeBrush = (EnhancedPrefabBrush)EditorGUILayout.ObjectField("Active Brush", activeBrush, typeof(EnhancedPrefabBrush), true);

        if (activeBrush == null)
        {
            EditorGUILayout.HelpBox("Assign an EnhancedPrefabBrush component to use the tool.", MessageType.Info);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("Brush Settings", EditorStyles.boldLabel);
        activeBrush.brushSize = EditorGUILayout.Slider("Brush Size", activeBrush.brushSize, 0.1f, 10f);
        activeBrush.spacing = EditorGUILayout.Slider("Spacing", activeBrush.spacing, 0.1f, 5f);
        activeBrush.paintDelay = EditorGUILayout.Slider("Paint Delay", activeBrush.paintDelay, 0.01f, 1f);
        activeBrush.prefabsPerStroke = EditorGUILayout.IntSlider("Prefabs Per Stroke", activeBrush.prefabsPerStroke, 1, 10);
        activeBrush.minScale = EditorGUILayout.Slider("Min Scale", activeBrush.minScale, 0.1f, 2f);
        activeBrush.maxScale = EditorGUILayout.Slider("Max Scale", activeBrush.maxScale, activeBrush.minScale, 2f);
        activeBrush.randomYRotation = EditorGUILayout.Toggle("Random Y Rotation", activeBrush.randomYRotation);
        activeBrush.maxSlopeAngle = EditorGUILayout.Slider("Max Slope Angle", activeBrush.maxSlopeAngle, 0f, 90f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Prefabs to Paint", EditorStyles.boldLabel);
        for (int i = 0; i < activeBrush.prefabsToPaint.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            activeBrush.prefabsToPaint[i] = (GameObject)EditorGUILayout.ObjectField(activeBrush.prefabsToPaint[i], typeof(GameObject), false);
            if (GUILayout.Button("Remove"))
                activeBrush.prefabsToPaint.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Prefab"))
            activeBrush.prefabsToPaint.Add(null);

        EditorGUILayout.Space();
        if (GUILayout.Button("Clear All Painted Prefabs"))
            activeBrush.ClearPrefabs();

        EditorGUILayout.LabelField($"Painted Prefabs: {activeBrush.GetPaintedPrefabs().Count}", EditorStyles.miniLabel);

        EditorGUILayout.EndScrollView();
    }
}