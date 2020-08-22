using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Lerp))]
public class Cone : MonoBehaviour
{
    private bool handlingSwipe = false;

    private Vector2Int currentIndex;
    private Vector2Int lastIndex;

    public Lerp horizontalLerp;

    [SerializeField]
    private BoardManager board;

    [SerializeField]
    private RenderQuad renderQuad;

    [SerializeField]
    private AudioEvent popScoopAudio;

    [SerializeField]
    private AudioEvent meltScoopAudio;

    [SerializeField]
    private AudioEvent gameOverJingle;
    [SerializeField]
    private AudioEvent losePopScoopAudio;


    private AudioSource audioSource;

    private Stack<StackableScoop> scoopStack = new Stack<StackableScoop>();
   
    public static event Action OnGameEnded = delegate {};

    private void Start()
    {
        horizontalLerp = GetComponent<Lerp>();
        audioSource = GetComponent<AudioSource>();
        currentIndex = new Vector2Int(1, 0); // Start in middle lane

        renderQuad = GetComponent<RenderQuad>();
        renderQuad.Render(transform.position);
        transform.position = board.GetPosition(currentIndex);

        // Add gesture listeners
        Gestures.OnSwipe += HandleSwipe;
        Gestures.SwipeEnded += EndSwipe;
        Gestures.ThreeTap += ClearStack;

        // Other event listeners
        Scoop.ScoopTapped += HandleScoopTap;
    }

    private void Update() {
        if(!handlingSwipe) {
            lastIndex = currentIndex;
        }
        Vector3 velocity = horizontalLerp.CalculateMovement();
        transform.Translate(velocity);
    }

    private void HandleSwipe(SwipeInfo swipe)
    {
        if (handlingSwipe ||
           swipe.Direction == SwipeInfo.SwipeDirection.UP ||
           swipe.Direction == SwipeInfo.SwipeDirection.DOWN)
        {
            return;
        }
        handlingSwipe = true;
        Vector3 currentPosition = board.GetPosition(currentIndex);
        Vector2Int nextIndex = board.GetNextIndex(currentIndex, swipe.Direction);
        Vector3 nextPosition = board.GetPosition(nextIndex);
        if (horizontalLerp.DoLerp(currentPosition, nextPosition))
        {
            lastIndex = currentIndex;
            currentIndex = nextIndex;
        }
    }

    private void EndSwipe()
    {
        handlingSwipe = false;
    }

    public int AddScoop(Scoop scoop)
    {
        return AddScoop(new StackableScoop(scoop, scoopStack));
    }

    private int AddScoop(StackableScoop scoop) {
        scoop.CalculateConsecutiveFlavors(scoopStack);
        scoopStack.Push(scoop);

        if(CheckMatch()) {
            HandleMatch();
        }

        else if (StackHeight() == board.numberOfRows)
        {
            Debug.Log("GAME OVER");
            StartCoroutine(GameOver());
        }
        return StackHeight();
    }

    IEnumerator GameOver() {
        OnGameEnded();
        yield return gameOverJingle.PlayAndWait(audioSource);
        int pops = StackHeight() - 1;
        for(int i = 0; i < pops; i++) {
            scoopStack.Pop().MeltScoop(audioSource, losePopScoopAudio);
            yield return new WaitForSeconds(.1f);
        }
        yield return CrumbleCone();

        SceneState.LoadScene(0); // Reload game scene for debug purposes
    }

    private void HandleMatch()
    {
        int matchingScoops = scoopStack.Peek().ConsecutiveFlavorScoops;
        Debug.Log("Handling match of " + matchingScoops);
        for (int i = 0; i < matchingScoops; i++)
        {
            scoopStack.Pop().MeltScoop(audioSource, meltScoopAudio);
        }
    }

    private WaitForSeconds CrumbleCone() {
        // remove gesture listeners
        Gestures.OnSwipe -= HandleSwipe;
        Gestures.SwipeEnded -= EndSwipe;
        Gestures.ThreeTap -= ClearStack;

        // Other event listeners
        Scoop.ScoopTapped -= HandleScoopTap;    
        return new WaitForSeconds(1);    
    }

    private void HandleScoopTap(int index) {
        StartCoroutine(PopScoops(index, .1f));
    }
    
    private IEnumerator PopScoops(int index, float delay) {
        int popCount = scoopStack.Count;
        for(int i = 0; i < popCount - index; i++) {
            popScoopAudio.Play(audioSource);
            int popHeight = popCount + i + 1;
            StackableScoop scoop = scoopStack.Pop();
            scoop.RemoveInputHandlers();
            scoop.MoveToIndex(new Vector2Int(Lane(), popHeight));
            yield return new WaitForSeconds(delay);
        }
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

    public bool ScoopValid(Vector2Int scoop) {
        if(scoop.x == currentIndex.x || scoop.x == lastIndex.x) {
            return scoop.y == StackHeight() || scoop.y == StackHeight() - 2;
        }
        return false;
    }

    public void ClearStack()
    {
        for (int i = 0; i < scoopStack.Count; i++)
        {
            scoopStack.Pop().MeltScoop(audioSource, meltScoopAudio);
        }
    }

    public Queue<StackableScoop> GetAirboundScoops(int index) {
        Queue<StackableScoop> scoopQueue = new Queue<StackableScoop>();
        for(int i = 0; i < scoopStack.Count - index; i++) {
            StackableScoop stackableScoop = scoopStack.Pop();
            scoopQueue.Enqueue(stackableScoop);
        }
        Debug.Log("ScoopQueue Count: " + scoopQueue.Count);
        return scoopQueue;
    }

}
