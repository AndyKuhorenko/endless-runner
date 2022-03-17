using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private const float cellDepth = 2.5f;
    private const float cellWidth = 2.5f;
    private const int envSpawnChance = 20;
    private const int corridorSpawnChance = 10;

    private const float rows = 4;
    private const int columns = 6;

    private GameObject[] envPrefabs;

    void Start()
    {
        bool hasCorridor = Random.Range(0, 100) < corridorSpawnChance;

        if (hasCorridor) createCorridor();

        CreateEnvironment();
    }

    private void createCorridor()
    {
        GameObject corridor = GameObject.FindGameObjectWithTag("Corridor");

        Instantiate(corridor, transform);
    }

    private void CreateEnvironment()
    {
        envPrefabs = GameObject.FindGameObjectsWithTag("EnvironmentPrefab");

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                bool isSpawned = Random.Range(0, 100) < envSpawnChance;

                if (isSpawned)
                {
                    GameObject prefab = envPrefabs[Random.Range(0, envPrefabs.Length)];

                    Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                    float newPosZ = transform.position.z + row * cellDepth;

                    if (col < columns / 2)
                    {
                        float newPosX = -4 - col * cellWidth;

                        Instantiate(prefab, new Vector3(newPosX, 0, newPosZ), randomRotation, transform);
                    }
                    else
                    {
                        float newPosX = -3.5f + col * cellWidth;

                        Instantiate(prefab, new Vector3(newPosX, 0, newPosZ), randomRotation, transform);
                    }
                }
            }
        }
    }

    void Update()
    {
        
    }
}
