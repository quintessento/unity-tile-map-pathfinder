using UnityEngine;
using UnityEngine.UI;

public class Tile
{
    public MapNode Node { get; set; }

    private Color _originalColor;

    private MeshFilter _meshFilter;
    private int _i1, _i2, _i3, _i4, _i5, _i6;

    private Text _label = null;

    public Tile(MeshFilter meshFilter, int i1, int i2, int i3, int i4, int i5, int i6, Color color, Text label)
    {
        _meshFilter = meshFilter;
        _i1 = i1;
        _i2 = i2;
        _i3 = i3;
        _i4 = i4;
        _i5 = i5;
        _i6 = i6;
        _originalColor = color;
        _label = label;
    }

    public void SetColor(Color color)
    {
        Color[] colors = _meshFilter.mesh.colors;
        colors[_i1] = colors[_i2] = colors[_i3] =
            colors[_i4] = colors[_i5] = colors[_i6] = color;
        _meshFilter.mesh.colors = colors;
    }

    public void ResetColor()
    {
        Color[] colors = _meshFilter.mesh.colors;
        colors[_i1] = colors[_i2] = colors[_i3] =
            colors[_i4] = colors[_i5] = colors[_i6] = _originalColor;
        _meshFilter.mesh.colors = colors;
    }

    public void ShowCoordinates()
    {
        _label.gameObject.SetActive(true);
        _label.text = string.Format("({0}, {1})", Node.XIndex, Node.ZIndex);
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
        _label.resizeTextMaxSize = _label.fontSize = 60;
    }

    public void HideLabel()
    {
        _label.gameObject.SetActive(false);
    }
}
