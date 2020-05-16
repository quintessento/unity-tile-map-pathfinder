using UnityEngine;

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

    public static int[,] obstacleL1 = new int[,]
    {
       { 1, 0 },
       { 1, 0 },
       { 1, 1 }
    };

    public static int[,] obstacleL2 = new int[,]
    {
       { 0, 0, 1 },
       { 1, 1, 1 }
    };


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
