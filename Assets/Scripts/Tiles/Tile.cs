using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SlowZombie slowZombie;
    public FastZombie fastZombie;
    public Weapon weapon;
    public GameObject[] monorails;
    [SerializeField] LaunchPad launchPad;

    public bool hasObstacles = false;
    public bool hasEnemies = false;
    public int obstacleSpawnChance = 0;

    private const int tileDepth = 10;
    private const float envCellDepth = 2.5f;
    private const float envCellWidth = 2.5f;

    private const int envSpawnChance = 10;
    private const int corridorSpawnChance = 10;

    private bool hasMonorail = false;
    private const int monorailSpawnChance = 5;

    private bool hasLaunchPad = false;
    private bool launchPadAtLeft = false;
    private const int launchPadSpawnChance = 5;

    private const float obstacleCellDepth = 3.5f;
    private List<float> takenPosZ = new List<float>(); 

    private const float rows = 4;
    private const int columns = 6;

    private int zombieSpawnCount = 0;
    private int zombieSpawnChance = 20;

    private int weaponSpawnChance = 5;

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

        if (hasCorridor)
        {
            CreateCorridor();
        }
        else
        {
            hasMonorail = Random.Range(0, 100) < monorailSpawnChance;
            hasLaunchPad = Random.Range(0, 100) < launchPadSpawnChance;

            if (false)
            {
                CreateMonorail();
            }
            else if (true)
            {
                CreateLaunchPad();
            }
        }

        CreateEnvironment();

        if (hasEnemies) CreateEnemies();

        if (hasObstacles) CreateObstacles();
    }

    private void CreateCorridor()
    {
        GameObject corridor = GameObject.FindGameObjectWithTag("Corridor");

        Instantiate(corridor, transform);
    }

    private void CreateMonorail()
    {
        GameObject monorail = monorails[Random.Range(0, monorails.Length)];

        Instantiate(monorail, new Vector3(0, 0, transform.position.z + tileDepth / 2), Quaternion.identity, transform);
    }

    private void CreateLaunchPad()
    {
        if (Random.Range(0, 100) > 50) launchPadAtLeft = true;

        if (launchPadAtLeft)
        {
            Instantiate(launchPad, new Vector3(-8, 0, transform.position.z + tileDepth / 2), Quaternion.identity, transform);
        }
        else
        {
            Instantiate(launchPad, new Vector3(8, 0, transform.position.z + tileDepth / 2), Quaternion.identity, transform);
        }
    }

    private void CreateEnvironment()
    {
        envPrefabs = GameObject.FindGameObjectsWithTag("EnvironmentPrefab");

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                bool isSpawned = Random.Range(0, 100) < envSpawnChance;

                // 2nd row is monorail position
                if ((hasMonorail && row != 2 && isSpawned) || (!hasMonorail && isSpawned) || (hasLaunchPad && launchPadAtLeft && col > columns / 2 && true))
                {
                    GameObject prefab = envPrefabs[Random.Range(0, envPrefabs.Length)];

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
                float posZ = transform.position.z + tileDepth + Random.Range(-3, 3);

                Quaternion rotation = Quaternion.Euler(0, 180, 0);

                Enemy zombie;

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
                bool isObstacleSpawn = Random.Range(0, 100) < obstacleSpawnChance;
                bool isWeaponSpawn = Random.Range(0, 100) < weaponSpawnChance;

                float posZ = transform.position.z + row * obstacleCellDepth;

                List<float> spawnedInZ = takenPosZ.FindAll(pos => pos == posZ);

                if (isObstacleSpawn && spawnedInZ.Count < 2)
                {
                    GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

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

                    if (!prefab.name.Contains("rail_middle"))
                    {
                        Instantiate(prefab, new Vector3(posX, posY, posZ + randomPosZ), rotation, transform);
                    }
                    else if (prefab.name.Contains("rail_middle") && posX == 0)
                    {
                        Instantiate(prefab, new Vector3(posX, posY, posZ + randomPosZ), rotation, transform);
                    }

                }
                else if (isWeaponSpawn)
                {
                    Instantiate(weapon, new Vector3(x, trackHeight + 0.5f, posZ), Quaternion.identity, transform);
                }
            }
        }
    }

    void Update()
    {
        
    }
}
