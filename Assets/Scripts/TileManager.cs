using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private int tilesInScene = 4;
    private readonly int tileLength = 10;
    private float tileSpawnPosZ = 0;

    private Tile tile;
    private List<Tile> activeTiles = new List<Tile>();


    private Player player;
    private float playerPosZ;

    void Start()
    {
        player = FindObjectOfType<Player>();
        tile = FindObjectOfType<Tile>();
        
        for (int i = 0; i < tilesInScene; i++)
        {
            AddTile();
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
            AddTile();
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

    private void AddTile()
    {
        Tile newTile = Instantiate(tile);

        newTile.transform.SetParent(transform);

        newTile.transform.position = Vector3.forward * tileSpawnPosZ;

        activeTiles.Add(newTile);

        this.tileSpawnPosZ += tileLength;
    }
}
