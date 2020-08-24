using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
[RequireComponent(typeof(Lerp))]
public class Scoop : MonoBehaviour
{

    private Lerp verticalLerp;
    private Lerp horizontalLerp;
    private bool handlingSwipe = false;
    private Vector2Int currentIndex;

    public Color flavor;

    private BoardManager board;
    
    public RenderQuad renderQuad;

    public ScoopIndicator scoopIndicator;

    public static event Action<int> ScoopTapped = delegate {};

    private float scoopMeltDelay = .1f;

    [HideInInspector]
    public int ConsecutiveFlavorScoops;

    public Stack<Scoop> scoopStack;
    public List<Scoop> FlyingScoops;

    private BoxCollider2D collider;

    //Should be called before scoop is part of stack
    private int DetermineConsecutiveFlavorScoops() {
        if(scoopStack.Count == 0 || scoopStack.Peek().flavor != this.flavor) {
            return 1; // Only one of this flavor consecutively
        }
        return scoopStack.Peek().ConsecutiveFlavorScoops + 1;
    }

    public int CalculateConsecutiveFlavors(Stack<Scoop> stack) {
        this.scoopStack = stack;
        this.ConsecutiveFlavorScoops = DetermineConsecutiveFlavorScoops();
        return ConsecutiveFlavorScoops;
    }
    private void Awake() {
        verticalLerp = GetComponent<Lerp>();
        verticalLerp.ReachedPoint += CheckCollisions;

    }

    private void Update() {
        if(!board.gameEnded) {
            Vector3 velocity = horizontalLerp.CalculateMovement() + verticalLerp.CalculateMovement();
            transform.Translate(velocity);
        }
    }

    public void Initialize(BoardManager board, Vector2Int currentIndex) {
        this.board = board;
        this.currentIndex = currentIndex;
        horizontalLerp = gameObject.AddComponent<Lerp>();

        renderQuad = GetComponent<RenderQuad>();
        renderQuad.laneWidth = board.laneWidth;
        renderQuad.rowHeight = board.rowHeight;
        renderQuad.SetColor(this.flavor);
        renderQuad.Render(transform.position);

        transform.position = board.GetPosition(currentIndex);

        collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector3(renderQuad.laneWidth/2.5f, renderQuad.rowHeight/2.5f, 0);

        scoopIndicator = (ScoopIndicator) GetComponentsInChildren<ScoopIndicator>()[0];
        scoopIndicator.SetIncomingFlavor(flavor);
        scoopIndicator.SetPosition(board.GetPosition(new Vector2Int(currentIndex.x, board.numberOfRows - 1)));

        CheckCollisions();
    }

    private bool HitFloor() {
        return currentIndex.y == 0;
    }

    private int HitStack() {
        return board.HitStack(currentIndex);
    }

    private bool HitMiddleStack() {
        return currentIndex.x == board.ConeLane() && currentIndex.y < board.ConeStackHeight() - 1;
    }

    // Checks collisions when a scoop has reached a new index
    private void CheckCollisions() {
        if(scoopStack != null) {
            return;
        }
        int index = HitStack();
        if(index != -1) {
            // We've hit the stack

          
            board.AddScoopToCone(this);
            if(index != board.ConeStackHeight()) {
                MoveToIndex(new Vector2Int(board.ConeLane(), board.ConeStackHeight() - 1));
            }
            Gestures.OnSwipe += HandleSwipe;
            Gestures.SwipeEnded += EndSwipe;
            horizontalLerp.speed = board.GetHorizontalLerpSpeed();
            
        } else if(HitFloor() || HitMiddleStack()) {
            board.lives--;
            if(board.lives == 0) {
                board.GameOver();
            }
            Destroy(this.gameObject);
        } else { 
            // Fall
            if(currentIndex.y <= board.numberOfRows) {
                // Destroy scoop indicator
                scoopIndicator.gameObject.SetActive(false);
            } else {
                scoopIndicator.SetPosition(board.GetPosition(new Vector2Int(currentIndex.x, board.numberOfRows - 1)));
                scoopIndicator.gameObject.SetActive(true);
            }
            Fall();
        }
    }

    private void HandleSwipe(SwipeInfo swipe) {
        if(handlingSwipe || 
           swipe.Direction == SwipeInfo.SwipeDirection.UP || 
           swipe.Direction == SwipeInfo.SwipeDirection.DOWN) {
            return;
        }

        handlingSwipe = true;
        Vector3 currentPosition = board.GetPosition(currentIndex);
        Vector2Int nextIndex = board.GetNextIndex(currentIndex, swipe.Direction);
        Vector3 nextPosition = board.GetPosition(nextIndex);
        if(horizontalLerp.DoLerp(currentPosition, nextPosition)) {
            currentIndex = nextIndex;
        }
    }

    public void RemoveInputHandlers() {
        Gestures.OnSwipe -= HandleSwipe;
        Gestures.SwipeEnded -= EndSwipe; 
    }

    private void EndSwipe() {
        handlingSwipe = false;
    }

    public void MeltScoop(AudioSource audioSource, AudioEvent meltEvent) {
        StartCoroutine(Melt(audioSource, meltEvent));
    }


    IEnumerator Melt(AudioSource audioSource, AudioEvent meltEvent) {
        meltEvent.Play(audioSource);
        yield return new WaitForSeconds(scoopMeltDelay);
        Destroy(this.gameObject);
    }


    private void OnDestroy() {
        scoopStack = null;
        FlyingScoops.Remove(this);
        RemoveInputHandlers();
    }

    public void SetSpeed(float speed) {
        verticalLerp.speed = speed;
    }
    
    public void MoveToIndex(Vector2Int index) {
        if(verticalLerp.DoLerp(board.GetPosition(currentIndex), board.GetPosition(index))) {
            currentIndex = index;
        };
    }


    private void Fall() {
        Vector2Int nextIndex = board.GetNextIndex(currentIndex, SwipeInfo.SwipeDirection.DOWN);
        if(verticalLerp == null) {
            return;
        } 
        if(verticalLerp.DoLerp(board.GetPosition(currentIndex), board.GetPosition(nextIndex))) {
            currentIndex = nextIndex;
        }
    }

    public void SetFlavor(Color flavor) {
        this.flavor = flavor;
    }


    public void OnMouseUpAsButton() {
        if(scoopStack != null)
            ScoopTapped(currentIndex.y - 1); // The index of the scoop within the stack
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0,1,0,.5f);
        if(collider != null) {
            Gizmos.DrawCube(collider.offset, collider.size);
        }
        
    }

    
}
