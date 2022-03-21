using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject slowZombie;
    public GameObject fastZombie;

    public bool hasObstacles = false;
    public bool hasEnemies = false;
    public int obstacleSpawnChance = 0;

    private const int tileDepth = 10;
    private const float envCellDepth = 2.5f;
    private const float envCellWidth = 2.5f;
    private const int envSpawnChance = 20;
    private const int corridorSpawnChance = 10;

    private const float obstacleCellDepth = 3.5f;
    private List<float> takenPosZ = new List<float>(); 

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

                    float newPosZ = transform.position.z + row * envCellDepth;

                    if (col < columns / 2)
                    {
                        float newPosX = -4 - col * envCellWidth;

                        Instantiate(prefab, new Vector3(newPosX, 0, newPosZ), randomRotation, transform);
                    }
                    else
                    {
                        float newPosX = -3.5f + col * envCellWidth;

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
            for (int row = 0; row < 3; row++)
            {
                bool isSpawn = Random.Range(0, 100) < obstacleSpawnChance;
                float posZ = transform.position.z + row * obstacleCellDepth;

                List<float> spawnedInZ = takenPosZ.FindAll(pos => pos == posZ);

                if (isSpawn && spawnedInZ.Count < 2)
                {
                    GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length - 1)];

                    float posX = x;
                    float posY = trackHeight;

                    Quaternion rotation = Quaternion.Euler(0, 0, 0);

                    if (prefab.name.Contains("rail_end"))
                    {
                        if (x == -1)
                        {
                            posY = 0;
                            posX = x - 0.25f;
                        }
                        else if (x == 0)
                        {
                            float randomNumber = Random.Range(0, 100);

                            foreach (GameObject obstacle in obstaclePrefabs)
                            {
                                if (randomNumber > 50)
                                {
                                    if (obstacle.name == "rock") prefab = obstacle;
                                }
                                else
                                {
                                    if (obstacle.name == "rail") prefab = obstacle;
                                }
                            }
                        }
                        else if (x == 1)
                        {
                            posY = 0;
                            posX = x + 0.25f;

                            rotation = Quaternion.Euler(0, 180, 0);
                        }
                    }

                    takenPosZ.Add(posZ);

                    float randomPosZ = Random.Range(-1, 1);

                    Instantiate(prefab, new Vector3(posX, posY, posZ + randomPosZ), rotation, transform);
                }
            }
        }
    }

    void Update()
    {
        
    }
}
