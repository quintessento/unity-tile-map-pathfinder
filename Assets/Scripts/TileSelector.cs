using System;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    private bool _isSettingStartTile = true;

    private Tile _startTile, _endTile;

    public event EventHandler StartTileSelected;
    public event EventHandler EndTileSelected;

    public Tile StartTile 
    {
        get => _startTile;
        private set
        {
            _startTile = value;
            
            if (value != null)
            {
                value.SetColor(Color.blue);
                StartTileSelected?.Invoke(this, null);
            }
        }
    }

    public Tile EndTile
    {
        get => _endTile;
        private set
        {
            _endTile = value;
           
            if (value != null)
            {
                value.SetColor(Color.red);
                EndTileSelected?.Invoke(this, null);
            }
        }
    }

    public void Reset()
    {
        StartTile = null;
        EndTile = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = CameraController.Main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if(tile != null && !tile.Node.IsOccupied && tile != _startTile && tile != _endTile)
                {
                    if (_isSettingStartTile)
                    {
                        StartTile?.ResetColor();
                        StartTile = tile;
                    }
                    else
                    {
                        EndTile?.ResetColor();
                        EndTile = tile;
                    }

                    _isSettingStartTile = !_isSettingStartTile;
                }
            }
        }
    }
}
