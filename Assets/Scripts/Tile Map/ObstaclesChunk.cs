using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ObstaclesChunk : MonoBehaviour
{
    private Mesh _mesh;
    private MeshFilter _meshFilter;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    private List<Color> _colors = new List<Color>();

    public void AddObstacle(int tileX, int tileZ)
    {
        float squareSize = 1f;
        float squareHalfSize = squareSize * 0.5f;

        Color obstacleColor = Random.ColorHSV(
            hueMin: 0.39f, hueMax: 0.4f,
            saturationMin: 0.5f, saturationMax: 0.6f,
            valueMin: 0.7f, valueMax: 0.8f
        );
        Obstacle obstacle = CreateObstacleCube(tileX, tileZ, squareHalfSize, obstacleColor);
    }

    public void Apply()
    {
        _mesh = new Mesh();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.colors = _colors.ToArray();
        _mesh.RecalculateNormals();
        _meshFilter.mesh = _mesh;
    }

    public void Clear()
    {
        _vertices.Clear();
        _triangles.Clear();
        _colors.Clear();
        _mesh = null;
    }

    private Obstacle CreateObstacleCube(int tileX, int tileZ, float squareHalfSize, Color color)
    {
        //p2-p3
        //|  |
        //p1-p4

        //south face
        AddSquare(
            new Vector3(tileX - squareHalfSize, 0f, tileZ - squareHalfSize),
            new Vector3(tileX - squareHalfSize, squareHalfSize * 2f, tileZ - squareHalfSize),
            new Vector3(tileX + squareHalfSize, squareHalfSize * 2f, tileZ - squareHalfSize),
            new Vector3(tileX + squareHalfSize, 0f, tileZ - squareHalfSize),
            color
        );

        //north face
        AddSquare(
            new Vector3(tileX + squareHalfSize, 0f, tileZ + squareHalfSize),
            new Vector3(tileX + squareHalfSize, squareHalfSize * 2f, tileZ + squareHalfSize),
            new Vector3(tileX - squareHalfSize, squareHalfSize * 2f, tileZ + squareHalfSize),
            new Vector3(tileX - squareHalfSize, 0f, tileZ + squareHalfSize),
            color
        );

        //west face
        AddSquare(
            new Vector3(tileX - squareHalfSize, 0f, tileZ + squareHalfSize),
            new Vector3(tileX - squareHalfSize, squareHalfSize * 2f, tileZ + squareHalfSize),
            new Vector3(tileX - squareHalfSize, squareHalfSize * 2f, tileZ - squareHalfSize),
            new Vector3(tileX - squareHalfSize, 0f, tileZ - squareHalfSize),
            color
        );

        //east face
        AddSquare(
            new Vector3(tileX + squareHalfSize, 0f, tileZ - squareHalfSize),
            new Vector3(tileX + squareHalfSize, squareHalfSize * 2f, tileZ - squareHalfSize),
            new Vector3(tileX + squareHalfSize, squareHalfSize * 2f, tileZ + squareHalfSize),
            new Vector3(tileX + squareHalfSize, 0f, tileZ + squareHalfSize),
            color
        );

        //top face
        AddSquare(
           new Vector3(tileX - squareHalfSize, squareHalfSize * 2f, tileZ - squareHalfSize),
           new Vector3(tileX - squareHalfSize, squareHalfSize * 2f, tileZ + squareHalfSize),
           new Vector3(tileX + squareHalfSize, squareHalfSize * 2f, tileZ + squareHalfSize),
           new Vector3(tileX + squareHalfSize, squareHalfSize * 2f, tileZ - squareHalfSize),
           color
       );

        return new Obstacle();
    }

    private void AddSquare(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color)
    {
        int i1 = _vertices.Count;
        int i2 = i1 + 1;
        int i3 = i1 + 2;
        int i4 = i1 + 3;
        int i5 = i1 + 4;
        int i6 = i1 + 5;

        _vertices.Add(p1);
        _vertices.Add(p2);
        _vertices.Add(p3);
        _vertices.Add(p1);
        _vertices.Add(p3);
        _vertices.Add(p4);

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
    }

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }
}
