using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TileMap : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private Tile _tilePrefab = null;
    [SerializeField]
    private Road _roadPrefab = null;
    [SerializeField]
    private Obstacle _obstaclePrefab = null;

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

    private List<Tile> _allTiles = new List<Tile>();

    private ListPool<Obstacle> _obstaclesPool = new ListPool<Obstacle>();
    private List<Obstacle> _obstacles = new List<Obstacle>();

    //TODO: try to get rid of
    private Dictionary<MapNode, Tile> _nodeToTile = new Dictionary<MapNode, Tile>();

    private Map _map;
    private Color[] _colors;

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

        _allTiles.Clear();

        for (int i = 0; i < _obstacles.Count; i++)
        {
            _obstaclesPool.Add(_obstacles[i]);
        }
        _obstacles.Clear();

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

    private void GenerateTileMap(Map map)
    {
        _tiles = new Tile[_sizeX, _sizeZ];

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        _colors = new Color[map.SizeX * map.SizeZ * 6];

        float squareSize = 1f;
        float squareHalfSize = squareSize * 0.5f;

        for (int x = 0; x < map.SizeX; x++)
        {
            for (int z = 0; z < map.SizeZ; z++)
            {
                MapNode node = map[x, z];

                //int i1 = vertices.Count;
                //int i2 = i1 + 1;
                //int i3 = i1 + 2;
                //int i4 = i1 + 3;
                //int i5 = i1 + 4;
                //int i6 = i1 + 5;

                //vertices.Add(new Vector3(x - squareHalfSize, 0f, z - squareHalfSize));
                //vertices.Add(new Vector3(x - squareHalfSize, 0f, z + squareHalfSize));
                //vertices.Add(new Vector3(x + squareHalfSize, 0f, z + squareHalfSize));
                //triangles.Add(i1);
                //triangles.Add(i2);
                //triangles.Add(i3);

                //vertices.Add(new Vector3(x - squareHalfSize, 0f, z - squareHalfSize));
                //vertices.Add(new Vector3(x + squareHalfSize, 0f, z + squareHalfSize));
                //vertices.Add(new Vector3(x + squareHalfSize, 0f, z - squareHalfSize));
                //triangles.Add(i4);
                //triangles.Add(i5);
                //triangles.Add(i6);

                Color tileColor = Random.ColorHSV(0.19f, 0.2f, 0.5f, 0.6f, 0.7f, 0.8f);
                //_colors[i1] = tileColor;
                //_colors[i2] = tileColor;
                //_colors[i3] = tileColor;
                //_colors[i4] = tileColor;
                //_colors[i5] = tileColor;
                //_colors[i6] = tileColor;

                //Tile tile = new Tile(mesh, ref _colors, i1, i2, i3, i4, i5, i6, tileColor);

                Tile tile = Instantiate(_tilePrefab, transform);
                tile.Initialize(new Vector3(x, 0f, z), tileColor);
                _tiles[x, z] = tile;
                tile.Node = node;

                _allTiles.Add(tile);

                if (node.HasObstacle)
                {
                    PlaceObstacle(x, z);
                }

                _nodeToTile[node] = tile;
            }
        }

        //mesh.vertices = vertices.ToArray();
        //mesh.triangles = triangles.ToArray();
        //mesh.colors = _colors;
        //mesh.RecalculateNormals();
        //GetComponent<MeshFilter>().mesh = mesh;
        //GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void PlaceObstacle(int x, int z)
    {
        int tileX = x;
        int tileZ = z;

        Obstacle obstacle = _obstaclesPool.Get();
        if(obstacle == null)
        {
            obstacle = Instantiate(_obstaclePrefab);
        }
        obstacle.transform.SetParent(transform);

        obstacle.transform.localPosition = new Vector3(tileX, 0.5f, tileZ);
        obstacle.GetComponent<Renderer>().material.color = Random.ColorHSV(
            hueMin: 0.39f, hueMax: 0.4f,
            saturationMin: 0.5f, saturationMax: 0.6f,
            valueMin: 0.7f, valueMax: 0.8f
        );

        _obstacles.Add(obstacle);
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

    private void OnValidate()
    {
        
    }
}
