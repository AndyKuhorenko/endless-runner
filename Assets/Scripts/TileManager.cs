using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private int tilesInScene = 3;
    [SerializeField] private Tile tile;
    [SerializeField] SlowZombie slowZombie;
    [SerializeField] FastZombie fastZombie;
    [SerializeField] Weapon weapon;
    [SerializeField] GameObject monorail;

    private readonly int tileLength = 10;
    private float tileSpawnPosZ = 0;

    private List<Tile> activeTiles = new List<Tile>();
    private int tileObstacleSpawnChance = 20;


    private Player player;
    private float playerPosZ;

    void Start()
    {
        player = FindObjectOfType<Player>();
        
        for (int i = 0; i < tilesInScene; i++)
        {
            if (i == 0 || i == 1)
            {
                AddTile(false);
            }
            else
            {
                AddTile(true);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        SetNewPlayerPosZ();

        UpdateTiles();
    }

    private void UpdateTiles()
    {
        float passedTiles = 1.5f;

        if (playerPosZ > (tileSpawnPosZ - (tilesInScene - passedTiles) * tileLength))
        {
            RemoveTile();
            AddTile(true);
        }
    }
    private void SetNewPlayerPosZ()
    {
        playerPosZ = player.GetPosZ();
    }

    private void RemoveTile()
    {
        Tile firstTile = activeTiles[0];

        activeTiles.Remove(firstTile);

        Destroy(firstTile.gameObject);
    }

    private void AddTile(bool hasEnemies)
    {
        Tile newTile = Instantiate(tile, transform);

        newTile.transform.position = Vector3.forward * tileSpawnPosZ;

        if (hasEnemies)
        {
            newTile.obstacleSpawnChance = tileObstacleSpawnChance;
            newTile.hasEnemies = hasEnemies;

            newTile.hasObstacles = hasEnemies;

            newTile.slowZombie = slowZombie;
            newTile.fastZombie = fastZombie;

            newTile.weapon = weapon;

            newTile.monorail = monorail;
        }

        activeTiles.Add(newTile);

        this.tileSpawnPosZ += tileLength;
    }

    public void IncreaseObstacleSpawnChance()
    {
        tileObstacleSpawnChance += 10;
    }
}
