using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class TileMap : MonoBehaviour
{
    private enum TileDebugStyle
    {
        None,
        Coords,
        Weight,
        Cost
    }

    [Header("Prefabs")]
    [SerializeField]
    private MapChunk _mapChunkPrefab = null;

    [Header("Map Settings")]
    [SerializeField]
    private int _sizeX = 10;
    [SerializeField]
    private int _sizeZ = 10;
    [SerializeField]
    private int _numObstacles = 4;
    [SerializeField]
    private bool _isWeighted = false;
    [SerializeField]
    private bool _drawDebug = false;
    [SerializeField]
    private TileDebugStyle _tileDebugStyle = TileDebugStyle.None;

    [Header("Other")]
    [SerializeField]
    private TileSelector _tileSelector = null;

    private int _tilesPerChunkX = 10, _tilesPerChunkZ = 10;
    private int _numChunksX, _numChunksZ;

    private TileDebugStyle _chosenDebugStyle = TileDebugStyle.None;

    private Tile[,] _tiles;
    private List<Tile> _allTiles = new List<Tile>();

    private ListPool<MapChunk> _chunksPool = new ListPool<MapChunk>();
    private List<MapChunk> _mapChunks = new List<MapChunk>();

    //TODO: try to get rid of
    private Dictionary<MapNode, Tile> _nodeToTile = new Dictionary<MapNode, Tile>();

    private Map _map;

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

        _sizeX = _map.SizeX;
        _sizeZ = _map.SizeZ;

        ClearMap();
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

    public void GenerateMap(bool useSettings = true)
    {
        ClearMap();

        if (useSettings)
        {
            _sizeX = _sizeZ = Settings.MapSize;
            _numObstacles = Settings.NumObstacles;
        }

        _map = new Map(_sizeX, _sizeZ, _numObstacles, _isWeighted);
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

        IPathfinder algo = PathfindersFactory.GetPathfinderForType(Settings.Pathfinder);
        yield return algo.FindPath(
            start, 
            end,
            Settings.AnimateSearch, 
            (node) => 
            { 
                if(!node.HasObstacle)
                    _nodeToTile[node].SetColor(Color.green);
            },
            (node) =>
            {
                if (!node.HasObstacle)
                    _nodeToTile[node].SetColor(Color.gray);
            }
        );
        
        List<MapNode> path = new List<MapNode>();

        MapNode current = end.CameFrom;
        while(current != null)
        {
            if (current.CameFrom == null && current != start)
            {
                path = null;
                break;
            }
            if(current != start)
                path.Add(current);
            current = current.CameFrom;
        }

        if (path != null && path.Count > 0)
        {
            for (int i = path.Count - 1; i >= 0; i--)
            {
                if (!path[i].HasObstacle)
                {
                    _nodeToTile[path[i]].SetColor(Color.white);
                    yield return new WaitForSeconds(0.05f);
                }
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

    private void ClearMap()
    {
        //reset coroutines to make sure a path is not being search for when we re-generate a map
        StopAllCoroutines();
        _tileSelector.Reset();

        for (int i = 0; i < _mapChunks.Count; i++)
        {
            _chunksPool.Add(_mapChunks[i]);
        }
        _mapChunks.Clear();

        _allTiles.Clear();
        _nodeToTile.Clear();
    }

    private void GenerateTileMap(Map map)
    {
        _tiles = new Tile[_sizeX, _sizeZ];

        _numChunksX = Mathf.CeilToInt(map.SizeX / (float)_tilesPerChunkX);
        _numChunksZ = Mathf.CeilToInt(map.SizeZ / (float)_tilesPerChunkZ);

        CreateMapChunks();
    }

    private void CreateMapChunks()
    {
        int prevChunkX = 0, prevChunkZ = 0;
        for (int x = 0; x < _numChunksX; x++)
        {
            for (int z = 0; z < _numChunksZ; z++)
            {
                MapChunk chunk = _chunksPool.Get();
                if (chunk == null)
                {
                    chunk = Instantiate(_mapChunkPrefab);
                }
                chunk.transform.SetParent(transform);

                int numTilesXInChunk = Mathf.Min(_map.SizeX, _tilesPerChunkX);
                int undividedTilesX = (x + 1) * _tilesPerChunkX;
                if (undividedTilesX > _map.SizeX)
                    numTilesXInChunk = _map.SizeX % _tilesPerChunkX;

                int numTilesZInChunk = Mathf.Min(_map.SizeZ, _tilesPerChunkZ);
                int undividedTilesZ = (z + 1) * _tilesPerChunkZ;
                if (undividedTilesZ > _map.SizeZ)
                    numTilesZInChunk = _map.SizeZ % _tilesPerChunkZ;

                chunk.GenerateMapChunk(numTilesXInChunk, numTilesZInChunk, x, z, prevChunkX, prevChunkZ, _map, _allTiles, _tiles, _nodeToTile);

                _mapChunks.Add(chunk);

                if (x == z)
                {
                    prevChunkX = numTilesXInChunk;
                    prevChunkZ = numTilesZInChunk;
                }
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

    private void OnDrawGizmos()
    {
        if (_drawDebug)
        {
            if (_tiles != null)
            {
                for (int z = 0; z < _sizeZ; z++)
                {
                    for (int x = 0; x < _sizeX; x++)
                    {
                        Tile tile = _tiles[x, z];
                        if (tile != null)
                        {
                            foreach (var n in tile.Node.ConnectedNeighbors)
                            {
                                Tile neighbor = _nodeToTile[n];

                                Vector3 start = new Vector3(tile.Node.XIndex, 0.1f, tile.Node.ZIndex);
                                Vector3 end = new Vector3(neighbor.Node.XIndex, 0.1f, neighbor.Node.ZIndex);
                                Vector3 dir = (end - start).normalized;

                                Debug.DrawLine(
                                    start + dir * 0.3f,
                                    end - dir * 0.3f,
                                    Color.red
                                    );
                            }
                        }
                    }
                }
            }
        }

        //if(_tileDebugStyle != _chosenDebugStyle)
        //{
            //_chosenDebugStyle = _tileDebugStyle;

            switch (_tileDebugStyle)
            {
                default:
                    for (int i = 0; i < _allTiles.Count; i++)
                    {
                        _allTiles[i].HideLabel();
                    }
                    break;
                case TileDebugStyle.Coords:
                    for (int i = 0; i < _allTiles.Count; i++)
                    {
                        _allTiles[i].ShowCoordinates();
                    }
                    break;
                case TileDebugStyle.Weight:
                    for (int i = 0; i < _allTiles.Count; i++)
                    {
                        _allTiles[i].ShowWeight();
                    }
                    break;
                case TileDebugStyle.Cost:
                    for (int i = 0; i < _allTiles.Count; i++)
                    {
                        _allTiles[i].ShowCost();
                    }
                    break;
            }
        //}
    }
}
