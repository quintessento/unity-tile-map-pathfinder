using Custom.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : IPathfinder
{
    public IEnumerator FindPath(MapNode start, MapNode end, IList<MapNode> validNodes, bool animateSearch = false, Action<MapNode> processingAction = null, Action<MapNode> processedAction = null)
    {
        PriorityQueue<MapNode> unexplored = new PriorityQueue<MapNode>();

        //initialization
        for (int i = 0; i < validNodes.Count; i++)
        {
            MapNode tile = validNodes[i];
            tile.Path = new List<MapNode>();
            tile.Distance = int.MaxValue;
            unexplored.Enqueue(tile, tile.Distance);
        }

        unexplored.ChangePriority(start, int.MaxValue, 0);
        start.Distance = 0;

        //processing
        while (unexplored.Count > 0)
        {
            MapNode current = unexplored.Dequeue();

            if (current == end)
            {
                //finished
                yield return current.Path;
                yield break;
            }

            for (int i = 0; i < current.ConnectedNeighbors.Count; i++)
            {
                MapNode neighbor = current.ConnectedNeighbors[i];

                int cameFromDistance = Mathf.Min(current.Distance, int.MaxValue - 1);
                int nextPathDistance = 1;
                int distanceScore = cameFromDistance + nextPathDistance;
                int distanceScoreWithHeuristic = distanceScore + GetHeuristicEuclidean(current, neighbor);

                if (distanceScoreWithHeuristic < neighbor.Distance)
                {
                    if (animateSearch && neighbor != start && neighbor != end)
                    {
                        processingAction?.Invoke(neighbor);
                        yield return new WaitForSeconds(0.05f);
                    }

                    unexplored.ChangePriority(neighbor, neighbor.Distance, distanceScoreWithHeuristic);

                    neighbor.Distance = distanceScoreWithHeuristic;
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

    private int GetHeuristicManhattan(MapNode a, MapNode b)
    {
        return Mathf.Abs(a.XIndex - b.XIndex) + Mathf.Abs(a.ZIndex - b.ZIndex);
    }

    private int GetHeuristicEuclidean(MapNode a, MapNode b)
    {
        float xDist = a.XIndex - b.XIndex;
        float zDist = a.ZIndex - b.ZIndex;
        //ceiling here might not be giving an accurate result!
        return Mathf.CeilToInt(Mathf.Sqrt(xDist * xDist + zDist * zDist));
    }
}
