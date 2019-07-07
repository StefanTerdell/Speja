using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshSpawner : MonoBehaviour
{
    public bool spawn;

    [Header("Grid")]
    public Vector2Int gridSize;
    public Vector2 distance;

    [Header("Node")]
    public bool pickRandomMaterial;
    public Material[] materials;
    public GameObject meshPrefab;

    [Header("Relations")]
    public GameObject linePrefab;

    public static Material GetRandomColor()
    {
        return instance.materials[Random.Range(0, instance.materials.Length - 1)];
    }

    static MeshSpawner instance;
    private void Awake()
    {
        instance = this;
    }

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

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var node = Instantiate(meshPrefab, new Vector3(x * distance.x, y * distance.y), Quaternion.identity, transform);

                    if (pickRandomMaterial)
                        foreach (var renderer in node.GetComponentsInChildren<MeshRenderer>())
                            renderer.material = materials[Random.Range(0, materials.Length - 1)];


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
