﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DentedPixel;

public class Cone : MonoBehaviour
{
    private bool handlingSwipe = false;

    private Vector2Int currentIndex;

    [SerializeField]
    private BoardManager board;
    [SerializeField]
    private AudioManager audioManager;

    private AudioSource audioSource;

    [SerializeField]
    private RenderQuad renderQuad;

    private Stack<Scoop> scoopStack = new Stack<Scoop>();

    private int comboMultiplier = 0;
    private float points = 0;
   
    private bool poppingScoops = false;

    [SerializeField]
    private float emptyConeBonus;

    [SerializeField]
    private float handleMatchDelay = .3f;

    private bool handlingMatch;
    private Coroutine handleMatchRoutine;

    private Queue<Scoop> poppedScoops = new Queue<Scoop>();


    private void Start()
    {
        currentIndex = new Vector2Int(1, 0); // Start in middle lane

        renderQuad = GetComponent<RenderQuad>();
        renderQuad.laneWidth = board.laneWidth;
        renderQuad.rowHeight = board.rowHeight;
        renderQuad.Render(transform.position);
        transform.position = board.GetPosition(currentIndex);

        audioSource = gameObject.AddComponent<AudioSource>();

    }

    public void SetBoard(BoardManager board) {
        this.board = board;
    }

    public void Hide() {
        foreach(Scoop scoop in scoopStack) {
            scoop.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void Show() {
        foreach(Scoop scoop in scoopStack) {
            scoop.gameObject.SetActive(true);
        }
        gameObject.SetActive(true);
    }

    public LTDescr MoveCone(SwipeInfo.SwipeDirection direction) {
        if(board.gameFrozen) {
            return null;
        }
        Vector2Int nextIndex = board.GetNextIndex(currentIndex, direction);
        Vector3 nextPosition = board.GetPosition(nextIndex);
        LeanTween.dispatchEvent((int) BoardManager.LeanTweenEvent.HorizontalSwipe, direction);
        return LeanTween.move(gameObject, nextPosition, .1f).setOnComplete(() => {
            currentIndex = nextIndex;
        });
    }

    public void AddScoop(Scoop scoop) {
        if(scoop.scoopStack != null) {
            return;
        }
        scoop.CalculateConsecutiveFlavors(scoopStack);
        scoopStack.Push(scoop);
    
        if(CheckMatch()) {
            if(handlingMatch && GetTopFlavor() == scoop.flavor) {
                StopCoroutine(handleMatchRoutine);
            }
            handleMatchRoutine = StartCoroutine(HandleMatch(true));
        } else if (StackHeight() == board.numberOfRows + 1)
        {
            // Allow the user to be quick if their stack reaches the very top.
            // Also allows the user to catch falling scoops to make a match of 3 at the very top
            board.GameOver();
        }
        // Debug_ScoopList("ScoopStack after merging flying stack: ", scoopStack);
    }

    private void PutScoopOnStack(Scoop scoop) {
        scoop.CalculateConsecutiveFlavors(scoopStack);
        scoopStack.Push(scoop);
        scoop.MoveToIndex(new Vector2Int(Lane(), StackHeight() - 1));
    }

    public void GameOver() {
        StartCoroutine(IGameOver());
    }
    IEnumerator IGameOver() {
        yield return audioManager.PlayAndWait(audioSource, audioManager.GameOverAudio);
        int pops = StackHeight() - 1;
        for(int i = 0; i < pops; i++) {
            audioManager.Play(audioSource, audioManager.DropScoopAudio);
            scoopStack.Pop().MeltScoop();
            yield return new WaitForSeconds(.1f);
        }
        yield return CrumbleCone();

        SceneState.LoadScene(0); // Reload game scene for debug purposes
    }

    private IEnumerator HandleMatch(bool offTheTop)
    {
        handlingMatch = true;
        comboMultiplier++;
        yield return new WaitForSeconds(handleMatchDelay);
        handlingMatch = false;
        audioManager.Play(audioSource, audioManager.ScoopsMatchAudio);
        int matchingScoops = scoopStack.Peek().ConsecutiveFlavorScoops;
        points += PointsManager.GetPointsFromMatch(matchingScoops);
        for (int i = 0; i < matchingScoops; i++)
        {
            scoopStack.Pop().MeltScoop();
        }
        if(offTheTop) {
            PointsManager.AddPoints(PointsManager.CalculatePoints(points, comboMultiplier));
            comboMultiplier = 0;
            points = 0;
            if(scoopStack.Count == 0) {
                // Apply emptyCone Bonus
                PointsManager.AddPoints(emptyConeBonus * board.scoopManager.speed);

            }
        }
    }

    private WaitForSeconds CrumbleCone() {
        return new WaitForSeconds(1);    
    }

    public void HandleScoopTap(int index) {
        if(board.TutorialActive()) {
            if(board.CurrentTutorialStep() == Tutorial.TutorialStep.WaitingForTap) {
                if(index == 1) {
                    board.Unfreeze();
                }
            } else {
                // Don't allow tapping during the tutorial unless it's the right time to tap
                return;
            }
        }
        if(!board.gameFrozen && !poppingScoops) {
            poppingScoops = true;
            StartCoroutine(PopScoops(index));
        }
    }
    
    private IEnumerator PopScoops(int index) {
        // Debug_ScoopList("Before popping: ",scoopStack);
        if(handlingMatch) {
            StopCoroutine(handleMatchRoutine);
            comboMultiplier = 0;
            yield return StartCoroutine(AddScoopsToStack(poppedScoops));
        }
        poppedScoops = new Queue<Scoop>();
        int popCount = scoopStack.Count;
        for(int i = 0; i < popCount - index; i++) {
            int popHeight = i + index + 1;
            Scoop scoop = scoopStack.Pop();
            poppedScoops.Enqueue(scoop);
            scoop.Pop(new Vector2Int(Lane(), popHeight));
        }
        audioManager.Play(audioSource, audioManager.SwitchScoopsAudio);
        // Debug_ScoopList("ScoopStack after popping: ", scoopStack);
        // Debug_ScoopList("Scoops List: ", scoops);
        yield return StartCoroutine(AddScoopsToStack(poppedScoops));
        // Debug_ScoopList("ScoopStack after adding: ", scoopStack);
        if(CheckMatch()) {
            handleMatchRoutine = StartCoroutine(HandleMatch(false));
            yield return handleMatchRoutine;

        }
        if(points == 0) {
            points = 1;
        }
        PointsManager.AddPoints(PointsManager.CalculatePoints(points, comboMultiplier));
        comboMultiplier = 0;
        points = 0;
        poppingScoops = false;
        if(scoopStack.Count == 0) {
            // Apply emptyConeBonus;
            PointsManager.AddPoints(emptyConeBonus * board.scoopManager.speed);
        }
    }

    private IEnumerator AddScoopsToStack(Queue<Scoop> scoops) {
        while(scoops.Count > 0) {
            Flavor currentFlavor = GetTopFlavor();
            if(scoops.Peek().flavor == currentFlavor) {
                PutScoopOnStack(scoops.Dequeue());
            } else {
                if(CheckMatch()) {
                    handleMatchRoutine = StartCoroutine(HandleMatch(false));
                    yield return handleMatchRoutine;
                } else {
                    PutScoopOnStack(scoops.Dequeue());
                }
            }
        }
    }

    private Flavor GetTopFlavor() {
        if(scoopStack.Count == 0) {
            return new Flavor(Color.white, "null flavor");
        }
        return scoopStack.Peek().flavor;
    }

    private bool CheckMatch()
    {
        return scoopStack.Count >= 3 && scoopStack.Peek().ConsecutiveFlavorScoops >= 3;
    }

    public int StackHeight()
    {
        return scoopStack.Count + 1; //Add one to account for the cone itself
    }

    public int Lane()
    {
        return currentIndex.x;
    }

    public bool ValidLane(int lane) {
        return lane == currentIndex.x;
    }

    public void ClearStack()
    {
        for (int i = 0; i < scoopStack.Count; i++)
        {
            scoopStack.Pop().MeltScoop();
        }
    }

    public void Debug_ScoopList(string intro, System.Collections.Generic.IEnumerable<Scoop> list) {
        string debugstring = "[";

        foreach(Scoop scoop in list) {
            debugstring += "("+ scoop.flavor.name + ", " + scoop.ConsecutiveFlavorScoops +"), ";
        }
        debugstring += "]";
        Debug.Log(intro + debugstring);

    }

}
