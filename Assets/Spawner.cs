using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int rows, columns;
    public bool spawn;
    public GameObject prefab;

    List<GameObject> spawnedItems = new List<GameObject>();

    void Update()
    {
        if (spawn)
        {
            spawn = false;

            foreach (var item in spawnedItems)
            {
                Destroy(item);
            }

            spawnedItems.Clear();

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    spawnedItems.Add(Instantiate(prefab, new Vector3(x * 6, y * 6), Quaternion.identity, transform));
                }
            }
        }
    }
}
