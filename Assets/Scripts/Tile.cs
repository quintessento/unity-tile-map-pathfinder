using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Renderer _renderer = null;

    public int x, z;
    public int pathfindingWeight;

    private Color _originalColor;
    
    public bool IsOccupied { get; set; }

    public void Initialize(Vector3 localPosition, Color color)
    {
        transform.localPosition = localPosition;
        SetColor(color);
        _originalColor = color;
    }

    public void SetColor(Color color)
    {
        _renderer.material.color = color;
    }

    public void ResetColor()
    {
        _renderer.material.color = _originalColor;
    }
}
