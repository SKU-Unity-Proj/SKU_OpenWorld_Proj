using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuOption : MonoBehaviour
{
    public GameObject soundOption;
    public Slider bgmSlider;
    public Slider soundEffectSlider;
    public GameObject keyOption;
    public AudioSource Bgm;
    public AudioSource SoundEffect;

    public void OnClickSoundBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(true);
        keyOption.SetActive(false);
        bgmSlider.onValueChanged.AddListener(OnBgmSliderValueChanged);
        soundEffectSlider.onValueChanged.AddListener(OnSoundEffectSliderValueChanged);
    }

    public void OnClickKeyBtn()
    {
        SoundEffect.Play();
        soundOption.SetActive(false);
        keyOption.SetActive(true);
    }
    void OnBgmSliderValueChanged(float volume)
    {
        Bgm.volume = volume;
    }
    void OnSoundEffectSliderValueChanged(float volume)
    {
        SoundEffect.volume = volume;
    }
}