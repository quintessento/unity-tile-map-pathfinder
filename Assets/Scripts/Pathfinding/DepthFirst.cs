using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirst : IPathfinder
{
    public IEnumerator FindPath(MapNode start, MapNode end, IList<MapNode> validNodes, bool animateSearch = false, Action<MapNode> processingAction = null, Action<MapNode> processedAction = null)
    {
        Stack<MapNode> unprocessed = new Stack<MapNode>();
        List<MapNode> processed = new List<MapNode>();

        unprocessed.Push(start);
        processed.Add(start);
        start.Path = new List<MapNode>();

        while (unprocessed.Count > 0)
        {
            MapNode current = unprocessed.Pop();

            if (current == end)
            {
                yield return current.Path;
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
                        yield return new WaitForSeconds(0.05f);
                    }

                    unprocessed.Push(neighbor);
                    processed.Add(neighbor);
                    neighbor.Path = new List<MapNode>(current.Path);
                    if (current != start)
                        neighbor.Path.Add(current);
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
