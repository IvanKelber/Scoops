using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{

    public int numberOfLanes;
    public int numberOfRows;
    public Camera cam;

    public int numberOfOffscreenRows;
    public int TotalRows {
        get {
            return numberOfOffscreenRows + numberOfRows;
        }
    }
    public Vector3[,] grid;

    public Cone cone;

    public ScoopManager scoopManager;

    public float laneWidth;
    public float rowHeight;

    public float lives = 3;
    public bool gameFrozen = false;

    private float timeUntilScoreUpdate;
    private float scoreUpdateDelay = 1f;

    public AudioSource audioSource;

    public bool devControls = false;

    [SerializeField]
    private TMP_Text livesCounter;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private Tutorial tutorial;


    private bool handlingSwipe = false;

    private void Awake()
    {
        PointsManager.Points = 0;
        if(cam == null) {
            Debug.LogError("Cannot calculate grid bounds because camera is missing.");
            return;
        }
        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;
        laneWidth = screenWidth/numberOfLanes;
        rowHeight = screenHeight/numberOfRows;
        Bounds cameraBounds = new Bounds(cam.transform.position, new Vector3(screenWidth, screenHeight, 0));
        grid = new Grid(TotalRows, numberOfRows, numberOfLanes, rowHeight, laneWidth, cameraBounds).grid;

        if(scoopManager != null) {
            scoopManager.board = this;
        } else {
            Debug.LogError("ScoopManager is null for BoardManager");
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        livesCounter.text = "" + lives;

        Gestures.OnSwipe += HandleSwipe;
        Gestures.SwipeEnded += EndSwipe;

    }
    
    public void HandleSwipe(SwipeInfo swipe)
    {
        if (handlingSwipe ||
           swipe.Direction == SwipeInfo.SwipeDirection.UP ||
           swipe.Direction == SwipeInfo.SwipeDirection.DOWN)
        {
            return;
        }
        if(TutorialActive()) {
            if(CurrentTutorialStep() == Tutorial.TutorialStep.Swipe) {
                if(swipe.Direction == SwipeInfo.SwipeDirection.RIGHT) {
                    UnFreezeGame();
                    AlertTutorial(Tutorial.TutorialStep.Tap);
                } else {
                    return;
                }
            } else {
                // Don't allow swiping unless the Tutorial Step is Swipe
                return;
            }
        }
        handlingSwipe = true;
        cone.MoveCone(swipe.Direction);
    }
    
    public void EndSwipe() {
        handlingSwipe = false;
    }

    public void ScoopTapped(int index) {
        cone.HandleScoopTap(index);
    }

    public void GameOver() {
        FreezeGame();
        cone.GameOver();
    }

    public void DropScoop() {
        audioManager.Play(audioSource, audioManager.DropScoopAudio);
        lives--;
        livesCounter.text = "" + lives;
        if(lives == 0) {
            GameOver();
        }
    }

    public void AlertTutorial(Tutorial.TutorialStep newStep) {
        tutorial.SetStep(newStep);
        if(tutorial.GetStep() == Tutorial.TutorialStep.Done) {
            scoopManager.spawning = true;
            tutorial.Destroy();
        }
    }

    public Tutorial.TutorialStep CurrentTutorialStep() {
        return tutorial.GetStep();
    }

    public bool TutorialActive() {
        return tutorial != null;
    }

    public void FreezeGame() {
        gameFrozen = true;
    }

    public void UnFreezeGame() {
        gameFrozen = false;
    }

    public int RandomLane() {
        return Random.Range(0, numberOfLanes);
    }


    public Vector3 GetPosition(int lane, int row) {
        return grid[lane,row];
    }

    public Vector3 GetPosition(Vector2Int index) {
        return GetPosition(index.x, index.y);
    }

    public Vector3 GetScreenPosition(int lane, int row) {
        return cam.WorldToScreenPoint(grid[lane,row]);
    }

    public Vector3 GetScreenPosition(Vector2Int index) {
        return GetScreenPosition(index.x, index.y);
    }

    public Vector2Int GetNextIndex(Vector2Int index, SwipeInfo.SwipeDirection direction) {
        Vector2Int nextIndex = index;
        if(direction == SwipeInfo.SwipeDirection.LEFT) {
            nextIndex.x = Mathf.Clamp(index.x - 1, 0, numberOfLanes-1);
        } else if(direction == SwipeInfo.SwipeDirection.RIGHT) {
            nextIndex.x = Mathf.Clamp(index.x + 1, 0, numberOfLanes-1);
        }

        if(direction == SwipeInfo.SwipeDirection.DOWN) {
            nextIndex.y = Mathf.Clamp(index.y - 1, 0, TotalRows - 1);
        } else if(direction == SwipeInfo.SwipeDirection.UP) {
            nextIndex.y = Mathf.Clamp(index.y + 1, 0, TotalRows - 1);
        }

        return nextIndex;
    }
    public float GetHorizontalLerpSpeed()
    {
        return cone.horizontalLerp.speed;
    }

    public int HitStack(Vector2Int scoop) {
        if(cone.ValidLane(scoop.x)) {
            if(scoop.y <= ConeStackHeight() && scoop.y >= ConeStackHeight() - 2) {
                return scoop.y;
            }
        }
        return -1;
    }

    public void AddScoopToCone(Scoop scoop) {
        cone.AddScoop(scoop);
    }

    public int ConeLane() {
        return cone.Lane();
    }

    public int ConeStackHeight() {
        return cone.StackHeight();
    }

    private void OnDrawGizmos() {
        if(grid != null) {
            for(int i = 0; i < numberOfLanes; i++) {
                for(int j = 0; j < TotalRows; j++) {
                    Gizmos.DrawWireSphere(GetPosition(i,j),.01f);
                }
            }
        }

    }

}
