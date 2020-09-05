using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PointsManager
{
    public static int Points = 0;


    public static int Empty_Cone_Bonus {
        get {
            return GetPointsFromMatch(3);
        }
    }


    public static event Action PointsAdded = delegate {};
    public static event Action<int, int> PointsAccrue = delegate {};
    public static event Action EmptyConeBonus = delegate {};

    private static int GetPointsFromMatch(int scoopsInMatch) {
        return (int) 50 * (scoopsInMatch*scoopsInMatch + 1); 
    }

    public static int CalculatePoints(int points, int multiplier) {
        return points * multiplier;
    }

    public static int AccruePoints(int scoopsInMatch, int comboMultiplier) {
        int points = GetPointsFromMatch(scoopsInMatch);
        PointsAccrue(points, comboMultiplier);
        return points;
    }

    public static void AddPoints(int points) {
        Points += points;
        PointsAdded();
    }

    public static int AddEmptyConeBonus() {
        EmptyConeBonus();
        return Empty_Cone_Bonus;
    }


}
