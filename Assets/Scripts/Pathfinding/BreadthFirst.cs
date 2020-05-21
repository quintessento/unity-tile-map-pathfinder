using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirst : IPathfinder
{
    public IEnumerator FindPath(MapNode start, MapNode end, IList<MapNode> validNodes, bool animateSearch = false, Action<MapNode> processingAction = null, Action<MapNode> processedAction = null)
    {
        Queue<MapNode> unprocessed = new Queue<MapNode>();
        List<MapNode> processed = new List<MapNode>();

        unprocessed.Enqueue(start);
        processed.Add(start);
        start.Path = new List<MapNode>();

        while (unprocessed.Count > 0)
        {
            MapNode current = unprocessed.Dequeue();

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

                    unprocessed.Enqueue(neighbor);
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
