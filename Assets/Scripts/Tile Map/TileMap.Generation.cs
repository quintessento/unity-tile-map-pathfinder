using UnityEngine;

public partial class TileMap : MonoBehaviour
{
    public void GenerateMap(bool useSettings = true)
    {
        ClearMap();

        if (useSettings)
        {
            _sizeX = _sizeZ = Settings.MapSize;
            _numObstacles = Settings.NumObstacles;
        }

        Map = new Map(_sizeX, _sizeZ, _numObstacles, _isWeighted);
        GenerateTileMap(Map);
    }

    private void GenerateTileMap(Map map)
    {
        _tiles = new Tile[_sizeX, _sizeZ];

        _numChunksX = Mathf.CeilToInt(map.SizeX / (float)_tilesPerChunkX);
        _numChunksZ = Mathf.CeilToInt(map.SizeZ / (float)_tilesPerChunkZ);

        CreateMapChunks();
    }

    private void CreateMapChunks()
    {
        int prevChunkX = 0, prevChunkZ = 0;
        for (int x = 0; x < _numChunksX; x++)
        {
            for (int z = 0; z < _numChunksZ; z++)
            {
                MapChunk chunk = _chunksPool.Get();
                if (chunk == null)
                {
                    chunk = Instantiate(_mapChunkPrefab);
                }
                chunk.transform.SetParent(transform);

                int numTilesXInChunk = Mathf.Min(Map.SizeX, _tilesPerChunkX);
                int undividedTilesX = (x + 1) * _tilesPerChunkX;
                if (undividedTilesX > Map.SizeX)
                    numTilesXInChunk = Map.SizeX % _tilesPerChunkX;

                int numTilesZInChunk = Mathf.Min(Map.SizeZ, _tilesPerChunkZ);
                int undividedTilesZ = (z + 1) * _tilesPerChunkZ;
                if (undividedTilesZ > Map.SizeZ)
                    numTilesZInChunk = Map.SizeZ % _tilesPerChunkZ;

                chunk.GenerateMapChunk(numTilesXInChunk, numTilesZInChunk, x, z, prevChunkX, prevChunkZ, Map, _allTiles, _tiles, _nodeToTile);

                _mapChunks.Add(chunk);

                if (x == z)
                {
                    prevChunkX = numTilesXInChunk;
                    prevChunkZ = numTilesZInChunk;
                }
            }
        }
    }
}
