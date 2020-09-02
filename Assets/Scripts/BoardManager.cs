using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BoardManager : MonoBehaviour
{

    public enum LeanTweenEvent {
        HorizontalSwipe
    }

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

    public ScoopManager scoopManager;

    public float laneWidth;
    public float rowHeight;

    public float lives = 3;
    public bool gameFrozen = false;

    public AudioSource audioSource;

    public bool devControls = false;

    [SerializeField]
    private TMP_Text livesCounter;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private Tutorial tutorial;

    [SerializeField]
    private Cone conePrefab;

    private List<Cone> cones = new List<Cone>();
    
    private int currentConeIndex;

    private Cone CurrentCone { 
        get {
            return cones[currentConeIndex];
        }
    }

    private LTDescr coneTween;

    private bool handlingSwipe = false;

    public static event Action FreezeGame = delegate { };
    public static event Action UnfreezeGame = delegate { };

    private void Awake()
    {
        LeanTween.LISTENERS_MAX = 25;
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

        cones.Add(Instantiate(conePrefab, transform.position, transform.rotation) as Cone);
        cones.Add(Instantiate(conePrefab, transform.position, transform.rotation) as Cone);
        foreach(Cone cone in cones) {
            cone.SetBoard(this);
            cone.Hide();
        }
        currentConeIndex = 0;
        CurrentCone.Show();

        audioSource = gameObject.AddComponent<AudioSource>();
        livesCounter.text = "" + lives;

        Gestures.OnSwipe += HandleSwipe;
        Gestures.SwipeEnded += EndSwipe;

        BoardManager.FreezeGame += OnFreezeGame;
        BoardManager.UnfreezeGame += OnUnfreezeGame;

    }

    private void Update() {
        if(devControls) {
            foreach(char c in Input.inputString) {
                if(c >= '0' && c <= '9') {
                    scoopManager.Dev_SpawnScoop((int)Char.GetNumericValue(c));
                }
            }
        }
    }

    private void IncrementCurrentConeIndex() {
        currentConeIndex = (currentConeIndex + 1) % cones.Count;
    }
    
    private void SwapCones() {
        CurrentCone.Hide();
        IncrementCurrentConeIndex();
        CurrentCone.Show();
    }

    public void HandleSwipe(SwipeInfo swipe)
    {
        if (handlingSwipe ||
           swipe.Direction == SwipeInfo.SwipeDirection.UP)
        {
            return;
        }

        if(swipe.Direction == SwipeInfo.SwipeDirection.DOWN) {
            // SwapCones();
            // handlingSwipe = true;
            return;
        }

        if(TutorialActive()) {
            if(CurrentTutorialStep() == Tutorial.TutorialStep.WaitingForSwipe) {
                if(swipe.Direction == SwipeInfo.SwipeDirection.RIGHT) {
                    UnfreezeGame();
                } else {
                    return;
                }
            } else {
                // Don't allow swiping unless the Tutorial Step is WaitingForSwipe
                return;
            }
        }
        handlingSwipe = true;
        if(CurrentCone != null) {
            coneTween = CurrentCone.MoveCone(swipe.Direction);
        }
    }
    
    public void EndSwipe() {
        handlingSwipe = false;
    }

    public void ScoopTapped(int index) {
        CurrentCone.HandleScoopTap(index);
    }

    public void GameOver() {
        FreezeGame();
        CurrentCone.GameOver();
    }

    public void Freeze() {
        FreezeGame();
    }

    public void Unfreeze() {
        UnfreezeGame();
    }

    public void DropScoop() {
        audioManager.Play(audioSource, audioManager.DropScoopAudio);
        lives--;
        livesCounter.text = "" + lives;
        if(lives == 0) {
            GameOver();
        }
    }

    public Tutorial.TutorialStep CurrentTutorialStep() {
        return tutorial.GetStep();
    }

    public bool TutorialActive() {
        return tutorial != null && tutorial.GetStep() != Tutorial.TutorialStep.Done;
    }

    private void OnFreezeGame() {
        gameFrozen = true;
        if(coneTween != null) {
            LeanTween.pause(coneTween.id);
        }
    }

    private void OnUnfreezeGame() {
        gameFrozen = false;
        if(coneTween != null) {
            LeanTween.resume(coneTween.id);
        }
    }

    public int RandomLane() {
        return UnityEngine.Random.Range(0, numberOfLanes);
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

    public int HitStack(Vector2Int scoop) {
        if(CurrentCone.ValidLane(scoop.x)) {
            if(scoop.y <= ConeStackHeight() && scoop.y >= ConeStackHeight() - 2) {
                return scoop.y;
            }
        }
        return -1;
    }

    public void AddScoopToCone(Scoop scoop) {
        CurrentCone.AddScoop(scoop);
    }

    public int ConeLane() {
        return CurrentCone.Lane();
    }

    public int ConeStackHeight() {
        return CurrentCone.StackHeight();
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
