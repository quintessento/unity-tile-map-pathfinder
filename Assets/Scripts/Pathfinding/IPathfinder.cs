using System.Collections;

public interface IPathfinder
{
    IEnumerator FindPath(Tile start, Tile end, bool animateSearch = false);
}
