using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelector : MonoBehaviour
{

    //public event EventHandler StartTileSelected;
    //public event EventHandler EndTileSelected;

    //public void Reset()
    //{
    //    StartTile = null;
    //    EndTile = null;
    //}

    private bool _isSettingStartTile = true;

    private TileMap _tileMap;

    private void Awake()
    {
        _tileMap = GetComponent<TileMap>();
    }

    private void Update()
    {
        //make sure we aren't over UI to prevent accidental board placement while interacting with the UI
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = CameraController.Main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 hitPosition = transform.InverseTransformPoint(hit.point);
                Tile tile = GetComponent<TileMap>().GetTile(hitPosition.x, hitPosition.z);

                if (tile != null && !tile.Node.HasObstacle && tile != _tileMap.StartTile && tile != _tileMap.StartTile)
                {
                    if (_isSettingStartTile)
                    {
                        _tileMap.StartTile?.ResetColor();
                        _tileMap.StartTile = tile;
                    }
                    else
                    {
                        _tileMap.EndTile?.ResetColor();
                        _tileMap.EndTile = tile;
                    }

                    _isSettingStartTile = !_isSettingStartTile;
                }
            }
        }
    }
}
