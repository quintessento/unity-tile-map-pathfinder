using System;
using System.Collections;
using System.Collections.Generic;

public interface IPathfinder
{
    IEnumerator FindPath(MapNode start, MapNode end, bool animateSearch = false, Action<MapNode> processingAction = null, Action<MapNode> processedAction = null);
}
