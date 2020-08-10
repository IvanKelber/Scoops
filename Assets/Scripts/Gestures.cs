using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gestures : MonoBehaviour
{

    [SerializeField]
    private bool detectBeforeRelase = true;

    [SerializeField]
    private float minSwipeDistance = 20f;


    private Vector3 fingerUpPosition;
    private Vector3 fingerDownPosition;

    public static event Action<SwipeInfo> OnSwipe = delegate { };
    public static event Action SwipeEnded = delegate { };

    private void Update() {
        foreach(Touch touch in Input.touches) {
            if(touch.phase == TouchPhase.Began) {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if(detectBeforeRelase && touch.phase == TouchPhase.Moved) {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if(touch.phase == TouchPhase.Ended) {
                fingerDownPosition = touch.position;
                DetectSwipe();
                SwipeEnded();
            }
        }   
    }

    private void DetectSwipe() {
        SwipeInfo swipeInfo = new SwipeInfo(fingerUpPosition, fingerDownPosition);
        if(swipeInfo.Magnitude > minSwipeDistance) {
            // We have detected a valid swipe
            OnSwipe(swipeInfo);
        }
    }


}
