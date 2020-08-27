using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopManager : MonoBehaviour
{
 
    private List<Scoop> flyingScoops = new List<Scoop>();

    public BoardManager board;
    public Flavor[] flavors;

    [SerializeField]
    private int numberOfFlavors;
    
    [SerializeField]
    private float startScoopSpeed;

    public float speed;
    [SerializeField]
    private float startSpawnDelay;
    [SerializeField]
    private float spawnDelay;

    [SerializeField]
    private float startScoreUpdateDelay = 1;

    private float timeUntilScoreUpdate;

    private float timeUntilNextSpawn;

    public bool spawning;

    public Scoop scoopPrefab;
    private bool handlingSwipe;

    private int scoopsSpawned = 0;

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
        speed = startScoopSpeed;
        spawnDelay = startSpawnDelay;
        timeUntilScoreUpdate = startScoreUpdateDelay;

        if(board.devControls) {
            Gestures.OnSwipe += ControlSpawner;
            Gestures.SwipeEnded += OnSwipeEnd;
        }
    }

    void Update()
    {
        float scoreUpdateDelay = .5f/(startScoreUpdateDelay + speed - startScoopSpeed);
        if(spawning && !board.gameFrozen) {
            if (timeUntilNextSpawn <= 0) {
                timeUntilNextSpawn = spawnDelay;
                scoopsSpawned++;
                if(scoopsSpawned % 15 == 0) {
                    speed += .33f;
                    speed = Mathf.Clamp(speed, startScoopSpeed, startScoopSpeed*2f);
                }
                if(scoopsSpawned % 20 == 0) {
                    spawnDelay -= .33f;
                    spawnDelay = Mathf.Clamp(spawnDelay, 1.5f, startSpawnDelay);
                }
                SpawnRandomScoop(InstantiateScoop());
            }
            timeUntilNextSpawn -= Time.deltaTime;
            if(timeUntilScoreUpdate <= 0) {
                PointsManager.AddPoints(1);
                timeUntilScoreUpdate = scoreUpdateDelay;
            }
            timeUntilScoreUpdate -= Time.deltaTime;
        }
    }

    public Scoop InstantiateScoop() {
        Scoop scoop = Instantiate(scoopPrefab, transform.position, transform.rotation) as Scoop;
        return scoop;
    }

    private Flavor RandomFlavor() {
        return flavors[Random.Range(0,Mathf.Clamp(numberOfFlavors, 0, flavors.Length))];
    }

    public Scoop SpawnRandomScoop(Scoop scoop) {
        SetScoop(scoop, RandomFlavor(), speed, new Vector2Int(board.RandomLane(), board.TotalRows - 1));
        return scoop;
    }

    public void SetScoop(Scoop scoop, Flavor flavor, float speed, Vector2Int startIndex) {
        scoop.SetFlavor(flavor);
        scoop.SetSpeed(speed);
        scoop.Initialize(board, startIndex);
    }

}
