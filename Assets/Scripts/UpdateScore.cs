using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class UpdateScore : MonoBehaviour
{

    public TMP_Text scoreText;
    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        scoreText.text = "" + 0;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "" + PointsManager.RoundedPoints();
    }
}
