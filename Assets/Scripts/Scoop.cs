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

    public ScoopIndicator scoopIndicator;

    public static event Action<int> ScoopTapped = delegate {};

    private float scoopMeltDelay = .1f;

    private void Awake() {
        verticalLerp = GetComponent<Lerp>();
        verticalLerp.ReachedPoint += CheckCollisions;

    }

    public void Initialize(Grid grid, Vector2Int currentIndex, Cone cone) {
        this.grid = grid;
        this.currentIndex = currentIndex;
        this.cone = cone;
        horizontalLerp = gameObject.AddComponent<Lerp>();

        renderQuad = GetComponent<RenderQuad>();
        renderQuad.grid = grid;
        renderQuad.SetColor(this.flavor);
        renderQuad.Render(transform.position);

        transform.position = grid.GetPosition(currentIndex);

        scoopIndicator = (ScoopIndicator) GetComponentsInChildren<ScoopIndicator>()[0];
        scoopIndicator.SetIncomingFlavor(flavor);
        scoopIndicator.SetPosition(grid.GetPosition(new Vector2Int(currentIndex.x, grid.numberOfRows - 1)));

        CheckCollisions();
    }

    private bool HitFloor() {
        return currentIndex.y == 0;
    }

    private bool HitStack() {
        if(cone.ScoopValid(currentIndex)) {
            currentIndex.y = cone.StackHeight();
            transform.position = grid.GetPosition(currentIndex);
            return true;
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
            horizontalLerp.speed = cone.GetHorizontalLerpSpeed();
            verticalLerp.speed = 10;
            Gestures.OnTap += HandleScoopTap;
        } else if(HitFloor()) {
            Destroy(this.gameObject);
        } else {
            if(currentIndex.y <= grid.numberOfRows) {
                // Destroy scoop indicator
                scoopIndicator.gameObject.SetActive(false);
            } else {
                scoopIndicator.SetPosition(grid.GetPosition(new Vector2Int(currentIndex.x, grid.numberOfRows - 1)));
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

    public void MeltScoop(AudioSource audioSource, AudioEvent meltEvent) {
        StartCoroutine(Melt(audioSource, meltEvent));
    }

    public void MeltScoop() {
        Destroy(this.gameObject);
    }

    IEnumerator Melt(AudioSource audioSource, AudioEvent meltEvent) {
        meltEvent.Play(audioSource);
        yield return new WaitForSeconds(scoopMeltDelay);
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
