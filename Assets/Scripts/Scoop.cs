using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
[RequireComponent(typeof(Lerp))]
public class Scoop : MonoBehaviour
{

    private Lerp verticalLerp;
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
    private void Awake() {
        verticalLerp = GetComponent<Lerp>();
        verticalLerp.ReachedPoint += CheckCollisions;
    }

    private void Update() {
        if(!board.gameFrozen) {
            velocity += verticalLerp.CalculateMovement();
            transform.Translate(velocity);
        }
        if(scoopIndicator != null) {
            if(currentIndex.y <= board.numberOfRows) {
                // Destroy scoop indicator
                Destroy(scoopIndicator.gameObject);
            } else {
                scoopIndicator.SetPosition(board.GetPosition(new Vector2Int(currentIndex.x, board.numberOfRows - 1)));
                scoopIndicator.gameObject.SetActive(true);
            }
        }
        velocity = Vector3.zero;
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

        CheckCollisions();
    }

    public void MoveScoop(Vector3 movement, int newLane) {
        if(movement == Vector3.zero) {
            return;
        }
        velocity += movement;
        currentIndex.x = newLane;
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
            verticalLerp.speed = 15;
            
        } else if(HitFloor() || HitMiddleStack()) {
            board.DropScoop();
            Destroy(this.gameObject);
        } else { 
            // Fall
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


    private void OnDestroy() {
        scoopStack = null;
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

    public void SetFlavor(Flavor flavor) {
        this.flavor = flavor;
    }

    private void OnMouseDown() {
        tapDown = Input.mousePosition;
    }

    private void OnMouseUpAsButton() {
        if(scoopStack != null && Vector3.Distance(tapDown, Input.mousePosition) < Gestures.minSwipeDistance)
            board.ScoopTapped(currentIndex.y - 1); // The index of the scoop within the stack
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0,1,0,.5f);
        if(collider != null) {
            Gizmos.DrawCube(collider.offset, collider.size);
        }
        
    }

    
}
