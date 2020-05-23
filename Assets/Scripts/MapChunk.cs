using System.Collections.Generic;
using UnityEngine;

public class MapChunk : MonoBehaviour
{
    private TilesChunk _tilesChunk;
    private ObstaclesChunk _obstaclesChunk;

    public void GenerateMapChunk(int numTilesXInChunk, int numTilesZInChunk, int chunkX, int chunkZ, int prevChunkTilesX, int prevChunkTilesZ, Map map, List<Tile> allTiles, Tile[,] tiles, Dictionary<MapNode, Tile> nodeToTile)
    {
        if (_tilesChunk == null)
        {
            _tilesChunk = GetComponentInChildren<TilesChunk>();
        }

        if (_obstaclesChunk == null)
        {
            _obstaclesChunk = GetComponentInChildren<ObstaclesChunk>();
        }

        _tilesChunk.Clear();
        _obstaclesChunk.Clear();

        for (int z = 0; z < numTilesZInChunk; z++)
        {
            for (int x = 0; x < numTilesXInChunk; x++)
            {
                int chunkOffsetX = chunkX * prevChunkTilesX;
                int chunkOffsetZ = chunkZ * prevChunkTilesZ;
                int tileX = chunkOffsetX + x;
                int tileZ = chunkOffsetZ + z;

                MapNode node = map[tileX, tileZ];

                if (!node.HasObstacle)
                {
                    _tilesChunk.AddTile(tileX, tileZ, map, allTiles, tiles, nodeToTile);
                }
                else
                {
                    _obstaclesChunk.AddObstacle(tileX, tileZ);
                }
            }
        }

        _tilesChunk.Apply();
        _obstaclesChunk.Apply();
    }
}
