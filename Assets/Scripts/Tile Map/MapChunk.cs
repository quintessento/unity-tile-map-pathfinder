using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tile map is split into these map chunks for efficiency and allowing virtually any size of meshes.
/// </summary>
public class MapChunk : MonoBehaviour
{
    //combined tiles mesh
    private TilesChunk _tilesChunk;
    //combined obstacles mesh
    private ObstaclesChunk _obstaclesChunk;

    /// <summary>
    /// Generates a new chunk of the map chunk given the provided coordinates.
    /// </summary>
    /// <param name="numTilesXInChunk">Number of tiles in x direction in this chunk.</param>
    /// <param name="numTilesZInChunk">Number of tiles in z direction in this chunk.</param>
    /// <param name="chunkX">Chunk's x index.</param>
    /// <param name="chunkZ">Chunk's z index.</param>
    /// <param name="prevChunkTilesX">Number of tiles in x direction in the previously-spawned chunk.</param>
    /// <param name="prevChunkTilesZ">Number of tiles in z direction in the previously-spawned chunk.</param>
    /// <param name="map">Map from which the tile map is generated.</param>
    /// <param name="allTiles">One-dimensional list of tiles.</param>
    /// <param name="tiles">Two-dimensional array of tiles.</param>
    /// <param name="nodeToTile">Dictionary that maps map nodes to their corresponding tiles.</param>
    public void GenerateMapChunk(
        int numTilesXInChunk, int numTilesZInChunk, 
        int chunkX, int chunkZ, 
        int prevChunkTilesX, int prevChunkTilesZ, 
        Map map, 
        List<Tile> allTiles, 
        Tile[,] tiles, 
        Dictionary<MapNode, Tile> nodeToTile)
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
