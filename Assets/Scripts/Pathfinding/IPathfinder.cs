using System;
using System.Collections;

public interface IPathfinder
{
    IEnumerator FindPath(
        MapNode start, 
        MapNode end, 
        bool animateSearch = false, 
        Action<MapNode> processingAction = null, 
        Action<MapNode> processedAction = null
    );
}
