﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public int numberOfLanes;
    public int numberOfRows;
    public Camera cam;

    public int numberOfOffscreenRows;
    public int TotalRows {
        get {
            return numberOfOffscreenRows + numberOfRows;
        }
    }
    public Vector3[,] grid;

    public Cone cone;

    public ScoopManager scoopManager;

    public float laneWidth;
    public float rowHeight;

    public float lives = 3;
    public bool gameEnded = false;

    private float timeUntilScoreUpdate;
    private float scoreUpdateDelay = 1f;

    public AudioSource AudioSource;
    private void Awake()
    {
        PointsManager.Points = 0;
        if(cam == null) {
            Debug.LogError("Cannot calculate grid bounds because camera is missing.");
            return;
        }
        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;
        laneWidth = screenWidth/numberOfLanes;
        rowHeight = screenHeight/numberOfRows;
        Bounds cameraBounds = new Bounds(cam.transform.position, new Vector3(screenWidth, screenHeight, 0));
        grid = new Grid(TotalRows, numberOfRows, numberOfLanes, rowHeight, laneWidth, cameraBounds).grid;

        if(scoopManager != null) {
            scoopManager.board = this;
        } else {
            Debug.LogError("ScoopManager is null for BoardManager");
        }

        AudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Update() {
        if(!gameEnded && scoopManager.spawning && timeUntilScoreUpdate <= 0) {
            PointsManager.AddPoints(1);
            timeUntilScoreUpdate = scoreUpdateDelay;
        }
        timeUntilScoreUpdate -= Time.deltaTime;
    }

    public void GameOver() {
        gameEnded = true;
        cone.GameOver();
    }

    public int RandomLane() {
        return Random.Range(0, numberOfLanes);
    }


    public Vector3 GetPosition(int lane, int row) {
        return transform.TransformPoint(transform.TransformPoint(grid[lane,row]));
    }

    public Vector3 GetPosition(Vector2Int index) {
        return GetPosition(index.x, index.y);
    }

    public Vector2Int GetNextIndex(Vector2Int index, SwipeInfo.SwipeDirection direction) {
        Vector2Int nextIndex = index;
        if(direction == SwipeInfo.SwipeDirection.LEFT) {
            nextIndex.x = Mathf.Clamp(index.x - 1, 0, numberOfLanes-1);
        } else if(direction == SwipeInfo.SwipeDirection.RIGHT) {
            nextIndex.x = Mathf.Clamp(index.x + 1, 0, numberOfLanes-1);
        }

        if(direction == SwipeInfo.SwipeDirection.DOWN) {
            nextIndex.y = Mathf.Clamp(index.y - 1, 0, TotalRows - 1);
        } else if(direction == SwipeInfo.SwipeDirection.UP) {
            nextIndex.y = Mathf.Clamp(index.y + 1, 0, TotalRows - 1);
        }

        return nextIndex;
    }
    public float GetHorizontalLerpSpeed()
    {
        return cone.horizontalLerp.speed;
    }

    public int HitStack(Vector2Int scoop) {
        if(cone.ValidLane(scoop.x)) {
            if(scoop.y <= ConeStackHeight() && scoop.y >= ConeStackHeight() - 2) {
                return scoop.y;
            }
        }
        return -1;
    }

    public void AddScoopToCone(Scoop scoop) {
        cone.AddScoop(scoop);
    }

    public int ConeLane() {
        return cone.Lane();
    }

    public int ConeStackHeight() {
        return cone.StackHeight();
    }

    private void OnDrawGizmos() {
        if(grid != null) {
            for(int i = 0; i < numberOfLanes; i++) {
                for(int j = 0; j < TotalRows; j++) {
                    Gizmos.DrawWireSphere(GetPosition(i,j),.01f);
                }
            }
        }

    }

}
