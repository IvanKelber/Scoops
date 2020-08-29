using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DentedPixel;

public class UpdateScore : MonoBehaviour
{

    public TMP_Text scoreText;
    public TMP_Text plusScoreText;

    private int pointsLeftToAdd = 0;
    private int pointsDisplayed = 0;
    void Start()
    {
        scoreText.text = "" + 0;
        Fade(plusScoreText, 0,0);
        PointsManager.PointsAdded += UpdatePoints;
    }


    public void UpdatePoints(int pointsAdded) {
        Debug.Log("plusScoreText: " + (plusScoreText == null));
        Debug.Log("scoreText: " + (scoreText == null));
        if(pointsAdded >  1) {
            pointsLeftToAdd += pointsAdded;
            SetPlusScore();
            Fade(plusScoreText, 1, .2f).setOnComplete(() => {
                TweenPoints(pointsLeftToAdd).setDelay(1).setOnComplete(() => {
                    Fade(plusScoreText, 0, .5f);
                    pointsDisplayed = PointsManager.RoundedPoints();
                });
            });
        } else {
            pointsDisplayed += pointsAdded;
            SetScore();
        }
    }

    public LTDescr Fade(TMP_Text tmp, float finalAlpha, float duration) {
        Color color = tmp.faceColor;
        return LeanTween.value(tmp.gameObject, color.a, finalAlpha, duration).setOnUpdate((float val) =>
        {
            color.a = val;
            tmp.faceColor = color;
        });
    }

    public LTDescr TweenPoints(int pointsToAdd) {
        return LeanTween.value(pointsToAdd, 0, 1f).setOnUpdate((float val) => {
            int difference = pointsLeftToAdd - (int) val;
            pointsLeftToAdd = (int) val;
            pointsDisplayed += difference;
            SetPlusScore();
            SetScore();
        });

        
    }

    private void SetPlusScore() {
        plusScoreText.text = "+" + pointsLeftToAdd;
    }
    private void SetScore() {
        scoreText.text = "" + pointsDisplayed;
    }

    private void OnDestroy() {
        PointsManager.PointsAdded -= UpdatePoints;
    }

}
