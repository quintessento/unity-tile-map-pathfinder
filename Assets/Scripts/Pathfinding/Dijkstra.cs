using Custom.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : IPathfinder
{
    private MapNode _start;
    private MapNode _end;

    public IEnumerator FindPath(MapNode start, MapNode end, bool animateSearch = false, Action<MapNode> processingAction = null, Action<MapNode> processedAction = null)
    {
        _start = start;
        _end = end;

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
                int gCost = current.Cost + DistanceToNeighbor(neighbor);

                if (gCost < neighbor.Cost || !unexplored.Contains(neighbor))
                {
                    neighbor.Cost = gCost;
                    neighbor.CameFrom = current;
                    unexplored.Enqueue(neighbor, neighbor.Cost);

                    if (animateSearch && neighbor != start && neighbor != end)
                    {
                        processingAction?.Invoke(neighbor);
                        yield return null;
                    }
                }
            }

            if (animateSearch && current != start && current != end)
            {
                processedAction?.Invoke(current);
            }
        }

        yield return null;
    }

    //private int DistanceToStart(MapNode node)
    //{
    //    return (int)(GetManhattanDistance(node, _start));
    //}

    private int DistanceToNeighbor(MapNode neighbor)
    {
        return neighbor.Weight;
    }

    private float GetManhattanDistance(MapNode a, MapNode b)
    {
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Z - b.Z);
    }

    //private float GetEuclideanDistance(MapNode a, MapNode b)
    //{
    //    float xDist = a.XIndex - b.XIndex;
    //    float zDist = a.ZIndex - b.ZIndex;
    //    return Mathf.Sqrt(xDist * xDist + zDist * zDist);
    //}
}
