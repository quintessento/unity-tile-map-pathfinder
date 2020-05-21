using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class Map
{
    public int SizeX { get; private set; }
    public int SizeZ { get; private set; }

    private MapNode[,] _nodes;
    private MapNode[] _nodesArray;

    public Map(int sizeX, int sizeZ, int numObstacles)
	{
        SizeX = sizeX;
        SizeZ = sizeZ;

        _nodesArray = new MapNode[sizeX * sizeZ];
        _nodes = new MapNode[sizeX, sizeZ];
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                MapNode node = new MapNode(x, z);
                _nodes[x, z] = node;

                int index = x + sizeZ * z;
                _nodesArray[index] = node;
            }
        }

        for (int i = 0; i < _nodesArray.Length; i++)
        {
            _nodesArray[i].ConnectedNeighbors = GetNeighbors(_nodesArray[i]);
        }

        for (int i = 0; i < numObstacles; i++)
        {
            PlaceObstacles();
        }
    }

    public Map(BinaryReader reader)
    {
        SizeX = reader.ReadInt32();
        SizeZ = reader.ReadInt32();

        _nodesArray = new MapNode[SizeX * SizeZ];
        _nodes = new MapNode[SizeX, SizeZ];
        for (int x = 0; x < SizeX; x++)
        {
            for (int z = 0; z < SizeZ; z++)
            {
                MapNode node = new MapNode(reader);
                _nodes[x, z] = node;

                int index = x + SizeZ * z;
                _nodesArray[index] = node;
            }
        }

        for (int i = 0; i < _nodesArray.Length; i++)
        {
            _nodesArray[i].ConnectedNeighbors = GetNeighbors(_nodesArray[i]);
        }
    }

    public MapNode this[int x, int z] => _nodes[x, z];

    public MapNode[] AsArray => _nodesArray;

    public void Save(BinaryWriter writer)
    {
        writer.Write(SizeX);
        writer.Write(SizeZ);
        for (int i = 0; i < _nodesArray.Length; i++)
        {
            _nodesArray[i].Save(writer);
        }
    }

    private List<MapNode> GetNeighbors(MapNode node)
    {
        List<MapNode> neighbors = new List<MapNode>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    //it's the current tile -> skip
                    continue;
                }

                if (Mathf.Abs(i) == Mathf.Abs(j))
                {
                    //we are going in diagonal -> skip
                    continue;
                }

                int xIndex = Mathf.Clamp(node.XIndex + i, 0, SizeX - 1);
                int zIndex = Mathf.Clamp(node.ZIndex + j, 0, SizeZ - 1);

                MapNode neighbor = _nodes[xIndex, zIndex];
                if (neighbor == null)
                {
                    //null -> skip
                    continue;
                }
                if (neighbor.HasObstacle)
                {
                    //skip the neigbor with obstacle (in case we are loading the map)
                    continue;
                }

                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private void PlaceObstacles()
    {
        int[,] obstacleBlueprint = Obstacles.RandomDeclared;

        int obstacleSizeX = obstacleBlueprint.GetLength(0);
        int obstacleSizeZ = obstacleBlueprint.GetLength(1);

        List<List<Tuple<int, int>>> possiblePlacements = new List<List<Tuple<int, int>>>();

        for (int x = 0; x < SizeX - obstacleSizeZ + 1; x++)
        {
            for (int z = 0; z < SizeZ - obstacleSizeX + 1; z++)
            {
                bool fits = true;
                List<Tuple<int, int>> coords = new List<Tuple<int, int>>();

                for (int i = 0; i < obstacleSizeZ; i++)
                {
                    for (int j = 0; j < obstacleSizeX; j++)
                    {
                        //reverse x so that we get the blueprint values in the correct order
                        if (obstacleBlueprint[obstacleSizeX - j - 1, i] == 0)
                        {
                            //skip empty
                            continue;
                        }

                        int obstacleX = x + i;
                        int obstacleZ = z + j;
                        if (_nodes[obstacleX, obstacleZ].HasObstacle)
                        {
                            //placement is invalid, because one of the required tiles is already occupied
                            fits = false;
                            continue;
                        }
                        coords.Add(new Tuple<int, int>(obstacleX, obstacleZ));
                    }
                }

                if (fits)
                {
                    possiblePlacements.Add(coords);
                }
            }
        }

        if (possiblePlacements.Count > 0)
        {
            List<Tuple<int, int>> placement = possiblePlacements[Random.Range(0, possiblePlacements.Count)];
            PlaceObstacle(placement);
        }
    }

    private void PlaceObstacle(List<Tuple<int, int>> coords)
    {
        for (int i = 0; i < coords.Count; i++)
        {
            int tileX = coords[i].Item1;
            int tileZ = coords[i].Item2;

            MapNode node = _nodes[tileX, tileZ];
            node.HasObstacle = true;

            //remove this tile from connected neighbors of its surrounding neighbors
            for (int neighborIndex = 0; neighborIndex < node.ConnectedNeighbors.Count; neighborIndex++)
            {
                MapNode neighbor = node.ConnectedNeighbors[neighborIndex];
                neighbor.ConnectedNeighbors.Remove(node);
            }
        }
    }
}
