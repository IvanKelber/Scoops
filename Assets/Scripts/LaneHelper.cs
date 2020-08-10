using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneHelper : MonoBehaviour
{
    public int numberOfLanes = 3;
    public int numberOfRows = 10;
    private float screenHeight;
    private float screenWidth;
    public float laneWidth;
    public float rowHeight;
    private Bounds cameraBounds;
    public GridLocation[][] grid;

    public Camera cam;


    //lane Constants
    public int LeftLane = 0;
    public int MiddleLane = 1;
    public int RightLane = 2;
    public int OOBLane = -1;

    private void Start()
    {
        screenHeight = 2f * cam.orthographicSize;
        screenWidth = screenHeight * cam.aspect;
        cameraBounds = new Bounds(cam.transform.position, new Vector3(screenWidth, screenHeight, 0));
        grid = InitializeGrid();
    }


    private GridLocation[][] InitializeGrid() {
        GridLocation [][] grid = new GridLocation[numberOfLanes][];
        for(int i = 0; i < numberOfLanes; i++) {
            grid[i] = new GridLocation[numberOfRows];
            for(int j = 0; j < numberOfRows; j++) {
                grid[i][j] = new GridLocation(i, j, CalculateGamePosition(i, j));
            }
        }
        return grid;
    }

    private Vector3 CalculateGamePosition(int lane, int row) {
            float xPosition = CalculateCoordinate(cameraBounds.min.x, cameraBounds.max.x, this.numberOfLanes, lane);
            float yPosition = CalculateCoordinate(cameraBounds.min.y, cameraBounds.max.y, this.numberOfRows, row);
            return new Vector3(xPosition, yPosition, 1);
        }

        private float CalculateCoordinate(float minimum, float maximum, float totalNumber, float i) {
            float dimensionLength = maximum - minimum; // total grid dimensionLength
            float cellLength = dimensionLength / totalNumber;
            float cellCenter = minimum + cellLength / 2;
            return cellCenter + cellLength * i;
        }
    public Vector3 GetPosition(int lane, int row) {
        return grid[lane][row].gamePosition;
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


    public struct GridLocation {
        int lane, row;
        public Vector3 gamePosition;

        public GridLocation(int lane, int row, Vector3 gamePosition) {
            this.lane = lane;
            this.row = row;
            this.gamePosition = gamePosition;
        }

    }

    private void OnDrawGizmos() {
        if(grid != null) {
            for(int i = 0; i < numberOfLanes; i++) {
                for(int j = 0; j < numberOfRows; j++) {
                    Gizmos.DrawWireSphere(GetPosition(i,j),.01f);
                }
            }
        }

    }

}
