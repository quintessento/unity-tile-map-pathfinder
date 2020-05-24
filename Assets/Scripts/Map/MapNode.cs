using System.Collections.Generic;
using System.IO;

public class MapNode
{
    public int XIndex { get; private set; }
    public int ZIndex { get; private set; }

	public List<MapNode> ConnectedNeighbors;

	public bool HasObstacle { get; set; }

	//pathfinding
	public int Weight { get; set; }
	public int Cost { get; set; }
	public MapNode CameFrom { get; set; }

	//-----------

	public MapNode(int x, int z, int weight)
	{
		XIndex = x;
		ZIndex = z;
		Weight = weight;

		ConnectedNeighbors = new List<MapNode>();
	}

	public MapNode(BinaryReader reader)
	{
		XIndex = reader.ReadInt32();
		ZIndex = reader.ReadInt32();
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
		writer.Write(XIndex);
		writer.Write(ZIndex);
		writer.Write(HasObstacle);
		writer.Write(Weight);
	}
}
