using Custom.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : IPathfinder
{
    public IEnumerator FindPath(Tile start, Tile end, IList<Tile> validNodes, bool animateSearch = false, Action<IPathfindingNode> processingAction = null, Action<IPathfindingNode> processedAction = null)
    {
        PriorityQueue<Tile> unexplored = new PriorityQueue<Tile>();

        //initialization
        for (int i = 0; i < validNodes.Count; i++)
        {
            Tile tile = validNodes[i];
            tile.Path = new List<Tile>();
            tile.Distance = int.MaxValue;
            unexplored.Enqueue(tile, tile.Distance);
        }

        unexplored.ChangePriority(start, int.MaxValue, 0);
        start.Distance = 0;

        //processing
        while(unexplored.Count > 0)
        {
            Tile current = unexplored.Dequeue();

            if (current == end)
            {
                //finished
                yield return current.Path;
                yield break;
            }

            for (int i = 0; i < current.Neighbors.Count; i++)
            {
                Tile neighbor = current.Neighbors[i];

                int cameFromDistance = Mathf.Min(current.Distance, int.MaxValue - 1);
                int nextPathDistance = 1;
                int distanceScore = cameFromDistance + nextPathDistance;

                if(distanceScore < neighbor.Distance)
                {
                    if (animateSearch && neighbor != start && neighbor != end)
                    {
                        processingAction?.Invoke(neighbor.Node);
                        yield return new WaitForSeconds(0.05f);
                    }

                    unexplored.ChangePriority(neighbor, neighbor.Distance, distanceScore);

                    neighbor.Distance = distanceScore;
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
