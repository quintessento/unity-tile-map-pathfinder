using Custom.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : IPathfinder
{
    public IEnumerator FindPath(MapNode start, MapNode end, bool animateSearch = false, Action<MapNode> processingAction = null, Action<MapNode> processedAction = null)
    {
        PriorityQueue<MapNode> unexplored = new PriorityQueue<MapNode>();
        HashSet<MapNode> explored = new HashSet<MapNode>();

        start.CameFrom = null;
        start.Cost = 0;
        unexplored.Enqueue(start, start.Cost);

        //processing
        while (unexplored.Count > 0)
        {
            MapNode current = unexplored.Dequeue();
            explored.Add(current);

            if (current == end)
            {
                //finished
                yield break;
            }

            for (int i = 0; i < current.ConnectedNeighbors.Count; i++)
            {
                MapNode neighbor = current.ConnectedNeighbors[i];
                if (explored.Contains(neighbor))
                    continue;

                //distance from starting node
                int gCost = neighbor.Weight * 10 + GetDistance(current, start);
                int fCost = gCost;

                if (fCost < neighbor.Cost || !unexplored.Contains(neighbor))
                {
                    if (animateSearch && neighbor != start && neighbor != end)
                    {
                        processingAction?.Invoke(neighbor);
                        yield return new WaitForSeconds(0.01f);
                    }

                    neighbor.Cost = fCost;
                    neighbor.CameFrom = current;
                    unexplored.Enqueue(neighbor, neighbor.Cost);
                }
            }

            if (animateSearch && current != start && current != end)
            {
                processedAction?.Invoke(current);
            }
        }

        yield return null;
    }

    private int GetDistance(MapNode a, MapNode b)
    {
        float xDist = a.XIndex - b.XIndex;
        float zDist = a.ZIndex - b.ZIndex;
        return (int)(Mathf.Sqrt(xDist * xDist + zDist * zDist) * 10);
    }
}
