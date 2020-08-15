using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Lerp))]
public class Scoop : MonoBehaviour
{

    private Lerp verticalLerp;
    private Lerp horizontalLerp;
    private Cone cone;
    private bool handlingSwipe = false;
    private Vector2Int currentIndex;

    public Color flavor;

    public Grid grid;
    
    public RenderQuad renderQuad;

    public static event Action<int> ScoopTapped = delegate {};


    private void Awake() {
        verticalLerp = GetComponent<Lerp>();
        verticalLerp.ReachedPoint += CheckCollisions;
    }

    public void Initialize(Grid grid, Vector2Int currentIndex, Cone cone) {
        this.grid = grid;
        this.currentIndex = currentIndex;
        this.cone = cone;
        horizontalLerp = gameObject.AddComponent<Lerp>();
        horizontalLerp.speed = cone.GetHorizontalLerpSpeed();

        renderQuad = GetComponent<RenderQuad>();
        renderQuad.grid = grid;
        renderQuad.SetColor(this.flavor);
        renderQuad.Render(transform.position);

        transform.position = grid.GetPosition(currentIndex);

        CheckCollisions();
    }

    private bool HitFloor() {
        return currentIndex.y == 0;
    }

    private bool HitStack() {
        if(currentIndex.x == cone.Lane()) {
            if(currentIndex.y == cone.StackHeight()) {
                return true;
            }
            if(currentIndex.y == cone.StackHeight() - 1) {
                currentIndex = new Vector2Int(currentIndex.x, currentIndex.y + 1);
                transform.position = grid.GetPosition(currentIndex);
                return true;
            }
        }
        return false;
    }

    private bool HitMiddleStack() {
        return currentIndex.x == cone.Lane() && currentIndex.y < cone.StackHeight() - 1;
    }

    // Checks collisions when a scoop has reached a new index
    private void CheckCollisions() {
        if(HitStack()) {
            cone.AddScoop(this);
            Gestures.OnSwipe += HandleSwipe;
            Gestures.SwipeEnded += EndSwipe;
            horizontalLerp.speed = cone.GetHorizontalLerpSpeed() - cone.StackHeight();
            Gestures.OnTap += HandleScoopTap;

        } else if(HitMiddleStack()) {
            this.Destroy();  
        } else if(HitFloor()) {
            this.Destroy();
        } else {
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
        Vector3 currentPosition = grid.GetPosition(currentIndex);
        Vector2Int nextIndex = grid.GetNextIndex(currentIndex, swipe.Direction);
        Vector3 nextPosition = grid.GetPosition(nextIndex);
        if(horizontalLerp.DoLerp(currentPosition, nextPosition)) {
            currentIndex = nextIndex;
        }
    }

    public void HandleScoopTap(Vector3 touchPosition) {
      if(renderQuad.Contains(touchPosition)) {
            ScoopTapped(currentIndex.y - 1); // The index of the scoop within the stack
        }
    }

    public void RemoveInputHandlers() {
        Gestures.OnSwipe -= HandleSwipe;
        Gestures.SwipeEnded -= EndSwipe; 
        Gestures.OnTap -= HandleScoopTap;

    }

    private void EndSwipe() {
        handlingSwipe = false;
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }

    private void OnDestroy() {
        RemoveInputHandlers();

    }

    public void SetSpeed(float speed) {
        verticalLerp.speed = speed;
    }
    
    public void MoveToIndex(Vector2Int index) {
        Debug.Log("Current index: " + currentIndex);
        Debug.Log("Next index: " + index);
        
        if(verticalLerp.DoLerp(grid.GetPosition(currentIndex), grid.GetPosition(index))) {
            currentIndex = index;
        };
    }


    private void Fall() {
        Vector2Int nextIndex = grid.GetNextIndex(currentIndex, SwipeInfo.SwipeDirection.DOWN);
        if(verticalLerp == null) {
            Debug.Log("Lerp is null");
            return;
        } 
        if(verticalLerp.DoLerp(grid.GetPosition(currentIndex), grid.GetPosition(nextIndex))) {
            currentIndex = nextIndex;
        }
    }

    public void SetFlavor(Color flavor) {
        this.flavor = flavor;
    }

    
}
