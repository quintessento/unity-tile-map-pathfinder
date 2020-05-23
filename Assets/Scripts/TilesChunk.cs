using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TilesChunk : MonoBehaviour
{
    [SerializeField]
    private Canvas _labelPrefab = null;

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    private List<Color> _colors = new List<Color>();

    private List<GameObject> _labels = new List<GameObject>();

    public void AddTile(int tileX, int tileZ, Map map, List<Tile> allTiles, Tile[,] tiles, Dictionary<MapNode, Tile> nodeToTile)
    {
        float squareSize = 1f;
        float squareHalfSize = squareSize * 0.5f;

        MapNode node = map[tileX, tileZ];

        Color tileColor = Random.ColorHSV(
            hueMin: 0.19f, hueMax: 0.2f,
            saturationMin: 0.5f, saturationMax: 0.6f,
            valueMin: 0.7f, valueMax: 0.8f
        );
        if (node.Weight > 1)
        {
            Color washedOutRed = tileColor + Color.red * 0.5f;
            tileColor = washedOutRed / node.Weight;
        }

        Canvas labelCanvas = Instantiate(_labelPrefab, transform);
        labelCanvas.transform.localPosition = new Vector3(tileX, 0.1f, tileZ);
        _labels.Add(labelCanvas.gameObject);
        Tile tile = CreateTile(tileX, tileZ, squareHalfSize, node, tileColor, labelCanvas.GetComponentInChildren<Text>());

        tiles[tileX, tileZ] = tile;
        allTiles.Add(tile);
        nodeToTile[node] = tile;
    }

    public void Apply()
    {
        _mesh = new Mesh();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.colors = _colors.ToArray();
        _mesh.RecalculateNormals();
        _meshFilter.mesh = _mesh;
        _meshCollider.sharedMesh = _mesh;
    }

    public void Clear()
    {
        _vertices.Clear();
        _triangles.Clear();
        _colors.Clear();
        _mesh = null;

        for (int i = 0; i < _labels.Count; i++)
        {
            Destroy(_labels[i]);
        }
        _labels.Clear();
    }

    private Tile CreateTile(int tileX, int tileZ, float squareHalfSize, MapNode node, Color color, Text label)
    {
        int i1 = _vertices.Count;
        int i2 = i1 + 1;
        int i3 = i1 + 2;
        int i4 = i1 + 3;
        int i5 = i1 + 4;
        int i6 = i1 + 5;

        _vertices.Add(new Vector3(tileX - squareHalfSize, 0f, tileZ - squareHalfSize));
        _vertices.Add(new Vector3(tileX - squareHalfSize, 0f, tileZ + squareHalfSize));
        _vertices.Add(new Vector3(tileX + squareHalfSize, 0f, tileZ + squareHalfSize));
        _vertices.Add(new Vector3(tileX - squareHalfSize, 0f, tileZ - squareHalfSize));
        _vertices.Add(new Vector3(tileX + squareHalfSize, 0f, tileZ + squareHalfSize));
        _vertices.Add(new Vector3(tileX + squareHalfSize, 0f, tileZ - squareHalfSize));

        _triangles.Add(i1);
        _triangles.Add(i2);
        _triangles.Add(i3);
        _triangles.Add(i4);
        _triangles.Add(i5);
        _triangles.Add(i6);

        _colors.Add(color);
        _colors.Add(color);
        _colors.Add(color);
        _colors.Add(color);
        _colors.Add(color);
        _colors.Add(color);

        return new Tile(_meshFilter, i1, i2, i3, i4, i5, i6, color, label) { Node = node };
    }

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
    }
}
