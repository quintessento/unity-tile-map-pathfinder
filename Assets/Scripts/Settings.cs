using System;
using UnityEngine;

public class Settings
{
    public static int MapSize { get; private set; }
    public static int NumObstacles { get; private set; }

    public static Type Pathfinder { get; private set; }

    public static bool AnimateSearch { get; private set; }

    public static bool IsCameraOrthographic { get; private set; }

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
