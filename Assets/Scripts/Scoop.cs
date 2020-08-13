using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Lerp))]
public class Scoop : MonoBehaviour
{

    private Lerp verticalLerp;
    private Lerp horizontalLerp;
    private Cone cone;
    private bool handlingSwipe = false;
    private Vector2Int currentIndex;

    private Color flavor;

    public Grid grid;
    
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

        transform.position = grid.GetPosition(currentIndex);

        CheckCollisions();
    }

    private bool HitFloor() {
        return currentIndex.y == 0;
    }

    private bool HitStack() {
        return currentIndex.x == cone.Lane() && currentIndex.y == cone.StackHeight();
    }

    private bool HitMiddleStack() {
        return currentIndex.x == cone.Lane() && currentIndex.y < cone.StackHeight();
    }

    // Checks collisions when a scoop has reached a new index
    private void CheckCollisions() {
        if(HitStack()) {
            cone.AddScoop(this);
            Gestures.OnSwipe += HandleSwipe;
            Gestures.SwipeEnded += EndSwipe;
            Debug.Log("Landed on top of stack");
        } else if(HitMiddleStack()) {
            Debug.Log("Hit Middle Stack");
            Destroy(this.gameObject);  
        } else if(HitFloor()) {
            Destroy(this.gameObject);
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
    private void EndSwipe() {
        handlingSwipe = false;
    }

    public void SetSpeed(float speed) {
        verticalLerp.speed = speed;
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
