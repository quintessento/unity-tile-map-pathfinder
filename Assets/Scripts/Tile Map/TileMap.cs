using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main entry point to the app. Generates the tile map and initiates pathfinding on the map.
/// </summary>
public partial class TileMap : MonoBehaviour
{
    private const int _tilesPerChunkX = 10, _tilesPerChunkZ = 10;

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

    private int _numChunksX, _numChunksZ;

    //general-purpose 2D array of tiles
    private Tile[,] _tiles;
    //one-dimensional list of tiles for ease of use, where applicable
    private readonly List<Tile> _allTiles = new List<Tile>();

    private readonly ListPool<MapChunk> _chunksPool = new ListPool<MapChunk>();
    private readonly List<MapChunk> _mapChunks = new List<MapChunk>();

    private Tile _startTile, _endTile;

    //mapping of model to view for reverse access
    private readonly Dictionary<MapNode, Tile> _nodeToTile = new Dictionary<MapNode, Tile>();

    public Map Map { get; private set; }

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

    public void Load(Map map)
    {
        Map = map;

        ClearMap();

        _sizeX = Map.SizeX;
        _sizeZ = Map.SizeZ;
        _numObstacles = Settings.NumObstacles;

        GenerateTileMap(Map);
    }

    /// <summary>
    /// Gets a tile from the tile map using a world x and z position.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="z">Z coordinate.</param>
    /// <returns>Tile if found.</returns>
    public Tile GetTile(float x, float z)
    {
        int xIndex = (int)(x / 1f + 1f * 0.5f);
        int zIndex = (int)(z / 1f + 1f * 0.5f);
        return _tiles[xIndex, zIndex];
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

    private void Start()
    {
        Settings.SettingsChanged += OnSettingsChanged;
        _isWeighted = Settings.IsWeighted;
    }

    private void OnApplicationQuit()
    {
        Settings.SettingsChanged -= OnSettingsChanged;
    }

    private void OnSettingsChanged(object sender, EventArgs e)
    {
        _numObstacles = Settings.NumObstacles;
        _isWeighted = Settings.IsWeighted;

        if (_tileDebugStyle != Settings.TileDebugStyle)
        {
            _tileDebugStyle = Settings.TileDebugStyle;

            //enable/disable labels for all tiles
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
        //drawing of debug connections (edges) between neighboring tiles
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
