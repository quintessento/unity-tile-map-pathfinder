using System;
using System.Collections;

/// <summary>
/// Pathfinding algorithms must implement this interface, which ensures automatic loading in the runtime interface.
/// </summary>
public interface IPathfinder
{
    /// <summary>
    /// Main body of a pathfinding algorithm where the path is built.
    /// </summary>
    /// <param name="start">Start node, from which the search initiates.</param>
    /// <param name="end">End node, at which the path from start must terminate.</param>
    /// <param name="animateSearch">Indicates whether delays will be introduced to visualize the search progress.</param>
    /// <param name="processingAction">Callback that passes a currently processed node for coloring or other debug/visualization action.</param>
    /// <param name="processedAction">Callback that passes an already processed node for coloring or other debug/visualization action.</param>
    /// <returns>Yield instruction.</returns>
    IEnumerator FindPath(
        MapNode start, 
        MapNode end, 
        bool animateSearch = false, 
        Action<MapNode> processingAction = null, 
        Action<MapNode> processedAction = null
    );
}
