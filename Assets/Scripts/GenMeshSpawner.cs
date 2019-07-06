using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenMeshSpawner : MonoBehaviour
{
    public bool spawn;

    [Header("Grid")]
    public Vector2Int gridSize;
    public Vector2 distance;

    [Header("Node")]
    public bool pickRandomMaterial;
    public Material[] materials;
    public GameObject meshPrefab;
    public int corners = 16;
    public float radius = 1;
    [Header("Relations")]
    public GameObject linePrefab;



    Vector3 lineOffset = new Vector3(0, 0, .001f);
    List<GameObject> spawnedNodes = new List<GameObject>();
    List<GameObject> spawnedRels = new List<GameObject>();

    void Update()
    {
        if (spawn)
        {
            spawn = false;

            foreach (var item in spawnedNodes)
            {
                Destroy(item);
            }

            spawnedNodes.Clear();

            foreach (var item in spawnedRels)
            {
                Destroy(item);
            }

            spawnedRels.Clear();

            var mesh = MeshGenerator.GetPrimitive2DMesh(corners, radius);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var node = Instantiate(meshPrefab, new Vector3(x * distance.x, y * distance.y), Quaternion.identity, transform);
                    node.GetComponent<MeshFilter>().mesh = mesh;

                    if (pickRandomMaterial)
                        node.GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length - 1)];


                    if (spawnedNodes.Count > 0)
                    {
                        var rel = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, transform);
                        var lr = rel.GetComponent<LineRenderer>();
                        lr.SetPositions(new[] { node.transform.position + lineOffset, spawnedNodes[Random.Range(0, spawnedNodes.Count - 1)].transform.position + lineOffset });
                    }

                    spawnedNodes.Add(node);
                }
            }
        }
    }
}
