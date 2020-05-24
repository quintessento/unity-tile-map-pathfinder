using System;
using System.Linq;

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
