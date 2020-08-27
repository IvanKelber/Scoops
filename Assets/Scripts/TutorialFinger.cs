using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DentedPixel;


public class TutorialFinger : MonoBehaviour
{

    
    [SerializeField]
    private float swipeDistance;

    [SerializeField]
    private float swipeDuration;

    [SerializeField]
    private float swipeDelay;

    private bool swiping = false;
    private bool tapping = false;

    void Awake() {
        Fade(0,0);
    }

    void Update() {
        if(swiping && !LeanTween.isTweening(gameObject)) {
           Swipe();
        }

        if(tapping && !LeanTween.isTweening(gameObject)) {
            Tap();
        }
    }

    public void StartSwipe() {
        swiping = true;
    }

    public void StopSwipe() {
        if(swiping) {
            swiping = false;
            LeanTween.pause(gameObject);
            Fade(0,.3f).setOnComplete(Reset);
        }
    }
    public void StartTap(Vector3 tapPosition) {
        if(!tapping) {
            tapping = true;
            SetupTap(tapPosition);
        }
    }

    public void StopTap() {
        if(tapping) {
            tapping = false;
            LeanTween.pause(gameObject);
            Fade(0,.3f).setOnComplete(Reset);
        }
    }

    public void SetupTap(Vector3 tapPosition) {
        LeanTween.move(gameObject, tapPosition, 0);
        LeanTween.rotate(gameObject, new Vector3(0,0,45), 0);
        Fade(1,.3f).setDelay(swipeDelay);
    }

    public LTDescr Tap() {

        // return Fade(.5f,1).setOnComplete()
        return LeanTween.scale(gameObject, new Vector3(.8f,.8f,.8f), .3f).setLoopPingPong();
    }

    public void Swipe() {
        Fade(1,.3f).setDelay(swipeDelay);
        LeanTween.move(gameObject, transform.position + (Vector3.right * swipeDistance), swipeDuration).setEaseInOutCubic().setOnComplete(InitialSwipePosition);
    }

    public void InitialSwipePosition() {
        Fade(0,.3f).setOnComplete(() => LeanTween.move(gameObject, transform.position + (Vector3.left * swipeDistance), 0));
        
    }

    public LTDescr Fade(float finalAlpha, float duration) {
        Image image = gameObject.GetComponent<Image>();
        Color color = image.color;
        return LeanTween.value(gameObject, color.a, finalAlpha, duration).setOnUpdate((float val) =>
        {
            color.a = val;
            image.color = color;
        });
    }

    public void Reset() {
        LeanTween.reset();
    }

    public void Destroy() {
        LeanTween.reset();
        Destroy(gameObject);
    }

}
