using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirst : IPathfinder
{
    public IEnumerator FindPath(Tile start, Tile end, IList<Tile> validNodes, bool animateSearch = false, Action<IPathfindingNode> processingAction = null, Action<IPathfindingNode> processedAction = null)
    {
        Stack<Tile> unprocessed = new Stack<Tile>();
        List<Tile> processed = new List<Tile>();

        unprocessed.Push(start);
        processed.Add(start);
        start.Path = new List<Tile>();

        while (unprocessed.Count > 0)
        {
            Tile current = unprocessed.Pop();

            if (current == end)
            {
                yield return current.Path;
                yield break;
            }

            IList<Tile> neighbors = current.Neighbors;
            for (int i = 0; i < neighbors.Count; i++)
            {
                Tile neighbor = neighbors[i];
                if (!processed.Contains(neighbor))
                {
                    if (animateSearch && neighbor != start && neighbor != end)
                    {
                        processingAction?.Invoke(neighbor.Node);
                        yield return new WaitForSeconds(0.05f);
                    }

                    unprocessed.Push(neighbor);
                    processed.Add(neighbor);
                    neighbor.Path = new List<Tile>(current.Path);
                    if (current != start)
                        neighbor.Path.Add(current);
                }
            }

            if (animateSearch && current != start && current != end)
            {
                processedAction?.Invoke(current.Node);
            }
        }

        yield return null;
    }
}
