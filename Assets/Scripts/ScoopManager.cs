using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopManager : MonoBehaviour
{
 
    private List<Scoop> flyingScoops = new List<Scoop>();

    public BoardManager board;
    public Flavor[] flavors;
    
    [SerializeField]
    private float startScoopSpeed;
    [SerializeField]
    private float spawnDelay;

    private float timeUntilNextSpawn;
    private ScoopSpawner spawner;

    public bool spawning;

    public Scoop scoopPrefab;

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

    void Start()
    {
        spawner = new ScoopSpawner(board, flavors, startScoopSpeed);
    }

    void Update()
    {
        if(spawning) {
            if (timeUntilNextSpawn <= 0) {
                timeUntilNextSpawn = spawnDelay;
                Scoop scoop = Instantiate(scoopPrefab, transform.position, transform.rotation) as Scoop;
                spawner.SpawnScoop(scoop);
            }
            timeUntilNextSpawn -= Time.deltaTime;
        }    
    }

}
