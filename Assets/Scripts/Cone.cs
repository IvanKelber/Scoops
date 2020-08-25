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
    private AudioManager audioManager;

    private AudioSource audioSource;

    [SerializeField]
    private RenderQuad renderQuad;

    private Stack<Scoop> scoopStack = new Stack<Scoop>();

    private int comboMultiplier = 0;
    private float points = 0;
   
    Dictionary<Color, int> colorMap = new Dictionary<Color, int>();

    private void Start()
    {
        horizontalLerp = GetComponent<Lerp>();
        currentIndex = new Vector2Int(1, 0); // Start in middle lane

        renderQuad = GetComponent<RenderQuad>();
        renderQuad.laneWidth = board.laneWidth;
        renderQuad.rowHeight = board.rowHeight;
        renderQuad.Render(transform.position);
        transform.position = board.GetPosition(currentIndex);

        audioSource = gameObject.AddComponent<AudioSource>();

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
        if(!board.gameEnded) {
            Vector3 velocity = horizontalLerp.CalculateMovement();
            transform.Translate(velocity);
        }
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

    public void AddScoop(Scoop scoop) {
        if(scoop.scoopStack != null) {
            return;
        }
        scoop.CalculateConsecutiveFlavors(scoopStack);
        scoopStack.Push(scoop);
    
        if(CheckMatch()) {
            StartCoroutine(HandleMatch(true));
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
        comboMultiplier++;
        yield return new WaitForSeconds(.3f);
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
        if(!board.gameEnded) 
            StartCoroutine(PopScoops(index));
    }
    
    private IEnumerator PopScoops(int index) {
        Queue<Scoop> scoops = new Queue<Scoop>();
        int popCount = scoopStack.Count;
        for(int i = 0; i < popCount - index; i++) {
            int popHeight = i + index + 1;
            Scoop scoop = scoopStack.Pop();
            scoops.Enqueue(scoop);
            scoop.MoveToIndex(new Vector2Int(Lane(), popHeight));
        }
        audioManager.Play(audioSource, audioManager.SwitchScoopsAudio);
        // Debug_ScoopList("ScoopStack after popping: ", scoopStack);
        // Debug_ScoopList("Scoops List: ", scoops);
        yield return StartCoroutine(AddScoopsToStack(scoops));
        // Debug_ScoopList("ScoopStack after adding: ", scoopStack);
        if(CheckMatch()) {
            yield return StartCoroutine(HandleMatch(false));
        }
        if(points == 0) {
            points = 1;
        }
        PointsManager.AddPoints(PointsManager.CalculatePoints(points, comboMultiplier));
        comboMultiplier = 0;
        points = 0;
    }

    private IEnumerator AddScoopsToStack(Queue<Scoop> scoops) {
        while(scoops.Count > 0) {
            Color currentFlavor = GetTopFlavor();
            if(scoops.Peek().flavor == currentFlavor) {
                PutScoopOnStack(scoops.Dequeue());
            } else {
                if(CheckMatch()) {
                    yield return StartCoroutine(HandleMatch(false));
                } else {
                    PutScoopOnStack(scoops.Dequeue());
                }
            }
        }
    }

    private Color GetTopFlavor() {
        if(scoopStack.Count == 0) {
            return Color.white;
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
        return lane == currentIndex.x || lane == lastIndex.x;
    }

    public void ClearStack()
    {
        for (int i = 0; i < scoopStack.Count; i++)
        {
            scoopStack.Pop().MeltScoop();
        }
    }

    public void Debug_ScoopList(string intro, System.Collections.Generic.IEnumerable<Scoop> list) {
        string debugstring = "scoop stack: [";

        foreach(Scoop scoop in list) {
            if(!colorMap.ContainsKey(scoop.flavor))
                colorMap.Add(scoop.flavor, colorMap.Count);
            debugstring += "C#" + colorMap[scoop.flavor] + ", ";
        }
        debugstring += "]";
        Debug.Log(intro + debugstring);
        

    }

}
