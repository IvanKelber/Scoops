using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Lerp))]
public class Scoop : MonoBehaviour
{

    public Lerp verticalLerp;
    private Cone cone;
    private Vector2Int currentIndex;

    private Color flavor;

    public Grid grid;
    
    private void Awake() {
        verticalLerp = GetComponent<Lerp>();
        verticalLerp.ReachedPoint += CheckCollisions;
    }

    public void Initialize(Grid grid, Vector2Int currentIndex) {
        this.grid = grid;
        this.currentIndex = currentIndex;
        transform.position = grid.GetPosition(currentIndex);
        Fall();
    }

    private bool HitFloor() {
        return currentIndex.y == 0;
    }

    // Checks collisions when a scoop has reached a new index
    private void CheckCollisions() {
        if(!HitFloor()) {
            Fall();
        } else {
            Destroy(this.gameObject);
        }
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
