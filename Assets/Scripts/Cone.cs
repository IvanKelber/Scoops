using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Lerp))]
public class Cone : MonoBehaviour
{
    private bool handlingSwipe = false;

    private Vector2Int currentIndex;

    private Lerp horizontalLerp;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private RenderQuad renderQuad;

    private Stack<StackableScoop> scoopStack = new Stack<StackableScoop>();
   

    private void Start()
    {
        horizontalLerp = GetComponent<Lerp>();
        currentIndex = new Vector2Int(1, 0); // Start in middle lane

        renderQuad = GetComponent<RenderQuad>();
        renderQuad.Render(transform.position);
        transform.position = grid.GetPosition(currentIndex);

        // Add gesture listeners
        Gestures.OnSwipe += HandleSwipe;
        Gestures.SwipeEnded += EndSwipe;
        Gestures.ThreeTap += ClearStack;

        // Other event listeners
        Scoop.ScoopTapped += HandleScoopTap;
    }

    public float GetHorizontalLerpSpeed()
    {
        return horizontalLerp.speed;
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
        Vector3 currentPosition = grid.GetPosition(currentIndex);
        Vector2Int nextIndex = grid.GetNextIndex(currentIndex, swipe.Direction);
        Vector3 nextPosition = grid.GetPosition(nextIndex);
        if (horizontalLerp.DoLerp(currentPosition, nextPosition))
        {
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

        if (StackHeight() == grid.numberOfRows)
        {
            Debug.Log("GAME OVER");
            // Emit game over event
        }
        else
        {
            if (CheckMatch())
            {
                HandleMatch();
            }
        }
        return StackHeight();
    }

    private void HandleMatch()
    {
        int matchingScoops = scoopStack.Peek().ConsecutiveFlavorScoops;
        Debug.Log("Handling match of " + matchingScoops);
        for (int i = 0; i < matchingScoops; i++)
        {
            scoopStack.Pop().MeltScoop();
        }
    }

    private void HandleScoopTap(int index) {
        StartCoroutine(PopScoops(index, .1f));
    }
    
    private IEnumerator PopScoops(int index, float delay) {
        int popCount = scoopStack.Count;
        for(int i = 0; i < popCount - index; i++) {
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

    public void ClearStack()
    {
        for (int i = 0; i < scoopStack.Count; i++)
        {
            scoopStack.Pop().MeltScoop();
        }
    }

    public Queue<StackableScoop> GetAirboundScoops(int index) {
        Queue<StackableScoop> scoopQueue = new Queue<StackableScoop>();
        for(int i = 0; i < scoopStack.Count - index; i++) {
            StackableScoop stackableScoop = scoopStack.Pop();
            // stackableScoop.scoop.AnimateFlip();
            scoopQueue.Enqueue(stackableScoop);
        }
        Debug.Log("ScoopQueue Count: " + scoopQueue.Count);
        return scoopQueue;
    }

}
