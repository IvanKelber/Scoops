using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PointsManager
{

    public const int Empty_Cone_Multiplier = 2;

    public static float Points = 0f;

    public static bool emptyConeBonusActive = false;

    public static event Action<int> PointsAdded = delegate {};

    public static float GetPointsFromMatch(int scoopsInMatch) {
        return 50 * (scoopsInMatch*scoopsInMatch + 1); 
    }

    public static float CalculatePoints(float points, int multiplier) {
        return points * multiplier;
    }

    public static void AddPoints(float points) {
        Points += points;
        PointsAdded((int) points);
    }
    public static int RoundedPoints() {
        return (int) Points;
    }


}
