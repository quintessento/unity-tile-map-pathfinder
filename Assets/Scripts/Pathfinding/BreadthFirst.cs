using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirst : IPathfinder
{
    public IEnumerator FindPath(Tile start, Tile end, bool animateSearch = false)
    {
        Queue<Tile> unprocessed = new Queue<Tile>();
        List<Tile> processed = new List<Tile>();

        unprocessed.Enqueue(start);
        processed.Add(start);
        start.path = new List<Tile>();

        while (unprocessed.Count > 0)
        {
            Tile current = unprocessed.Dequeue();

            if (animateSearch)
            {
                current.SetColor(Color.gray);
                yield return new WaitForSeconds(0.05f);
            }

            if (current == end)
            {
                yield return current.path;
                yield break;
            }

            List<Tile> neighbors = current.neighbors;
            for (int i = 0; i < neighbors.Count; i++)
            {
                Tile neighbor = neighbors[i];
                if (!processed.Contains(neighbor))
                {
                    unprocessed.Enqueue(neighbor);
                    processed.Add(neighbor);
                    neighbor.path = new List<Tile>(current.path);
                    if (current != start)
                        neighbor.path.Add(current);
                }
            }
        }

        yield return null;
    }
}
