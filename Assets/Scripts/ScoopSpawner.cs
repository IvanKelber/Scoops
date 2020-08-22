using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopSpawner 
{

    private BoardManager board;
    private Flavor[] flavors;

    private float scoopSpeed;

    public ScoopSpawner(BoardManager board, Flavor[] flavors, float scoopSpeed) {
        this.board = board;
        this.flavors = flavors;
        this.scoopSpeed = scoopSpeed;
    }

    public Scoop SpawnScoop(Scoop scoop) {
        scoop.SetFlavor(RandomFlavor());
        scoop.SetSpeed(scoopSpeed);
        Vector2Int startIndex = new Vector2Int(board.RandomLane(), board.TotalRows - 1);
        scoop.Initialize(board, startIndex);
        return scoop;
    }

    public void SetSpeed(float speed) {
        scoopSpeed = speed;
    }

    private Color RandomFlavor() {
        return flavors[Random.Range(0,flavors.Length)].color;
    }
}
