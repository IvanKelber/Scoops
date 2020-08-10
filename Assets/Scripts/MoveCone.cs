using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCone : MonoBehaviour
{
    public float speed;
    public float waitTime;

    [Range(0,5)]
    public float easeAmount;

    private float nextMoveTime;
    private float percentageBetweenPoints = 0;
    private bool lerping = false;
    private bool handlingSwipe = false;
    public LaneHelper laneHelper;
    private int nextLane;
    private int lastLane;

    private void Start() {
        nextMoveTime = Time.time;
        lastLane = laneHelper.MiddleLane;

        // Add gesture listeners
        Gestures.OnSwipe += HandleSwipe;
        Gestures.SwipeEnded += EndSwipe;
    }

    private void Update() {
        Vector3 velocity = CalculateConeMovement();
        transform.Translate(velocity);
    }
    private void HandleSwipe(SwipeInfo swipe) {
        if(handlingSwipe) {
            return;
        }
        handlingSwipe = true;
        nextLane = laneHelper.GetNextLane(lastLane, swipe.Direction);
        Debug.Log(lastLane);
        Debug.Log(nextLane);
        if(nextLane != lastLane) {
            lerping = true;
        }
        Debug.Log(swipe.Direction);
        Debug.Log(swipe.GetVelocity());

    }

    private void EndSwipe() {
        handlingSwipe = false;
    }

    private Vector3 CalculateLanePosition(int lane) {
        return laneHelper.GetPosition(lane, 0); //Cone is always on lane 0
    }

    private Vector3 CalculateConeMovement() {

        if(!lerping || Time.time < nextMoveTime) {
            return Vector3.zero;
        }

        Vector3 lastPosition = CalculateLanePosition(lastLane);
        Vector3 nextPosition = CalculateLanePosition(nextLane);
        float distanceBetweenPoints = Vector3.Distance(lastPosition, nextPosition);

        percentageBetweenPoints += Time.deltaTime * speed;
        percentageBetweenPoints = Mathf.Clamp01(percentageBetweenPoints);

        float easedPercentBetweenPoints = Ease(percentageBetweenPoints);

        Vector3 newPos = Vector3.Lerp(lastPosition, nextPosition, easedPercentBetweenPoints);

        if(percentageBetweenPoints >= 1) {
            percentageBetweenPoints = 0;
            nextMoveTime = Time.time + waitTime;
            lastLane = nextLane;
            lerping = false;
        }

        return newPos - transform.position;
    }

    private float Ease(float x) {
        return x;
    }



}
 