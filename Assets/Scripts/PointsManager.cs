using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager
{

    public const int Match_3_Points = 100;

    public const int Match_4_Points = 500;

    public const int Combo_2_Match_Points = 1000;
    
    public const int Combo_3_Match_Points = 10000;

    public const int Empty_Cone_Multiplier = 2;


    public static float Points = 0f;


    public static float GetPointsFromMatch(int scoopsInMatch) {
        // f(x) = 4(5^(x-1)) yields f(3) = 100, f(4) = 500, f(5) = 2500
        return 4*Mathf.Pow(5, scoopsInMatch - 1); 
    }

    public static float CalculatePoints(float points, int multiplier) {
        return points * Mathf.Pow(10, multiplier);
    }

    public static void AddPoints(float points) {
        Points += points;
    }

    public int RoundedPoints() {
        return (int) Points;
    }


}
