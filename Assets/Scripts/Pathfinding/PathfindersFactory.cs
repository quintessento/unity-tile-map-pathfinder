using System;
using System.Linq;

/// <summary>
/// Gives access to existing pathfinding algorithms. Any of the IPathfinder implementers in the project will be picked up using reflection and
/// made available in the runtime interface.
/// </summary>
public static class PathfindersFactory
{
    public static Type[] GetAvailablePathfinderTypes()
    {
        Type type = typeof(IPathfinder);
        Type[] types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => 
                type.IsAssignableFrom(p) && 
                p != type && 
                Attribute.GetCustomAttribute(p, typeof(ExcludeAlgorithm)) == null
            ).ToArray();

        return types;
    }

    public static IPathfinder GetPathfinderForType(Type type)
    {
        return (IPathfinder)Activator.CreateInstance(type);
    }
}
