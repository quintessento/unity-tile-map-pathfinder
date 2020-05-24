using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual representation of a Map Node.
/// </summary>
public class Tile
{
    public MapNode Node { get; private set; }

    private Color _originalColor;

    private readonly MeshFilter _meshFilter;
    private readonly int[] _indices;

    private readonly Text _label = null;

    /// <summary>
    /// Creates a tile based on a node.
    /// </summary>
    /// <param name="node">The model of the tile.</param>
    /// <param name="meshFilter">Parent mesh's filter.</param>
    /// <param name="color">Initial color.</param>
    /// <param name="label">Label object.</param>
    /// <param name="indices">Set of vertex indices for vertices on the mesh.</param>
    public Tile(MapNode node, MeshFilter meshFilter, Color color, Text label, params int[] indices)
    {
        Node = node;
        _meshFilter = meshFilter;

        _originalColor = color;
        _label = label;

        _indices = new int[indices.Length];
        for (int i = 0; i < indices.Length; i++)
        {
            _indices[i] = indices[i];
        }

        switch (Settings.TileDebugStyle)
        {
            default:
                HideLabel();
                break;
            case TileDebugStyle.Coords:
                ShowCoordinates();
                break;
            case TileDebugStyle.Weight:
                ShowWeight();
                break;
            case TileDebugStyle.Cost:
                ShowCost();
                break;
        }
    }

    public void SetColor(Color color)
    {
        Color[] colors = _meshFilter.mesh.colors;
        for (int i = 0; i < _indices.Length; i++)
        {
            colors[_indices[i]] = color;
        }
        _meshFilter.mesh.colors = colors;
    }

    public void ResetColor()
    {
        Color[] colors = _meshFilter.mesh.colors;
        for (int i = 0; i < _indices.Length; i++)
        {
            colors[_indices[i]] = _originalColor;
        }
        _meshFilter.mesh.colors = colors;
    }

    public void ShowCoordinates()
    {
        _label.gameObject.SetActive(true);
        _label.text = string.Format("({0}, {1})", Node.X, Node.Z);
        _label.resizeTextMaxSize = _label.fontSize = 20;
    }

    public void ShowWeight()
    {
        _label.gameObject.SetActive(true);
        _label.text = Node.Weight.ToString();
        _label.resizeTextMaxSize = _label.fontSize = 60;
    }

    public void ShowCost()
    {
        _label.gameObject.SetActive(true);
        _label.text = Node.Cost.ToString();
        _label.resizeTextMaxSize = _label.fontSize = 40;
    }

    public void HideLabel()
    {
        _label.gameObject.SetActive(false);
    }
}
