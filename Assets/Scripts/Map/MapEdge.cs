public class MapEdge
{
    public MapNode Node1 { get; private set; }
    public MapNode Node2 { get; private set; }

    public MapEdge(MapNode n1, MapNode n2)
    {
        Node1 = n1;
        Node2 = n2;
    }
}
