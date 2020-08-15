using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopSpawner : MonoBehaviour
{

    public Scoop scoopPrefab;
    public Grid grid;
    public Cone cone;
    public float speed;

    public Color[] flavors;
    // Update is called once per frame

    public bool spawning = false;

    public float timeBetweenSpawn;
    private float timeUntilNextSpawn;

    private void Start() {
        Gestures.OnSwipe += ControlSpawner;
    }

    private void ControlSpawner(SwipeInfo swipe) {
        switch(swipe.Direction) {
            case SwipeInfo.SwipeDirection.UP:
                spawning = false;
                break;
            case SwipeInfo.SwipeDirection.DOWN:
                spawning = true;
                break;
        }
    }

    void Update()
    {
        if(spawning) {
            if (timeUntilNextSpawn <= 0) {
                timeUntilNextSpawn = timeBetweenSpawn;
                ChooseScoop();
            }
            timeUntilNextSpawn -= Time.deltaTime;
        }
    }

    private void ChooseScoop() {
        Scoop scoop = Instantiate(scoopPrefab, transform.position, transform.rotation) as Scoop;
        scoop.SetFlavor(RandomFlavor());
        scoop.SetSpeed(speed);
        Vector2Int startIndex = new Vector2Int(Random.Range(0,3), grid.TotalRows - 1);
        scoop.Initialize(grid, startIndex, cone);
    }

    private Color RandomFlavor() {
        return flavors[Random.Range(0,flavors.Length)];
    }
}
