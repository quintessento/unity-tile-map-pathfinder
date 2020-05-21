using System.Collections.Generic;
using System.IO;

public class MapNode
{
    public int XIndex { get; private set; }
    public int ZIndex { get; private set; }

	public List<MapNode> ConnectedNeighbors;

	public bool HasObstacle { get; set; }

	//pathfinding
	public IList<MapNode> Path { get; set; }
	public int Distance { get; set; }
	//-----------

	public MapNode(int x, int z)
	{
		XIndex = x;
		ZIndex = z;

		ConnectedNeighbors = new List<MapNode>();
	}

	public MapNode(BinaryReader reader)
	{
		XIndex = reader.ReadInt32();
		ZIndex = reader.ReadInt32();
		HasObstacle = reader.ReadBoolean();
		Distance = reader.ReadInt32();

		ConnectedNeighbors = new List<MapNode>();
	}

	public void Save(BinaryWriter writer)
	{
		writer.Write(XIndex);
		writer.Write(ZIndex);
		writer.Write(HasObstacle);
		writer.Write(Distance);
	}
}
