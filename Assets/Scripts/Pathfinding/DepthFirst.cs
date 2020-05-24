using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcludeAlgorithm]
public class DepthFirst : IPathfinder
{
    public IEnumerator FindPath(MapNode start, MapNode end, bool animateSearch = false, Action<MapNode> processingAction = null, Action<MapNode> processedAction = null)
    {
        Stack<MapNode> unexplored = new Stack<MapNode>();
        List<MapNode> explored = new List<MapNode>();

        unexplored.Push(start);
        explored.Add(start);

        start.CameFrom = null;

        while (unexplored.Count > 0)
        {
            MapNode current = unexplored.Pop();

            if (current == end)
            {
                yield break;
            }

            IList<MapNode> neighbors = current.ConnectedNeighbors;
            for (int i = 0; i < neighbors.Count; i++)
            {
                MapNode neighbor = neighbors[i];
                if (!explored.Contains(neighbor))
                {
                    unexplored.Push(neighbor);
                    explored.Add(neighbor);
                    neighbor.CameFrom = current;

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
}
