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

    private int matchSum = 0;
    private int multiplier = 0;

    private LTDescr tweenPoints;

    private int emptyCone;

    void Start()
    {
        scoreText.text = "" + 0;
        Fade(plusScoreText, 0,0);
        PointsManager.PointsAdded += UpdatePoints;
        PointsManager.PointsAccrue += AccruePoints;
        PointsManager.EmptyConeBonus += EmptyConeBonus;
    }

    void Update() {
        if(matchSum > 0 && multiplier > 0 && plusScoreText.faceColor.a == 0) {
            Debug.Log("Fading plusScore in");
            Fade(plusScoreText, 1, .2f);
        }
    }

    public void UpdatePoints() {
        pointsLeftToAdd += (matchSum * multiplier) + emptyCone;
        if(pointsLeftToAdd == 0) {
            // If it's called from something other than a match
            pointsDisplayed = PointsManager.Points;
            SetScore();
            return;
        }
        matchSum = 0;
        multiplier = 0;
        emptyCone = 0;
        tweenPoints = TweenPoints(pointsLeftToAdd).setDelay(1).setOnComplete(() => {
            Fade(plusScoreText, 0, .5f);
            pointsDisplayed = PointsManager.Points;
        });
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

    private void AccruePoints(int matchSum, int multiplier) {
        Debug.Log("Acrruing Points");
        this.matchSum += matchSum;
        this.multiplier = multiplier;
        plusScoreText.text = "" + this.matchSum + (multiplier > 1 ? " X " + multiplier: "");
    }

    private void EmptyConeBonus() {
        emptyCone = PointsManager.Empty_Cone_Bonus;
        plusScoreText.text += " + " + emptyCone;
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
