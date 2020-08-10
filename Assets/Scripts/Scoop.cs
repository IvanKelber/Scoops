using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Scoop : MonoBehaviour
{


    private RenderQuad renderQuad;

    public float speed = 10;

    [SerializeField]
    private LaneHelper grid;

    private float percentageBetweenPoints;
    

    int lane = 0;
    int currentRow;
    int nextRow;

    bool falling = false;

    // Start is called before the first frame update
    void Start()
    {

        SetAtTopOfLane();
        falling = true;
    }

    private void SetAtTopOfLane() {
        renderQuad = GetComponent<RenderQuad>();
        currentRow = grid.numberOfRows - 1;
        nextRow = currentRow - 1;
        renderQuad.Render(transform.position);
        transform.position = grid.GetPosition(lane, currentRow);
 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = CalculateScoopMovement();
        transform.Translate(velocity);
        if(currentRow == 0) {
            falling = false;
        }
    }

     private Vector3 CalculateScoopMovement() {

        if(!falling) {
            return Vector3.zero;
        }

        Vector3 lastPosition = grid.GetPosition(lane, currentRow);
        Vector3 nextPosition = grid.GetPosition(lane, nextRow);
        float distanceBetweenPoints = Vector3.Distance(lastPosition, nextPosition);

        percentageBetweenPoints += Time.deltaTime * speed;
        percentageBetweenPoints = Mathf.Clamp01(percentageBetweenPoints);

        Vector3 newPos = Vector3.Lerp(lastPosition, nextPosition, percentageBetweenPoints);

        if(percentageBetweenPoints >= 1) {
            percentageBetweenPoints = 0;
            currentRow = nextRow;
            nextRow = currentRow - 1;
        }

        return newPos - transform.position;
    }
    
}
