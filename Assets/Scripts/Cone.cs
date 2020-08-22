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

    private Stack<Scoop> scoopStack = new Stack<Scoop>();
   
    public static event Action OnGameEnded = delegate {};
    Dictionary<Color, int> colorMap = new Dictionary<Color, int>();

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

    public void AddScoop(Scoop scoop) {
        if(scoop.scoopStack == scoopStack) {
            // Scoop already added to stack from previous AddFlyingStack
            return;
        }
        if(scoop.FlyingScoops.Count > 0 && scoop == scoop.FlyingScoops[0]) {
            // Add any scoops that are the same flavor as the current top flavor
            if(!AddFlyingStack(scoop.FlyingScoops)) {
                // Must add the original scoop
                scoop.CalculateConsecutiveFlavors(scoopStack);
                scoopStack.Push(scoop);
            }
        } else {
            // Add a scoop that is not part of a flying stack
            scoop.CalculateConsecutiveFlavors(scoopStack);
            scoopStack.Push(scoop);
        }
        
        if(CheckMatch()) {
            HandleMatch();
        } else if (StackHeight() == board.numberOfRows)
        {
            Debug.Log("GAME OVER");
            StartCoroutine(GameOver());
        }
        // Debug_ScoopList("ScoopStack after merging flying stack: ", scoopStack);
    }

    private bool AddFlyingStack(List<Scoop> flyingScoops) {
        // Debug_ScoopList("FlyingScoops: ", flyingScoops);
        // Debug_ScoopList("ScoopStack: ", scoopStack);
        int scoopsAdded = 0;
        if(scoopStack.Count == 0) {
            return false;
        }

        Color currentTopFlavor = scoopStack.Peek().flavor;

        while(flyingScoops[scoopsAdded].flavor == currentTopFlavor) {
            flyingScoops[scoopsAdded].CalculateConsecutiveFlavors(scoopStack);
            scoopStack.Push(flyingScoops[scoopsAdded]);
            scoopsAdded++;
        }
        return scoopsAdded > 0;
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
        List<Scoop> scoops = new List<Scoop>();
        int popCount = scoopStack.Count;
        for(int i = 0; i < popCount - index; i++) {
            popScoopAudio.Play(audioSource);
            int popHeight = popCount + i + 1;
            Scoop scoop = scoopStack.Pop();
            scoop.RemoveInputHandlers();
            scoop.scoopStack = null;
            scoop.MoveToIndex(new Vector2Int(Lane(), popHeight));
            scoops.Add(scoop);
            scoop.FlyingScoops = scoops;
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
