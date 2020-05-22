using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TileChunk : MonoBehaviour
{
    [SerializeField]
    private Obstacle _obstaclePrefab = null;

    private ListPool<Obstacle> _obstaclesPool = new ListPool<Obstacle>();
    private List<Obstacle> _obstacles = new List<Obstacle>();

    private Color[] _colors;

    public void GenerateTiles(int sizeX, int sizeZ, int chunkX, int chunkZ, Map map, List<Tile> allTiles, Tile[,] tiles, Dictionary<MapNode, Tile> nodeToTile)
    {
        for (int i = 0; i < _obstacles.Count; i++)
        {
            _obstaclesPool.Add(_obstacles[i]);
        }
        _obstacles.Clear();

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        _colors = new Color[sizeX * sizeZ * 6];

        float squareSize = 1f;
        float squareHalfSize = squareSize * 0.5f;

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                int tileX = chunkX * 10 + x;
                int tileZ = chunkZ * 10 + z;
                MapNode node = map[tileX, tileZ];

                int i1 = vertices.Count;
                int i2 = i1 + 1;
                int i3 = i1 + 2;
                int i4 = i1 + 3;
                int i5 = i1 + 4;
                int i6 = i1 + 5;

                vertices.Add(new Vector3(tileX - squareHalfSize, 0f, tileZ - squareHalfSize));
                vertices.Add(new Vector3(tileX - squareHalfSize, 0f, tileZ + squareHalfSize));
                vertices.Add(new Vector3(tileX + squareHalfSize, 0f, tileZ + squareHalfSize));
                triangles.Add(i1);
                triangles.Add(i2);
                triangles.Add(i3);

                vertices.Add(new Vector3(tileX - squareHalfSize, 0f, tileZ - squareHalfSize));
                vertices.Add(new Vector3(tileX + squareHalfSize, 0f, tileZ + squareHalfSize));
                vertices.Add(new Vector3(tileX + squareHalfSize, 0f, tileZ - squareHalfSize));
                triangles.Add(i4);
                triangles.Add(i5);
                triangles.Add(i6);

                Color tileColor = Random.ColorHSV(0.19f, 0.2f, 0.5f, 0.6f, 0.7f, 0.8f);
                _colors[i1] = tileColor;
                _colors[i2] = tileColor;
                _colors[i3] = tileColor;
                _colors[i4] = tileColor;
                _colors[i5] = tileColor;
                _colors[i6] = tileColor;

                Tile tile = new Tile(mesh, ref _colors, i1, i2, i3, i4, i5, i6, tileColor) { Node = node };

                tiles[tileX, tileZ] = tile;
                allTiles.Add(tile);
                nodeToTile[node] = tile;

                if (node.HasObstacle)
                {
                    PlaceObstacle(tileX, tileZ);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = _colors;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void AddTileToMesh()
    {

    }

    private void PlaceObstacle(int x, int z)
    {
        int tileX = x;
        int tileZ = z;

        Obstacle obstacle = _obstaclesPool.Get();
        if (obstacle == null)
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
}
