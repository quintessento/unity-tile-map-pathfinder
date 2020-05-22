using UnityEngine;
using UnityEngine.UI;

public class Tile
{
    [SerializeField]
    private Renderer _renderer = null;
    [SerializeField]
    private Text _label = null;

    //public int x, z;

    public MapNode Node { get; set; }

    private Color _originalColor;

    private Mesh _mesh;
    private Color[] _colors;
    private int _i1, _i2, _i3, _i4, _i5, _i6;

    private static MaterialPropertyBlock _propertyBlock;

    public Tile(Mesh mesh, ref Color[] colors, int i1, int i2, int i3, int i4, int i5, int i6, Color color)
    {
        _mesh = mesh;
        _colors = colors;
        _i1 = i1;
        _i2 = i2;
        _i3 = i3;
        _i4 = i4;
        _i5 = i5;
        _i6 = i6;
        _originalColor = color;
    }

    //public void Initialize(Vector3 localPosition, Color color)
    //{
    //    transform.localPosition = localPosition;
    //    SetColor(color);
    //    _originalColor = color;
    //    HideLabel();
    //}

    //public void SetColor(Color color)
    //{
    //    //_renderer.material.color = color;

    //    if(_propertyBlock == null)
    //        _propertyBlock = new MaterialPropertyBlock();
    //    _propertyBlock.SetColor("_Color", color);
    //    _renderer.SetPropertyBlock(_propertyBlock);
    //}

    //public void ResetColor()
    //{
    //    //_renderer.material.color = _originalColor;

    //    if (_propertyBlock == null)
    //        _propertyBlock = new MaterialPropertyBlock();
    //    _propertyBlock.SetColor("_Color", _originalColor);
    //    _renderer.SetPropertyBlock(_propertyBlock);
    //}

    public void SetColor(Color color)
    {
        _colors[_i1] = _colors[_i2] = _colors[_i3] =
            _colors[_i4] = _colors[_i5] = _colors[_i6] = color;
        _mesh.colors = _colors;
    }

    public void ResetColor()
    {
        _colors[_i1] = _colors[_i2] = _colors[_i3] =
            _colors[_i4] = _colors[_i5] = _colors[_i6] = _originalColor;
        _mesh.colors = _colors;
    }

    //public void ShowCoordinates()
    //{
    //    _label.gameObject.SetActive(true);
    //    _label.text = string.Format("({0}, {1})", x, z);
    //}

    //public void ShowWeight()
    //{
    //    _label.gameObject.SetActive(true);
    //    _label.text = pathfindingWeight.ToString();
    //}

    //public void ShowDistance()
    //{
    //    _label.gameObject.SetActive(true);
    //    _label.text = _distance.ToString();
    //}

    public void HideLabel()
    {
        _label.gameObject.SetActive(false);
    }

}
