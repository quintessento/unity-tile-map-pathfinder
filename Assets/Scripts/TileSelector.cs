using UnityEngine;

public class TileSelector : MonoBehaviour
{
    private bool _isSettingStartTile = true;

    public Tile StartTile { get; private set; }
    public Tile EndTile { get; private set; }

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
                if(tile != null && !tile.IsOccupied && tile != StartTile && tile != EndTile)
                {
                    tile.SetColor(_isSettingStartTile ? Color.blue : Color.red);
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
