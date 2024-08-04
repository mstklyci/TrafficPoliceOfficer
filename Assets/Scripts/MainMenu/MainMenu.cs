using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Button playButton;
    [SerializeField] private Button howtoplayButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button highScoreButton;
    [SerializeField] private Button settingsButton;

    [SerializeField] private Button howtoplayCloseButton;
    [SerializeField] private Button creditsCloseButton;
    [SerializeField] private Button highscoreCloseButton;
    [SerializeField] private Button settingsCloseButton;

    [SerializeField] private GameObject howtoplayPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject highscorePanel;
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip buttonClip;
    [SerializeField] AudioClip musicClip;
    [SerializeField] AudioClip newSceneClip;    

    private void Start()
    {
        howtoplayPanel.SetActive(false);
        creditsPanel.SetActive(false);
        highscorePanel.SetActive(false);
        settingsPanel.SetActive(false);

        if (musicClip != null)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayBtn()
    {
        StartCoroutine(PlaySound());
    }
    private IEnumerator PlaySound()
    {
        audioSource.PlayOneShot(newSceneClip);
        yield return new WaitWhile(() => audioSource.isPlaying);
        SceneManager.LoadScene(1);
    }
    public void HowToPlayBtn()
    {
        howtoplayPanel.SetActive(true);
        audioSource.PlayOneShot(buttonClip);
    }
    public void howtoplayCloseBtn()
    {
        howtoplayPanel.SetActive(false);
        audioSource.PlayOneShot(buttonClip);
    }
    public void CreditsBtn()
    {
        creditsPanel.SetActive(true);
        audioSource.PlayOneShot(buttonClip);
    }
    public void creditsCloseBtn()
    {
        creditsPanel.SetActive(false);
        audioSource.PlayOneShot(buttonClip);
    }
    public void HighScoreBtn()
    {
        highscorePanel.SetActive(true);
        audioSource.PlayOneShot(buttonClip);
    }
    public void highscoreCloseBtn()
    {
        highscorePanel.SetActive(false);
        audioSource.PlayOneShot(buttonClip);
    }
    public void SettingsBtn()
    {
        settingsPanel.SetActive(true);
        audioSource.PlayOneShot(buttonClip);
    }
    public void settingsCloseBtn()
    {
        settingsPanel.SetActive(false);
        audioSource.PlayOneShot(buttonClip);
    }
}