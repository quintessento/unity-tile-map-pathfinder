using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirst : IPathfinder
{
    public IEnumerator FindPath(MapNode start, MapNode end, bool animateSearch = false, Action<MapNode> processingAction = null, Action<MapNode> processedAction = null)
    {
        Stack<MapNode> unprocessed = new Stack<MapNode>();
        List<MapNode> processed = new List<MapNode>();

        unprocessed.Push(start);
        processed.Add(start);

        start.CameFrom = null;

        while (unprocessed.Count > 0)
        {
            MapNode current = unprocessed.Pop();

            if (current == end)
            {
                yield break;
            }

            IList<MapNode> neighbors = current.ConnectedNeighbors;
            for (int i = 0; i < neighbors.Count; i++)
            {
                MapNode neighbor = neighbors[i];
                if (!processed.Contains(neighbor))
                {
                    if (animateSearch && neighbor != start && neighbor != end)
                    {
                        processingAction?.Invoke(neighbor);
                        yield return new WaitForSeconds(0.01f);
                    }

                    unprocessed.Push(neighbor);
                    processed.Add(neighbor);
                    neighbor.CameFrom = current;
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
