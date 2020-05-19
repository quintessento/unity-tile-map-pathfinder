using System;
using System.Collections;
using System.Collections.Generic;

public interface IPathfinder
{
    IEnumerator FindPath(Tile start, Tile end, IList<Tile> validNodes, bool animateSearch = false, Action<IPathfindingNode> processingAction = null, Action<IPathfindingNode> processedAction = null);
}
