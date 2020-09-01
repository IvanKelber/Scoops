using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Scoop : MonoBehaviour
{

    public Vector2Int currentIndex;

    public Flavor flavor;

    private BoardManager board;
    
    public RenderQuad renderQuad;

    public ScoopIndicator scoopIndicator;

    private float scoopMeltDelay = .1f;

    [HideInInspector]
    public int ConsecutiveFlavorScoops;

    public Stack<Scoop> scoopStack;

    private BoxCollider2D collider;

    private Vector3 tapDown;

    private Vector3 velocity;
    private LTDescr verticalScoopTween;

    //Should be called before scoop is part of stack
    private int DetermineConsecutiveFlavorScoops() {
        if(scoopStack.Count == 0 || (scoopStack.Peek().flavor != this.flavor)) {
            return 1; // Only one of this flavor consecutively
        }
        return scoopStack.Peek().ConsecutiveFlavorScoops + 1;
    }

    public int CalculateConsecutiveFlavors(Stack<Scoop> stack) {
        this.scoopStack = stack;
        this.ConsecutiveFlavorScoops = DetermineConsecutiveFlavorScoops();
        return ConsecutiveFlavorScoops;
    }

    private void Update() {
        if(scoopIndicator != null) {
            if(currentIndex.y <= board.numberOfRows) {
                // Destroy scoop indicator
                Destroy(scoopIndicator.gameObject);
            } else {
                scoopIndicator.SetPosition(board.GetPosition(new Vector2Int(currentIndex.x, board.numberOfRows - 1)));
                scoopIndicator.gameObject.SetActive(true);
            }
        }
    }

    public void Initialize(BoardManager board, Vector2Int currentIndex) {
        this.board = board;
        this.currentIndex = currentIndex;

        renderQuad = GetComponent<RenderQuad>();
        renderQuad.laneWidth = board.laneWidth;
        renderQuad.rowHeight = board.rowHeight;
        renderQuad.SetColor(this.flavor.color);
        renderQuad.Render(transform.position);

        transform.position = board.GetPosition(currentIndex);

        collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector3(renderQuad.laneWidth/2.5f, renderQuad.rowHeight/2.5f, 0);

        scoopIndicator = (ScoopIndicator) GetComponentsInChildren<ScoopIndicator>()[0];
        scoopIndicator.SetIncomingFlavor(flavor);
        scoopIndicator.SetPosition(board.GetPosition(new Vector2Int(currentIndex.x, board.numberOfRows - 1)));

        BoardManager.FreezeGame += FreezeScoop;
        BoardManager.UnfreezeGame += UnfreezeScoop;
    }

    public void Start() {
        CheckCollisions();
    }

    public void MoveScoopHorizontally(LTEvent e) {
        SwipeInfo.SwipeDirection direction = (SwipeInfo.SwipeDirection) e.data;
        Vector2Int nextIndex = board.GetNextIndex(currentIndex, direction);
        Vector3 nextPosition = board.GetPosition(nextIndex);
        LeanTween.move(gameObject, nextPosition,.1f).setOnComplete(() => {
            currentIndex = nextIndex;
        });
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
            // if(index != board.ConeStackHeight()) {
            //     MoveToIndex(new Vector2Int(board.ConeLane(), board.ConeStackHeight() - 1));
            // }
            LeanTween.addListener(gameObject, 0, MoveScoopHorizontally);
            
        } else if(HitFloor() || HitMiddleStack()) {
            board.DropScoop();
            Destroy(this.gameObject);
        } else { 
            Fall();
        }
    }

    public void MeltScoop() {
        StartCoroutine(Melt());
    }


    IEnumerator Melt() {
        yield return new WaitForSeconds(scoopMeltDelay);
        Destroy(this.gameObject);
    }

    private void FreezeScoop() {
        if(verticalScoopTween != null) {
            LeanTween.pause(verticalScoopTween.id);
        }
    }

    private void UnfreezeScoop() {
        if(verticalScoopTween != null) {
            LeanTween.resume(verticalScoopTween.id);
        }
    }

    private void OnDestroy() {
        scoopStack = null;
        LeanTween.removeListener(gameObject, 0, MoveScoopHorizontally);
        BoardManager.FreezeGame -= FreezeScoop;
        BoardManager.UnfreezeGame -= UnfreezeScoop;
    }
    
    public void MoveToIndex(Vector2Int index) {
        MoveScoopVertically(board.GetPosition(index), .05f).setOnComplete( () => {
            currentIndex = index;
        });
    }


    private void Fall() {
        Vector2Int nextIndex = board.GetNextIndex(currentIndex, SwipeInfo.SwipeDirection.DOWN);
        verticalScoopTween = MoveScoopVertically(board.GetPosition(nextIndex), .2f).setOnComplete(() => {
            currentIndex = nextIndex;
            CheckCollisions();
        });
    }

    public LTDescr MoveScoopVertically(Vector3 finalPosition, float duration) {
        return LeanTween.move(gameObject, finalPosition, duration);
    }

    public void SetFlavor(Flavor flavor) {
        this.flavor = flavor;
    }

    private void OnMouseDown() {
        tapDown = Input.mousePosition;
    }

    private void OnMouseUpAsButton() {
        if(scoopStack != null && Vector3.Distance(tapDown, Input.mousePosition) < Gestures.minSwipeDistance)
            board.ScoopTapped(currentIndex.y - 1); // The index of the scoop within the stack
        else if(board.devControls) {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0,1,0,.5f);
        if(collider != null) {
            Gizmos.DrawCube(collider.offset, collider.size);
        }
        
    }

    
}
