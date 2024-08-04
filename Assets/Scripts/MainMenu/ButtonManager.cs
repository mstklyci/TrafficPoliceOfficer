using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    [SerializeField] private Sprite greenStopImage;
    [SerializeField] private Sprite pauseStopImage;
    [SerializeField] private Sprite greenSoundImage;
    [SerializeField] private Sprite darkgreenSoundImage;


    [SerializeField] private Button StopButton;
    [SerializeField] private Button SoundButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button MenuButton;

    private bool isStopped;
    private bool soundOpen;

    //Audio
    [SerializeField] private AudioMixer audioMixer;
    private AudioListener audioListener;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip musicClip;
    [SerializeField] AudioClip buttonClip;
    [SerializeField] AudioClip newSceneClip;

    private void Start()
    {
        isStopped = false;
        soundOpen = true;

        Time.timeScale = 1.0f;
        StopButton.image.sprite = greenStopImage;

        audioListener = FindObjectOfType<AudioListener>();
        audioListener.enabled = true;
        SoundButton.image.sprite = greenSoundImage;

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("musicVolume");
            musicSource.volume = musicVolume;
        }
        if (PlayerPrefs.HasKey("soundVolume"))
        {
            float soundVolume = PlayerPrefs.GetFloat("soundVolume");
            audioSource.volume = soundVolume;
        }

        if (musicClip != null)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopGame()
    {
        if (isStopped == false)
        {
            Time.timeScale = 0;
            isStopped = true;
            StopButton.image.sprite = pauseStopImage;
            audioSource.PlayOneShot(buttonClip);
        }
        else
        {
            Time.timeScale = 1f;
            isStopped = false;
            StopButton.image.sprite = greenStopImage;
            audioSource.PlayOneShot(buttonClip);
        }       
    }

    public void SaoundOn()
    {
        if (soundOpen == true)
        {
            soundOpen = false;
            SetMasterVolume(0f);
            SoundButton.image.sprite = darkgreenSoundImage;          
        }
        else
        {
            soundOpen = true;
            SetMasterVolume(1f);
            SoundButton.image.sprite = greenSoundImage;
            audioSource.PlayOneShot(buttonClip);
        }
    }
    void SetMasterVolume(float volume)
    {
        float dB;
        if (volume == 0f)
        {
            dB = -80f;
        }
        else if (volume == 1f)
        {
            dB = 0f;
        }
        else
        {
            dB = Mathf.Lerp(-80f, 0f, volume);
        }

        audioMixer.SetFloat("MasterParam", dB);
    }

    public void RestartGame()
    {
        StartCoroutine(RestartSound());
    }
    private IEnumerator RestartSound()
    {
        audioSource.PlayOneShot(newSceneClip);
        yield return new WaitWhile(() => audioSource.isPlaying);
        SceneManager.LoadScene(1);
    }
    public void MainMenu()
    {
        StartCoroutine(MainSound());
    }
    private IEnumerator MainSound()
    {
        audioSource.PlayOneShot(newSceneClip);
        yield return new WaitWhile(() => audioSource.isPlaying);
        SceneManager.LoadScene(0);
    }
}