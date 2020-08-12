using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lerp : MonoBehaviour
{

    // Public (set through editor)
    public float speed;
    public LerpDescriptor descriptor;

    private float percentageBetweenPoints = 0;
    private bool lerping = false;
    private Vector3 currentPosition;
    private Vector3 nextPosition;
    
    public event Action ReachedPoint = delegate {};

    private void Update() {
        Vector3 velocity = CalculateMovement();
        transform.Translate(velocity);
    }
    public bool DoLerp(Vector3 currentPosition, Vector3 nextPosition) {
        if(lerping) {
            return false;
        }
        this.currentPosition = currentPosition;
        this.nextPosition = nextPosition;
        if(currentPosition != nextPosition) {
            lerping = true;
            return true;
        }
        return false;
    }

    private Vector3 CalculateMovement() {

        if(!lerping) {
            return Vector3.zero;
        }

        float distanceBetweenPoints = Vector3.Distance(currentPosition, nextPosition);

        percentageBetweenPoints += Time.deltaTime * speed;
        percentageBetweenPoints = Mathf.Clamp01(percentageBetweenPoints);

        float easedPercentBetweenPoints = Ease(percentageBetweenPoints);

        Vector3 newPos = Vector3.Lerp(currentPosition, nextPosition, easedPercentBetweenPoints);

        if(percentageBetweenPoints >= 1) {
            percentageBetweenPoints = 0;
            currentPosition = nextPosition;
            lerping = false;
            ReachedPoint();
        }

        return newPos - transform.position;
    }

    private float Ease(float x) {
        return x;
    }

    public enum LerpDescriptor
    {
        Horizontal, Vertical, Diagonal, Freeform
    }
}
 