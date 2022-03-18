using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject slowZombie;
    public GameObject fastZombie;

    public bool hasObstacles = false;
    public bool hasEnemies = false;

    private const int tileDepth = 10;
    private const float cellDepth = 2.5f;
    private const float cellWidth = 2.5f;
    private const int envSpawnChance = 20;
    private const int corridorSpawnChance = 10;
    private const int obstacleSpawnChance = 10;

    private const float rows = 4;
    private const int columns = 6;

    private int zombieSpawnCount = 0;
    private int zombieSpawnChance = 20;

    private enum Track
    {
        Left = -1,
        Middle,
        Right,
    };
    private const int tracksCount = 3;
    private const float trackHeight = 0.1f;

    private GameObject[] envPrefabs;
    private GameObject[] obstaclePrefabs;

    void Start()
    {
        obstaclePrefabs = GameObject.FindGameObjectsWithTag("ObstaclePrefab");

        bool hasCorridor = Random.Range(0, 100) < corridorSpawnChance;

        if (hasCorridor) CreateCorridor();

        CreateEnvironment();

        if (hasEnemies) CreateEnemies();

        if (hasObstacles) CreateObstacles();
    }

    private void CreateCorridor()
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
                    GameObject prefab = envPrefabs[Random.Range(0, envPrefabs.Length - 1)];

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

    private void CreateEnemies()
    {
        for (int x = -1; x <= (int)Track.Right; x++)
        {
            bool isSpawned = Random.Range(0, 100) < zombieSpawnChance;

            if (zombieSpawnCount < 2 && isSpawned)
            {
                int randomNumber = Random.Range(0, 100);

                float posX = x + Random.Range(-0.2f, 0.2f);
                float posY = trackHeight;
                float posZ = transform.position.z + tileDepth + Random.Range(-2, 2);

                Quaternion rotation = Quaternion.Euler(0, 180, 0);

                GameObject zombie;

                if (randomNumber > 50)
                {
                    zombie = slowZombie;
                }
                else
                {
                    zombie = fastZombie;
                }

                Instantiate(zombie, new Vector3(posX, posY, posZ), rotation, transform);

                zombieSpawnCount++;
            }
        }    
    }

    private void CreateObstacles()
    {
        for (int x = -1; x <= (int)Track.Right; x++)
        {
            for (int row = 0; row < rows; row++)
            {
                bool isSpawned = Random.Range(0, 100) < obstacleSpawnChance;

                if (isSpawned)
                {
                    GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length - 1)];

                    float posX = x;
                    float posY = trackHeight;
                    float posZ = transform.position.z + row * cellDepth;

                    Quaternion rotation = Quaternion.Euler(0, 0, 0);

                    if (prefab.name.Contains("rail_end"))
                    {
                        posY = 0;

                        if (x == -1)
                        {
                            rotation = Quaternion.Euler(0, 180, 0);
                            posX = x - 0.5f;
                        }
                        else if (x == 0)
                        {
                            // todo
                        }
                        else if (x == 1)
                        {
                            posX = x + 0.5f;
                        }
                    }
                }
            }

            
        }
    }

    void Update()
    {
        
    }
}
