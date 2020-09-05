using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class DisplayLastScore : MonoBehaviour
{
    public TMP_Text scoreText;
    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int points = PointsManager.Points;
        if(points == 0) {
            Destroy(this.gameObject);
        } else {
            scoreText.text = "Score: " + PointsManager.Points;
        }
    }
}
