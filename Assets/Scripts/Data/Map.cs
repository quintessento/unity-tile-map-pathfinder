using System;
using System.Collections.Generic;
using System.IO;
using Random = UnityEngine.Random;

/// <summary>
/// Representation of the world made with nodes which can later be translated into tiles.
/// </summary>
public class Map
{
    /// <summary>
    /// Number of nodes along the X axis.
    /// </summary>
    public int SizeX { get; private set; }

    /// <summary>
    /// Number of nodes along the Z axis.
    /// </summary>
    public int SizeZ { get; private set; }

    private readonly MapNode[,] _nodes;

    //used to get an unoccupied node after the map has been generated
    private readonly List<MapNode> _emptyNodes;

    public Map(int sizeX, int sizeZ, int numObstacles, bool weighted = false)
	{
        SizeX = sizeX;
        SizeZ = sizeZ;
        
        _nodes = new MapNode[sizeX, sizeZ];
        _emptyNodes = new List<MapNode>();
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

                _emptyNodes.Add(node);
            }
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

        _nodes = new MapNode[SizeX, SizeZ];
        _emptyNodes = new List<MapNode>();
        for (int z = 0; z < SizeZ; z++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                MapNode node = new MapNode(reader);
                _nodes[x, z] = node;

                AssignNeighbors(node, x, z);

                if (!node.HasObstacle)
                    _emptyNodes.Add(node);
            }
        }
    }

    public MapNode this[int x, int z] => _nodes[x, z];

    public void Save(BinaryWriter writer)
    {
        writer.Write(SizeX);
        writer.Write(SizeZ);

        for (int z = 0; z < SizeZ; z++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                _nodes[x, z].Save(writer);
            }
        }
    }

    public MapNode PopRandomEmptyNode()
    {
        if (_emptyNodes.Count > 0)
        {
            MapNode node = _emptyNodes[Random.Range(0, _emptyNodes.Count)];
            _emptyNodes.Remove(node);
            return node;
        }
        return null;
    }

    public void ReturnEmptyNode(MapNode node)
    {
        if(node != null && !node.HasObstacle && !_emptyNodes.Contains(node))
        {
            _emptyNodes.Add(node);
        }
    }

    //registers neigbors with nodes during construction of the map
    //should be called in the constructor or a loader, where the nodes array is filled
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

    //places obstacles at nodes with provided coordinates
    private void PlaceObstacle(List<Tuple<int, int>> coords)
    {
        for (int i = 0; i < coords.Count; i++)
        {
            int tileX = coords[i].Item1;
            int tileZ = coords[i].Item2;
            
            MapNode node = _nodes[tileX, tileZ];
            node.HasObstacle = true;
            _emptyNodes.Remove(node);

            //remove this tile from connected neighbors of its surrounding neighbors
            for (int neighborIndex = 0; neighborIndex < node.ConnectedNeighbors.Count; neighborIndex++)
            {
                MapNode neighbor = node.ConnectedNeighbors[neighborIndex];
                neighbor.ConnectedNeighbors.Remove(node);
            }
        }
    }
}
