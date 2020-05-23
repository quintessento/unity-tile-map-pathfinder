using System;
using System.Collections.Generic;
using System.IO;
using Random = UnityEngine.Random;

public class Map
{
    public int SizeX { get; private set; }
    public int SizeZ { get; private set; }

    private MapNode[,] _nodes;

    public Map(int sizeX, int sizeZ, int numObstacles, bool weighted = false)
	{
        SizeX = sizeX;
        SizeZ = sizeZ;
        
        NodesList = new List<MapNode>(sizeX * sizeZ);
        _nodes = new MapNode[sizeX, sizeZ];
        for (int z = 0; z < sizeZ; z++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                int weight = 1;
                if(weighted)
                {
                    weight = Random.value > 0.3f ? 1 : Random.Range(2, 10);
                }

                MapNode node = new MapNode(x, z, weight);
                _nodes[x, z] = node;

                AssignNeighbors(node, x, z);

                NodesList.Add(node);
            }
        }

        //for (int i = 0; i < NodesList.Count; i++)
        //{
        //    NodesList[i].ConnectedNeighbors = GetNeighbors(NodesList[i]);
        //}

        for (int i = 0; i < numObstacles; i++)
        {
            PlaceObstacles();
        }
    }

    public Map(BinaryReader reader)
    {
        SizeX = reader.ReadInt32();
        SizeZ = reader.ReadInt32();

        NodesList = new List<MapNode>(SizeX * SizeZ);
        _nodes = new MapNode[SizeX, SizeZ];
        for (int x = 0; x < SizeX; x++)
        {
            for (int z = 0; z < SizeZ; z++)
            {
                MapNode node = new MapNode(reader);
                _nodes[x, z] = node;

                NodesList.Add(node);
            }
        }

        //for (int i = 0; i < NodesList.Count; i++)
        //{
        //    NodesList[i].ConnectedNeighbors = GetNeighbors(NodesList[i]);
        //}
    }

    public MapNode this[int x, int z] => _nodes[x, z];

    public List<MapNode> NodesList { get; }

    public void Save(BinaryWriter writer)
    {
        writer.Write(SizeX);
        writer.Write(SizeZ);
        for (int i = 0; i < NodesList.Count; i++)
        {
            NodesList[i].Save(writer);
        }
    }

    private void AssignNeighbors(MapNode node, int x, int z)
    {
        if (x > 0 && x < (SizeX - 1) && z > 0 && z < (SizeZ - 1))
        {
            //middle of the map
            node.AddNeighbor(_nodes[x - 1, z]);
            node.AddNeighbor(_nodes[x, z - 1]);
        }
        else if (x == 0 && z > 0 && z < (SizeZ - 1))
        {
            //left border
            node.AddNeighbor(_nodes[x, z - 1]);
        }
        else if (x == (SizeX - 1) && z > 0 && z < (SizeZ - 1))
        {
            //right border
            node.AddNeighbor(_nodes[x, z - 1]);
            node.AddNeighbor(_nodes[x - 1, z]);
        }
        else if (z == 0 && x > 0 && x < (SizeX - 1))
        {
            //bottom border
            node.AddNeighbor(_nodes[x - 1, z]);
        }
        else if (z == (SizeZ - 1) && x > 0 && x < (SizeX - 1))
        {
            //top border
            node.AddNeighbor(_nodes[x, z - 1]);
            node.AddNeighbor(_nodes[x - 1, z]);
        }
        else if (x == 0 && z == 0)
        {
            //BL corner
        }
        else if (x == (SizeX - 1) && z == (SizeZ - 1))
        {
            //TR corner
            node.AddNeighbor(_nodes[x - 1, z]);
            node.AddNeighbor(_nodes[x, z - 1]);
        }
        else if (x == (SizeX - 1) && z == 0)
        {
            //BR corner
            node.AddNeighbor(_nodes[x - 1, z]);
        }
        else if (x == 0 && z == (SizeZ - 1))
        {
            //TL corner
            node.AddNeighbor(_nodes[x, z - 1]);
        }
    }

    //private List<MapNode> GetNeighbors(MapNode node)
    //{
    //    List<MapNode> neighbors = new List<MapNode>();

    //    for (int i = -1; i <= 1; i++)
    //    {
    //        for (int j = -1; j <= 1; j++)
    //        {
    //            if (i == 0 && j == 0)
    //            {
    //                //it's the current tile -> skip
    //                continue;
    //            }

    //            if (Mathf.Abs(i) == Mathf.Abs(j))
    //            {
    //                //we are going in diagonal -> skip
    //                continue;
    //            }

    //            int xIndex = node.ZIndex + j;
    //            int zIndex = node.XIndex + i;

    //            if (xIndex < 0 || xIndex >= SizeX || zIndex < 0 || zIndex >= SizeZ)
    //                continue;

    //            MapNode neighbor = _nodes[xIndex, zIndex];
    //            if (neighbor == null)
    //            {
    //                //null -> skip
    //                continue;
    //            }
    //            //if (neighbor.HasObstacle)
    //            //{
    //            //    //skip the neigbor with obstacle (in case we are loading the map)
    //            //    continue;
    //            //}

    //            neighbors.Add(neighbor);
    //        }
    //    }

    //    return neighbors;
    //}

    private void PlaceObstacles()
    {
        int[,] obstacleBlueprint = Obstacles.RandomDeclared;

        int obstacleSizeX = obstacleBlueprint.GetLength(0);
        int obstacleSizeZ = obstacleBlueprint.GetLength(1);

        List<List<Tuple<int, int>>> possiblePlacements = new List<List<Tuple<int, int>>>();

        for (int z = 0; z < SizeZ - obstacleSizeX + 1; z++)
        {
            for (int x = 0; x < SizeX - obstacleSizeZ + 1; x++)
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
