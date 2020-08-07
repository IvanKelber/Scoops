using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneHelper
{
    public int numberOfLanes = 3;
    private float screenHeight;
    private float screenWidth;
    private float laneWidth;
    private Bounds cameraBounds;

    private Camera cam;


    //lane Constants
    public int LeftLane = 0;
    public int MiddleLane = 1;
    public int RightLane = 2;
    public int OOBLane = -1;

    // Start is called before the first frame update
    public LaneHelper(Camera cam)
    {
        this.cam = cam;
        screenHeight = 2f * cam.orthographicSize;
        screenWidth = screenHeight * cam.aspect;
        laneWidth = screenWidth / numberOfLanes;
        cameraBounds = new Bounds(cam.transform.position, new Vector3(screenWidth, screenHeight, 0));
    }


    private int DetermineLane(Vector3 position) {
        if(position.x >= cameraBounds.min.x) {
            if(position.x < cameraBounds.min.x + laneWidth) {
                return LeftLane;
            } 
            if(position.x >= cameraBounds.min.x + laneWidth && position.x < cameraBounds.min.x + laneWidth * 2) {
                return MiddleLane;
            }
            if(position.x >= cameraBounds.min.x + laneWidth * 2 && position.x <= cameraBounds.max.x) {
                return RightLane;
            }
        }
        return OOBLane;
    }

    public float GetLaneCenter(int lane) {
        float laneCenter =  cameraBounds.min.x + laneWidth/2;
        return laneCenter + laneWidth * lane;
    }

    public int GetNextLane(int currentLane, SwipeInfo.SwipeDirection direction) {
        int nextLane = currentLane;
        if(direction == SwipeInfo.SwipeDirection.LEFT) {
            nextLane = Mathf.Clamp(nextLane - 1, LeftLane, RightLane);
        } else if(direction == SwipeInfo.SwipeDirection.RIGHT) {
            nextLane = Mathf.Clamp(nextLane + 1, LeftLane, RightLane);
        }
        return nextLane;
    }

}
