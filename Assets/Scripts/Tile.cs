using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Renderer _renderer = null;
    [SerializeField]
    private Text _label = null;

    public int x, z;
    public int pathfindingWeight;
    public List<Tile> path;
    public List<Tile> neighbors;

    private Color _originalColor;
    private bool _isOccupied;
    private int _distance;

    public bool IsOccupied 
    {
        get => _isOccupied;
        set 
        {
            _isOccupied = value;

            if (value)
            {
                HideLabel();
            }
        } 
    }

    public int Distance
    {
        get => _distance;
        set
        {
            _distance = value;

            if (_distance != int.MaxValue)
                ShowDistance();
            else
                HideLabel();
        }
    }

    public void Initialize(Vector3 localPosition, Color color)
    {
        transform.localPosition = localPosition;
        SetColor(color);
        _originalColor = color;
        HideLabel();

        neighbors = new List<Tile>();
    }

    public void SetColor(Color color)
    {
        _renderer.material.color = color;
    }

    public void ResetColor()
    {
        _renderer.material.color = _originalColor;
    }

    public void ShowCoordinates()
    {
        _label.gameObject.SetActive(true);
        _label.text = string.Format("({0}, {1})", x, z);
    }

    public void ShowWeight()
    {
        _label.gameObject.SetActive(true);
        _label.text = pathfindingWeight.ToString();
    }

    public void ShowDistance()
    {
        _label.gameObject.SetActive(true);
        _label.text = _distance.ToString();
    }

    public void HideLabel()
    {
        _label.gameObject.SetActive(false);
    }
}
