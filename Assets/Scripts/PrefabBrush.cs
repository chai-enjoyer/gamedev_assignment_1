using UnityEngine;
using System.Collections.Generic;

public class EnhancedPrefabBrush : MonoBehaviour
{
    [Header("Brush Settings")]
    public List<GameObject> prefabsToPaint = new List<GameObject>(); 
    public float brushSize = 1f; 
    public float spacing = 0.5f; 
    public float paintDelay = 0.1f; 
    public int prefabsPerStroke = 3; 
    public float minScale = 0.8f; 
    public float maxScale = 1.2f; 
    public bool randomYRotation = true; 
    public float maxSlopeAngle = 45f; 

    [Header("Painted Instances")]
    [SerializeField] private List<GameObject> paintedPrefabs = new List<GameObject>(); 
    private float lastPaintTime = 0f;

    public void PaintPrefabs(Vector3 center, Vector3 normal)
    {
        if (prefabsToPaint.Count == 0 || prefabsToPaint[0] == null) return;

        if (Time.realtimeSinceStartup - lastPaintTime < paintDelay) return;


        for (int i = 0; i < prefabsPerStroke; i++)
        {

            Vector2 randomOffset = Random.insideUnitCircle * brushSize;
            Vector3 position = center + new Vector3(randomOffset.x, 0, randomOffset.y);


            Ray ray = new Ray(position + Vector3.up * 10f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f))
            {
                position = hit.point;
                normal = hit.normal;
            }
            else
            {
                continue; 
            }


            float slopeAngle = Vector3.Angle(normal, Vector3.up);
            if (slopeAngle > maxSlopeAngle) continue;


            bool tooClose = false;
            foreach (GameObject existing in paintedPrefabs)
            {
                if (existing != null && Vector3.Distance(existing.transform.position, position) < spacing)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;


            GameObject prefab = prefabsToPaint[Random.Range(0, prefabsToPaint.Count)];
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);


            instance.transform.up = normal;


            if (randomYRotation)
                instance.transform.Rotate(0, Random.Range(0, 360f), 0);
            float scale = Random.Range(minScale, maxScale);
            instance.transform.localScale = Vector3.one * scale;

            instance.transform.parent = transform;


            paintedPrefabs.Add(instance);
        }

        lastPaintTime = Time.realtimeSinceStartup;
    }

    public void ErasePrefabs(Vector3 position)
    {
        for (int i = paintedPrefabs.Count - 1; i >= 0; i--)
        {
            if (paintedPrefabs[i] != null && Vector3.Distance(paintedPrefabs[i].transform.position, position) < brushSize)
            {
                DestroyImmediate(paintedPrefabs[i]); 
                paintedPrefabs.RemoveAt(i);
            }
        }
    }

    public void ClearPrefabs()
    {
        for (int i = paintedPrefabs.Count - 1; i >= 0; i--)
        {
            if (paintedPrefabs[i] != null)
                DestroyImmediate(paintedPrefabs[i]); 
        }
        paintedPrefabs.Clear();
    }

    public List<GameObject> GetPaintedPrefabs()
    {
        return paintedPrefabs;
    }
}