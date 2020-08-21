using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Grid grid;

    public Cone cone;

    public int RandomLane() {
        return Random.Range(0, grid.numberOfLanes);
    }
}
