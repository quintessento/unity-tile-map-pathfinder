using System.Collections.Generic;

public interface IPathfindingNode
{
    IList<IPathfindingNode> Path { get; set; }
    IList<IPathfindingNode> Neighbors { get; set; }
    bool IsOccupied { get; set; }
}
