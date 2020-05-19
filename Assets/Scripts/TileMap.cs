using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileMap : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private Tile _tilePrefab = null;
    [SerializeField]
    private Road _roadPrefab = null;
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

    private Dictionary<IPathfindingNode, Tile> _nodeToTile = new Dictionary<IPathfindingNode, Tile>();

    public Tile GetTile(int x, int z)
    {
        if(x >= 0 && x < _tiles.GetLength(0) && z >= 0 && z < _tiles.GetLength(1))
        {
            return _tiles[x, z];
        }
        return null;
    }

    public void RandomizeStartEnd()
    {
        Debug.LogError("Not implemented");
        //_tileSelector.StartTile = _emptyTiles[Random.Range(0, _emptyTiles.Count)];
    }

    public void GenerateMap()
    {
        //reset coroutines to make sure a path is not being search for when we re-generate a map
        StopAllCoroutines();
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
        _nodeToTile.Clear();

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
                _tileIntMap[x, z] = 0;

                _allTiles.Add(tile);
                _emptyTiles.Add(tile);
            }
        }

        for (int i = 0; i < _numObstacles; i++)
        {
            PlaceObstacles();
        }

        //TODO: optimize and move to the initial loop
        for (int i = 0; i < _allTiles.Count; i++)
        {
            _allTiles[i].Neighbors = GetNeighbors(_allTiles[i]);
            //for (int j = 0; j < _allTiles[i].Neighbors.Count; j++)
            //{
            //    Road road = Instantiate(_roadPrefab, transform);
            //    road.Tile1 = _allTiles[i];
            //    road.Tile2 = _allTiles[i].Neighbors[j];
            //}
            _nodeToTile.Add(_allTiles[i].Node, _allTiles[i]);
        }
    }

    public void FindPath()
    {
        ResetHighlights();

        if (_tileSelector.StartTile != null && _tileSelector.EndTile != null)
        {
            StopAllCoroutines();
            StartCoroutine(FindPathCoroutine());
        }
    }

    private void ResetHighlights()
    {
        for (int i = 0; i < _allTiles.Count; i++)
        {
            if (_allTiles[i] == _tileSelector.StartTile || _allTiles[i] == _tileSelector.EndTile)
                continue;

            _allTiles[i].ResetColor();
        }
    }

    private IEnumerator FindPathCoroutine()
    {
        Tile start = _tileSelector.StartTile;
        Tile end = _tileSelector.EndTile;

        for (int i = 0; i < _allTiles.Count; i++)
        {
            _allTiles[i].Distance = int.MaxValue;
        }

        IPathfinder algo = PathfindersFactory.GetPathfinderForType(Settings.Pathfinder);
        IEnumerator algoCoroutine = algo.FindPath(
            start, 
            end, 
            _allTiles.Where(x => !x.IsOccupied).ToArray(), 
            Settings.AnimateSearch, 
            (node) => 
            { 
                _nodeToTile[node].SetColor(Color.green);
            },
            (node) =>
            {
                _nodeToTile[node].SetColor(Color.gray);
            }
        );
        yield return algoCoroutine;
        List<Tile> path = algoCoroutine.Current as List<Tile>;

        if (path != null && path.Count > 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                _nodeToTile[path[i].Node].SetColor(Color.white);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            //notify the player
            Debug.Log("Could not find a path");
        }

        yield return null;
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    //it's the current tile -> skip
                    continue;
                }

                if (Mathf.Abs(i) == Mathf.Abs(j))
                {
                    //we are going in diagonal -> skip
                    continue;
                }

                Tile neighbor = GetTile(tile.x + i, tile.z + j);
                if (neighbor == null)
                {
                    //null -> skip
                    continue;
                }

                if (neighbor.Node.IsOccupied)
                {
                    //has an obstacle -> skip
                    continue;
                }

                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private void PlaceObstacles()
    {
        int[,] obstacleBlueprint = Obstacles.RandomDeclared;

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
            tile.Node.IsOccupied = true;
            _tileIntMap[tileX, tileZ] = 1;
            _emptyTiles.Remove(tile);
        }
    }

    private void Start()
    {
        GenerateMap();

        _tileSelector.StartTileSelected += OnStartTileSelected;
        _tileSelector.EndTileSelected += OnEndTileSelected;
    }

    private void OnStartTileSelected(object sender, EventArgs e)
    {
        //stop pathfinding
        StopAllCoroutines();
        ResetHighlights();

        //for (int x = 0; x < _sizeX; x++)
        //{
        //    for (int z = 0; z < _sizeZ; z++)
        //    {
        //        Tile tile = _tiles[x, z];
        //        tile.Distance = DistanceFromTo(_tileSelector.StartTile, tile);
        //    }
        //}

        //_tileSelector.StartTile.Distance = 0;
    }

    private void OnEndTileSelected(object sender, EventArgs e)
    {
        //stop pathfinding
        StopAllCoroutines();
        ResetHighlights();
    }

    private int DistanceFromTo(Tile from, Tile to)
    {
        return 
            ((from.x > to.x ? from.x - to.x : to.x - from.x) +
            (from.z > to.z ? from.z - to.z : to.z - from.z));
    }
}
