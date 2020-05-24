using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileMap : MonoBehaviour
{
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

    private int _tilesPerChunkX = 10, _tilesPerChunkZ = 10;
    private int _numChunksX, _numChunksZ;

    private Tile[,] _tiles;
    private List<Tile> _allTiles = new List<Tile>();

    private ListPool<MapChunk> _chunksPool = new ListPool<MapChunk>();
    private List<MapChunk> _mapChunks = new List<MapChunk>();

    private Tile _startTile, _endTile;

    private Dictionary<MapNode, Tile> _nodeToTile = new Dictionary<MapNode, Tile>();

    private Map _map;

    private string _saveFilePath;

    public Tile StartTile
    {
        get => _startTile;
        set
        {
            _startTile = value;

            if (value != null)
            {
                value.SetColor(Color.blue);
            }

            StopAllCoroutines();
            ResetHighlights();
        }
    }

    public Tile EndTile
    {
        get => _endTile;
        set
        {
            _endTile = value;

            if (value != null)
            {
                value.SetColor(Color.red);
            }

            StopAllCoroutines();
            ResetHighlights();
        }
    }

    public void Save()
    {
        if (_map != null)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(_saveFilePath, FileMode.Create)))
            {
                writer.Write(Settings.NumObstacles);
                _map.Save(writer);
                MessagePanel.ShowMessage("Saved map to " + _saveFilePath);
            }
        }
    }

    public void Load()
    {
        using (BinaryReader reader = new BinaryReader(File.Open(_saveFilePath, FileMode.Open))) 
        {
            _numObstacles = reader.ReadInt32();
            _map = new Map(reader);

            ClearMap();

            _sizeX = _map.SizeX;
            _sizeZ = _map.SizeZ;
            _numObstacles = Settings.NumObstacles;

            GenerateTileMap(_map);
        }
    }

    public Tile GetTile(float x, float z)
    {
        int xIndex = (int)(x / 1f + 1f * 0.5f);
        int zIndex = (int)(z / 1f + 1f * 0.5f);
        return _tiles[xIndex, zIndex];
    }

    public void RandomizeStartEnd()
    {
        if (_map != null)
        {
            _map.ReturnEmptyNode(StartTile?.Node);
            _map.ReturnEmptyNode(EndTile?.Node);

            MapNode startNode = _map.PopRandomEmptyNode();
            MapNode endNode = _map.PopRandomEmptyNode();

            if (startNode != null && endNode != null)
            {
                StartTile = _nodeToTile[startNode];
                EndTile = _nodeToTile[endNode];
            }
        }
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

        if (StartTile != null && EndTile != null)
        {
            StopAllCoroutines();
            StartCoroutine(FindPathCoroutine());
        }
    }

    private IEnumerator FindPathCoroutine()
    {
        MapNode start = StartTile.Node;
        MapNode end = EndTile.Node;

        IPathfinder algo = PathfindersFactory.GetPathfinderForType(Settings.Pathfinder);
        yield return algo.FindPath(
            start, 
            end,
            Settings.AnimateSearch, 
            (node) => 
            {
                if (!node.HasObstacle)
                {
                    Tile tile = _nodeToTile[node];
                    tile.SetColor(Color.green);
                    if (_tileDebugStyle == TileDebugStyle.Cost)
                        tile.ShowCost();
                }
            },
            (node) =>
            {
                if (!node.HasObstacle)
                {
                    Tile tile = _nodeToTile[node];
                    tile.SetColor(Color.gray);
                    if (_tileDebugStyle == TileDebugStyle.Cost)
                        tile.ShowCost();
                }
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
            string message = "Could not find a path";
            Debug.Log(message);
            MessagePanel.ShowMessage(message);
        }

        yield return null;
    }

    private void ResetHighlights()
    {
        for (int i = 0; i < _allTiles.Count; i++)
        {
            if (_allTiles[i] == StartTile || _allTiles[i] == EndTile)
                continue;

            _allTiles[i].ResetColor();
        }
    }

    private void ClearMap()
    {
        //reset coroutines to make sure a path is not being search for when we re-generate a map
        StopAllCoroutines();
        StartTile = null;
        EndTile = null;

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

    private void Awake()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, "save.sav");
    }

    private void Start()
    {
        Settings.SettingsChanged += OnSettingsChanged;
        _isWeighted = Settings.IsWeighted;
    }

    private void OnSettingsChanged(object sender, EventArgs e)
    {
        _numObstacles = Settings.NumObstacles;
        _isWeighted = Settings.IsWeighted;

        if (_tileDebugStyle != Settings.TileDebugStyle)
        {
            _tileDebugStyle = Settings.TileDebugStyle;

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
        }
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

                                Vector3 start = new Vector3(tile.Node.X, 0.1f, tile.Node.Z);
                                Vector3 end = new Vector3(neighbor.Node.X, 0.1f, neighbor.Node.Z);
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
    }
}
