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

    private void Start() {
        horizontalLerp = GetComponent<Lerp>();
        currentIndex = new Vector2Int(1,0); // Start in middle lane

        // renderQuad.Render(transform.position);
        transform.position = grid.GetPosition(currentIndex);

        // Add gesture listeners
        Gestures.OnSwipe += HandleSwipe;
        Gestures.SwipeEnded += EndSwipe;

        // Other event listeners
    }

    private void Update() {
        // Vector3 velocity = CalculateConeMovement();
        // transform.Translate(velocity);
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


    public void AddHeight() {
        currentIndex.y++;
    }
}
 