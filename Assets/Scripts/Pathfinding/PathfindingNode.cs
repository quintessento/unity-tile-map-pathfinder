using System.Collections.Generic;

public class PathfindingNode : IPathfindingNode
{
    public IList<IPathfindingNode> Path { get; set; }
    public IList<IPathfindingNode> Neighbors { get; set; }
    public bool IsOccupied { get; set; }
}
