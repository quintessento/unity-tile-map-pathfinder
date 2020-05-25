using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TileMap : MonoBehaviour
{
    /// <summary>
    /// Sets random start and end tiles for pathfinding.
    /// </summary>
    public void RandomizeStartEnd()
    {
        if (Map != null)
        {
            Map.ReturnEmptyNode(StartTile?.Node);
            Map.ReturnEmptyNode(EndTile?.Node);

            MapNode startNode = Map.PopRandomEmptyNode();
            MapNode endNode = Map.PopRandomEmptyNode();

            if (startNode != null && endNode != null)
            {
                StartTile = _nodeToTile[startNode];
                EndTile = _nodeToTile[endNode];
            }
        }
    }

    /// <summary>
    /// Activates the search for a path on the tile map, as long as start and end tiles are set.
    /// </summary>
    public void FindPath()
    {
        ResetHighlights();

        if (StartTile != null && EndTile != null)
        {
            StopAllCoroutines();
            StartCoroutine(FindPathCoroutine());
        }
    }

    private IEnumerator FindPathCoroutine()
    {
        MapNode start = StartTile.Node;
        MapNode end = EndTile.Node;

        IPathfinder algo = PathfindersFactory.GetPathfinderForType(Settings.Pathfinder);
        yield return algo.FindPath(
            start,
            end,
            Settings.AnimateSearch,
            (node) =>
            {
                if (!node.HasObstacle)
                {
                    Tile tile = _nodeToTile[node];
                    tile.SetColor(Color.green);
                    if (_tileDebugStyle == TileDebugStyle.Cost)
                        tile.ShowCost();
                }
            },
            (node) =>
            {
                if (!node.HasObstacle)
                {
                    Tile tile = _nodeToTile[node];
                    tile.SetColor(Color.gray);
                    if (_tileDebugStyle == TileDebugStyle.Cost)
                        tile.ShowCost();
                }
            }
        );

        List<MapNode> path = new List<MapNode>();

        MapNode current = end.CameFrom;
        while (current != null)
        {
            if (current.CameFrom == null && current != start)
            {
                path = null;
                break;
            }
            if (current != start)
                path.Add(current);
            current = current.CameFrom;
        }

        if (path != null && path.Count > 0)
        {
            for (int i = path.Count - 1; i >= 0; i--)
            {
                if (!path[i].HasObstacle)
                {
                    _nodeToTile[path[i]].SetColor(Color.white);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
        else
        {
            //notify the player
            string message = "Could not find a path";
            Debug.Log(message);
            MessagePanel.ShowMessage(message);
        }

        yield return null;
    }

}
