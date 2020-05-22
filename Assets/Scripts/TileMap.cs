using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileMap : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private TileChunk _tileChunkPrefab = null;

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

    private int _tilesPerChunkX = 10, _tilesPerChunkZ = 10;
    private int _numChunksX, _numChunksZ;

    //private ListPool<Tile> _tilesPool = new ListPool<Tile>();
    private Tile[,] _tiles;
    private List<Tile> _allTiles = new List<Tile>();

    //private ListPool<Obstacle> _obstaclesPool = new ListPool<Obstacle>();
    //private List<Obstacle> _obstacles = new List<Obstacle>();

    private ListPool<TileChunk> _chunksPool = new ListPool<TileChunk>();
    private List<TileChunk> _tileChunks = new List<TileChunk>();

    //TODO: try to get rid of
    private Dictionary<MapNode, Tile> _nodeToTile = new Dictionary<MapNode, Tile>();

    private Map _map;
    //private Color[] _colors;

    private string _saveFilePath;

    private void Awake()
    {
        _saveFilePath = Path.Combine(Application.dataPath, "save.sav");
    }

    public void Save()
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(_saveFilePath, FileMode.Create))) 
        {
            _map.Save(writer);
        }
    }

    public void Load()
    {
        using (BinaryReader reader = new BinaryReader(File.Open(_saveFilePath, FileMode.Open))) 
        {
            _map = new Map(reader);
        }

        GenerateTileMap(_map);
    }

    public Tile GetTile(float x, float z)
    {
        int xIndex = (int)(x / 1f + 1f * 0.5f);
        int zIndex = (int)(z / 1f + 1f * 0.5f);
        return _tiles[xIndex, zIndex];
        //Debug.Log(xIndex + ", " + zIndex);
        //int index = zIndex + _map.SizeX * xIndex;
        //return _allTiles[index];
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

        for (int i = 0; i < _tileChunks.Count; i++)
        {
            _chunksPool.Add(_tileChunks[i]);
        }
        _tileChunks.Clear();

        _allTiles.Clear();
        _nodeToTile.Clear();

        _sizeX = _sizeZ = Settings.MapSize;
        _numObstacles = Settings.NumObstacles;

        _map = new Map(_sizeX, _sizeZ, _numObstacles);
        GenerateTileMap(_map);
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

    private IEnumerator FindPathCoroutine()
    {
        MapNode start = _tileSelector.StartTile.Node;
        MapNode end = _tileSelector.EndTile.Node;

        for (int i = 0; i < _allTiles.Count; i++)
        {
            _allTiles[i].Node.Distance = int.MaxValue;
        }

        IPathfinder algo = PathfindersFactory.GetPathfinderForType(Settings.Pathfinder);
        IEnumerator algoCoroutine = algo.FindPath(
            start, 
            end,
            _map.AsArray.Where(x => !x.HasObstacle).ToArray(), 
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
        List<MapNode> path = algoCoroutine.Current as List<MapNode>;

        if (path != null && path.Count > 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                _nodeToTile[path[i]].SetColor(Color.white);
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

    private void ResetHighlights()
    {
        for (int i = 0; i < _allTiles.Count; i++)
        {
            if (_allTiles[i] == _tileSelector.StartTile || _allTiles[i] == _tileSelector.EndTile)
                continue;

            _allTiles[i].ResetColor();
        }
    }


    private void GenerateTileMap(Map map)
    {
        _tiles = new Tile[_sizeX, _sizeZ];

        _numChunksX = map.SizeX / _tilesPerChunkX;
        _numChunksZ = map.SizeZ / _tilesPerChunkZ;

        CreateTileChunks();
    }

    private void CreateTileChunks()
    {
        for (int x = 0; x < _numChunksX; x++)
        {
            for (int z = 0; z < _numChunksZ; z++)
            {
                TileChunk chunk = _chunksPool.Get();
                if (chunk == null)
                {
                    chunk = Instantiate(_tileChunkPrefab);
                }
                chunk.transform.SetParent(transform);

                int numTilesXInChunk = 10;
                int numTilesZInChunk = 10;
                chunk.GenerateTiles(numTilesXInChunk, numTilesZInChunk, x, z, _map, _allTiles, _tiles, _nodeToTile);

                _tileChunks.Add(chunk);
            }
        }
    }

    private void Start()
    {
        //GenerateMap();

        _tileSelector.StartTileSelected += OnStartTileSelected;
        _tileSelector.EndTileSelected += OnEndTileSelected;
    }

    private void OnStartTileSelected(object sender, EventArgs e)
    {
        //stop pathfinding
        StopAllCoroutines();
        ResetHighlights();
    }

    private void OnEndTileSelected(object sender, EventArgs e)
    {
        //stop pathfinding
        StopAllCoroutines();
        ResetHighlights();
    }
}
