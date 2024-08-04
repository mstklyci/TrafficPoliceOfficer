using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    [SerializeField] Text[] highScoreTexts;

    void Start()
    {
        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            int score = PlayerPrefs.GetInt("HighScore" + i, -1);
            if (score >= 0)
            {
                highScoreTexts[i].text = score.ToString();
            }
            else
            {
                highScoreTexts[i].text = "";
            }
        }

        int currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
    }
}