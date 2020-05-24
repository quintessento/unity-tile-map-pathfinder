using System.Collections.Generic;
using System.IO;

/// <summary>
/// Map node represents a cell of the map. Pathfinding can use nodes to find paths.
/// </summary>
public class MapNode
{
    public int X { get; private set; }
    public int Z { get; private set; }

	public List<MapNode> ConnectedNeighbors;

	public bool HasObstacle { get; set; }

    #region Pathfinding
	/// <summary>
	/// Difficulty of reaching or traversing the node. If the map/graph is unweighted, weight is always 1.
	/// Otherwise, weight effectively represents distance to this node from its immediate neighbor.
	/// </summary>
    public int Weight { get; set; }
	/// <summary>
	/// Used to calculate the score of pathfinding navigation.
	/// </summary>
	public int Cost { get; set; }
	/// <summary>
	/// Link to the previous node in the path formed by a pathfinder algorithm.
	/// </summary>
	public MapNode CameFrom { get; set; }
    #endregion

    public MapNode(int x, int z, int weight)
	{
		X = x;
		Z = z;
		Weight = weight;

		ConnectedNeighbors = new List<MapNode>();
	}

	public MapNode(BinaryReader reader)
	{
		X = reader.ReadInt32();
		Z = reader.ReadInt32();
		HasObstacle = reader.ReadBoolean();
		Weight = reader.ReadInt32();

		ConnectedNeighbors = new List<MapNode>();
	}

	public void AddNeighbor(MapNode neighbor)
    {
		if (neighbor == null)
        {
			UnityEngine.Debug.LogError("Trying to add a null neighbor.");
			return;
        }

		if (neighbor.HasObstacle || this.HasObstacle)
		{
			//skip nodes with obstacles, as they are not valid neighbors
			//this case arrises when loading the map from a save file
			return;
		}

		if (!ConnectedNeighbors.Contains(neighbor))
		{
			ConnectedNeighbors.Add(neighbor);
		}
        if (!neighbor.ConnectedNeighbors.Contains(this))
        {
			neighbor.ConnectedNeighbors.Add(this);
        }
    }

	public void Save(BinaryWriter writer)
	{
		writer.Write(X);
		writer.Write(Z);
		writer.Write(HasObstacle);
		writer.Write(Weight);
	}
}
