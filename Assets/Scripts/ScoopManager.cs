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

    public float speed;
    [SerializeField]
    private float spawnDelay;

    private float timeUntilNextSpawn;
    private ScoopSpawner spawner;

    public bool spawning;

    public Scoop scoopPrefab;
    private bool handlingSwipe;

    private void ControlSpawner(SwipeInfo swipe) {
        if(handlingSwipe) {
            return;
        }
        handlingSwipe = true;
        switch(swipe.Direction) {
            case SwipeInfo.SwipeDirection.UP:
                spawning = false;
                break;
            case SwipeInfo.SwipeDirection.DOWN:
                if(spawning) {
                    speed++;
                } else {
                    speed = startScoopSpeed;
                }
                spawner.SetSpeed(speed);
                spawning = true;
                break;
        }
    }

    void OnSwipeEnd() {
        handlingSwipe = false;
    }

    void Start()
    {
        if(board != null) {
            board.scoopManager = this;
        }
        spawner = new ScoopSpawner(board, flavors, startScoopSpeed);
        Gestures.OnSwipe += ControlSpawner;
        Gestures.SwipeEnded += OnSwipeEnd;
    }

    void Update()
    {
        if(spawning && !board.gameEnded) {
            if (timeUntilNextSpawn <= 0) {
                timeUntilNextSpawn = spawnDelay;
                Scoop scoop = Instantiate(scoopPrefab, transform.position, transform.rotation) as Scoop;
                spawner.SpawnScoop(scoop);
            }
            timeUntilNextSpawn -= Time.deltaTime;
        }    
    }

}
