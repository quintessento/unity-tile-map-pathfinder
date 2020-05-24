using UnityEngine;

/// <summary>
/// Collection of obstacles defined as 2D arrays of 1s and 0s, where 0 stands for empty and 1 for obstacle piece on a tile.
/// </summary>
public class Obstacles
{
    public static int[,] obstacle1x1 = new int[,]
    {
        { 1 }
    };
    public static int[,] obstacle1x2 = new int[,]
    {
        { 1, 1 }
    };
    public static int[,] obstacle2x1 = new int[,]
    {
        { 1 },
        { 1 }
    };
    public static int[,] obstacle2x2 = new int[,]
    {
       { 1, 1 },
       { 1, 1 }
    };

    //for testing
    public static int[,] obstacleL1 = new int[,]
    {
       { 1, 0 },
       { 1, 0 },
       { 1, 1 }
    };

    //for testing
    public static int[,] obstacleL2 = new int[,]
    {
       { 0, 0, 1 },
       { 1, 1, 1 }
    };

    /// <summary>
    /// Returns a random obstacle with equal probability. Available obstacles are 1x1, 1x2, 2x1, 2x2.
    /// </summary>
    public static int[,] RandomDeclared
    {
        get
        {
            float value = Random.value;
            if(value < 0.25f)
            {
                return obstacle1x1;
            }
            else if(value >= 0.25f && value < 0.5f)
            {
                return obstacle1x2;
            }
            else if (value >= 0.5f && value < 0.75f)
            {
                return obstacle2x1;
            }

            return obstacle2x2;
        }
    }
}
