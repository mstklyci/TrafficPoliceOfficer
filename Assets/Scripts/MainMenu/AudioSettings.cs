using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Image soundImg;
    [SerializeField] private Image musicImg;

    [SerializeField] private Sprite soundSp;
    [SerializeField] private Sprite sound0;
    [SerializeField] private Sprite musicSp;
    [SerializeField] private Sprite music0;

    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        audioMixer.SetFloat("MasterParam", 0);

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSoundVolume();
        }       
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("MusicParam", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);

        if (volume > 0.1)
        {
            musicImg.sprite = musicSp;
        }
        else if (volume <= 0.001)
        {
            musicImg.sprite = music0;
        }       
    }
    public void SetSoundVolume()
    {
        float Audiovolume = soundSlider.value;
        audioMixer.SetFloat("SoundParam", Mathf.Log10(Audiovolume) * 20);
        PlayerPrefs.SetFloat("soundVolume", Audiovolume);

        if (Audiovolume > 0.1)
        {
            soundImg.sprite = soundSp;
        }
        else if (Audiovolume <= 0.001)
        {
            soundImg.sprite = sound0;
        }
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        soundSlider.value = PlayerPrefs.GetFloat("soundVolume");

        SetMusicVolume();
        SetSoundVolume();
    }
}