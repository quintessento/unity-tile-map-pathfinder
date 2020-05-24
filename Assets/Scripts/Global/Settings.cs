using System;
using UnityEngine;

/// <summary>
/// Shared collection of user settings that can be persisted via PlayerPrefs or otherwise stored.
/// </summary>
public static class Settings
{
    /// <summary>
    /// Size of a square map, where x = MapSize and z = MapSize.
    /// </summary>
    public static int MapSize { get; private set; }
    public static int NumObstacles { get; private set; }
    /// <summary>
    /// Are the nodes/tiles weighted?
    /// </summary>
    public static bool IsWeighted { get; private set; }

    /// <summary>
    /// Indicates whether tiles show labels with some useful information.
    /// </summary>
    public static TileDebugStyle TileDebugStyle { get; private set; }

    /// <summary>
    /// Selected pathfidning algorithm.
    /// </summary>
    public static Type Pathfinder { get; private set; }

    /// <summary>
    /// Will the pathfinding algorithm show its progress before showing the optimal path it had found.
    /// </summary>
    public static bool AnimateSearch { get; private set; }

    public static bool IsCameraOrthographic { get; private set; }

    #region Setters

    public static void SetMapSize(int size, bool notifyChange)
    {
        MapSize = size;

        PlayerPrefs.SetInt(key_mapSize, size);

        if (notifyChange)
        {
            SettingsChanged?.Invoke(null, null);
        }
    }

    public static void SetNumObstacles(int numObstacles, bool notifyChange)
    {
        NumObstacles = numObstacles;

        PlayerPrefs.SetInt(key_numObstacles, numObstacles);

        if (notifyChange)
        {
            SettingsChanged?.Invoke(null, null);
        }
    }

    public static void SetIsWeighted(bool isWeighted, bool notifyChange)
    {
        IsWeighted = isWeighted;

        if (notifyChange)
        {
            SettingsChanged?.Invoke(null, null);
        }
    }

    public static void SetTileDebugStyle(TileDebugStyle style, bool notifyChange)
    {
        TileDebugStyle = style;

        if (notifyChange)
        {
            SettingsChanged?.Invoke(null, null);
        }
    }

    public static void SetPathfinder(Type pathfinder, bool notifyChange)
    {
        Pathfinder = pathfinder;

        if (notifyChange)
        {
            SettingsChanged?.Invoke(null, null);
        }
    }

    public static void SetAnimateSearch(bool animateSearch, bool notifyChange)
    {
        AnimateSearch = animateSearch;

        if (notifyChange)
        {
            SettingsChanged?.Invoke(null, null);
        }
    }

    public static void SetCameraOrthographic(bool isOrthographic, bool notifyChange)
    {
        IsCameraOrthographic = isOrthographic;

        PlayerPrefs.SetInt(key_isCameraOrthographic, isOrthographic ? 1 : 0);

        if (notifyChange)
        {
            SettingsChanged?.Invoke(null, null);
        }
    }

    #endregion

    /// <summary>
    /// Fired when any of the settings have changed.
    /// </summary>
    public static event EventHandler SettingsChanged;

    private const string key_mapSize = "key_map_size";
    private const string key_numObstacles = "key_num_obstacles";
    private const string key_isCameraOrthographic = "key_orth_cam";

    static Settings()
    {
        InitializeDefaults();
    }

    private static void InitializeDefaults()
    {
        MapSize = 10;
        NumObstacles = 10;
        IsCameraOrthographic = true;
    }

    private static void InitializeFromPlayerPrefs()
    {
        MapSize = PlayerPrefs.GetInt(key_mapSize, 10);
        NumObstacles = PlayerPrefs.GetInt(key_numObstacles, 10);
        IsCameraOrthographic = PlayerPrefs.GetInt(key_isCameraOrthographic, 1) == 1 ? true : false;
    }
}
