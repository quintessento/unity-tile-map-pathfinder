using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileMap : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private Tile _tilePrefab = null;
    [SerializeField]
    private GameObject _obstaclePrefab = null;

    [Header("Map Settings")]
    [SerializeField]
    private int _sizeX = 10;
    [SerializeField]
    private int _sizeZ = 10;
    [SerializeField]
    private int _numObstacles = 4;

    [Header("Other")]
    [SerializeField]
    private TileSelector _tileSelector = null;

    private Tile[,] _tiles;
    private int[,] _tileIntMap;

    private List<Tile> _allTiles = new List<Tile>();
    private List<Tile> _emptyTiles = new List<Tile>();
    private List<GameObject> _obstacles = new List<GameObject>();

    public void GenerateMap()
    {
        _tileSelector.Reset();

        for (int i = 0; i < _allTiles.Count; i++)
        {
            Destroy(_allTiles[i].gameObject);
        }
        _allTiles.Clear();

        for (int i = 0; i < _obstacles.Count; i++)
        {
            Destroy(_obstacles[i].gameObject);
        }
        _obstacles.Clear();

        _emptyTiles.Clear();

        _tiles = new Tile[_sizeX, _sizeZ];
        _tileIntMap = new int[_sizeX, _sizeZ];


        for (int x = 0; x < _sizeX; x++)
        {
            for (int z = 0; z < _sizeZ; z++)
            {
                Tile tile = Instantiate(_tilePrefab, transform);
                tile.Initialize(
                    new Vector3(x, 0f, z), 
                    Random.ColorHSV(0.19f, 0.2f, 0.5f, 0.6f, 0.7f, 0.8f)
                );
                _tiles[x, z] = tile;
                tile.x = x;
                tile.z = z;
                tile.IsOccupied = false;
                _tileIntMap[x, z] = 0;

                _allTiles.Add(tile);
                _emptyTiles.Add(tile);
            }
        }

        for (int i = 0; i < _numObstacles; i++)
        {
            PlaceObstacles();
        }
    }

    private void PlaceObstacles()
    {
        int[,] obstacleBlueprint = Obstacles.obstacleL1;

        int obstacleSizeX = obstacleBlueprint.GetLength(0);
        int obstacleSizeZ = obstacleBlueprint.GetLength(1);

        List<List<Tuple<int, int>>> possiblePlacements = new List<List<Tuple<int, int>>>();

        for (int x = 0; x < _sizeX - obstacleSizeZ + 1; x++)
        {
            for (int z = 0; z < _sizeZ - obstacleSizeX + 1; z++)
            {
                bool fits = true;
                List<Tuple<int, int>> coords = new List<Tuple<int, int>>();
                
                for (int i = 0; i < obstacleSizeZ; i++)
                {
                    for (int j = 0; j < obstacleSizeX; j++)
                    {
                        //reverse x so that we get the blueprint values in the correct order
                        if (obstacleBlueprint[obstacleSizeX - j - 1, i] == 0)
                        {
                            //skip empty
                            continue;
                        }

                        int obstacleX = x + i;
                        int obstacleZ = z + j;
                        if(_tileIntMap[obstacleX, obstacleZ] == 1)
                        {
                            //placement is invalid, because one of the required tiles is already occupied
                            fits = false;
                            continue;
                        }
                        coords.Add(new Tuple<int, int>(obstacleX, obstacleZ));
                    }
                }

                if (fits)
                {
                    possiblePlacements.Add(coords);
                }
            }
        }

        if (possiblePlacements.Count > 0)
        {
            List<Tuple<int, int>> placement = possiblePlacements[Random.Range(0, possiblePlacements.Count)];
            PlaceObstacle(placement);
        }
    }

    private void PlaceObstacle(List<Tuple<int, int>> coords)
    {

        for (int i = 0; i < coords.Count; i++)
        {
            int tileX = coords[i].Item1;
            int tileZ = coords[i].Item2;
            GameObject obstacle = Instantiate(_obstaclePrefab, transform);
            obstacle.transform.localPosition = new Vector3(tileX, 0.5f, tileZ);
            obstacle.GetComponent<Renderer>().material.color = Random.ColorHSV(
                hueMin: 0.39f, hueMax: 0.4f,
                saturationMin: 0.5f, saturationMax: 0.6f,
                valueMin: 0.7f, valueMax: 0.8f
            );

            _obstacles.Add(obstacle);

            Tile tile = _tiles[tileX, tileZ];
            tile.IsOccupied = true;
            _tileIntMap[tileX, tileZ] = 1;
            _emptyTiles.Remove(tile);
        }
    }

    private void Start()
    {
        GenerateMap();
    }
}
