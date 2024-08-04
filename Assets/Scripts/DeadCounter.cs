using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeadCounter : MonoBehaviour
{

    public static int deadCounter = 10;
    public static int scoreCounter = 0;
    public static int gameScore;
    private bool highScoreSaved = false;

    [SerializeField] Text scoreText;
    [SerializeField] Text gameScoreText;
    [SerializeField] Image loseImage;

    [SerializeField] CharacterController controller;

    [SerializeField] Sprite crosswalkSprite, xSprite;
    [SerializeField] Image[] DeadImages;

    void Start()
    {
        deadCounter = 10;
        scoreCounter = 0;
        gameScore = 0;
        loseImage.gameObject.SetActive(false);
        scoreText.text = scoreCounter.ToString();
        controller = GameObject.FindWithTag("Police").GetComponent<CharacterController>();

        foreach (Image spriteRenderer in DeadImages)
        {
            spriteRenderer.sprite = crosswalkSprite;
        }        

    }

    void Update()
    {
        scoreText.text = scoreCounter.ToString();
        gameScoreText.text = gameScore.ToString();       

        if (deadCounter >= 1 && deadCounter <= 9 && controller.policeDead == false)
        {
            if (deadCounter - 1 < DeadImages.Length)
            {
                if (DeadImages[deadCounter].sprite != xSprite)
                {
                    DeadImages[deadCounter].sprite = xSprite;
                    ImageControl();
                }
            }
        }
        else if (deadCounter <= 0)
        {
            loseImage.gameObject.SetActive(true);

            if (deadCounter - 1 < DeadImages.Length)
            {
                ImageControl();
            }
        }
        else if (controller.policeDead == true)
        {
            loseImage.gameObject.SetActive(true);
        }

        if(loseImage.gameObject.activeSelf == true)
        {
            gameScore = scoreCounter;
            ScoreSave();
        }
    }
    void ImageControl()
    {
        if (deadCounter <= 0)
        {
            deadCounter = 0;
        }
        else if (deadCounter >= DeadImages.Length)
        {
            deadCounter = DeadImages.Length - 1;
        }

        for (int i = deadCounter; i < DeadImages.Length; i++)
        {
            if (DeadImages[i].sprite != xSprite)
            {
                DeadImages[i].sprite = xSprite;
            }
        }
    }

    public void ScoreSave()
    {
        PlayerPrefs.SetInt("CurrentScore", gameScore);

        if (highScoreSaved == false)
        {
            AddScoreToHighScores(gameScore);
            highScoreSaved = true;
        }
    }

    void AddScoreToHighScores(int score)
    {
        List<int> highScores = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            highScores.Add(PlayerPrefs.GetInt("HighScore" + i, 0));
        }

        highScores.Add(score);
        highScores.Sort((a, b) => b.CompareTo(a));

        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
        }
    }
}