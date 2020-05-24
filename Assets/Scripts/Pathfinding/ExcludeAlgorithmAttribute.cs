/// <summary>
/// Implementors of IPathfinder marked with this attribute will not be available during runtime.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class)]
public class ExcludeAlgorithm : System.Attribute { }
