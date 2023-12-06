using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource BGM;
    public AudioSource SoundEffect;

    public void OnClickPlayBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("JackHouse");
    }
    public void OnClickRestartBtn()
    {
        SoundEffect.Play();
        Debug.Log("click btn");
    }
    public void OnClickChapterBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("Chapter");
    }
    public void OnClickOptionBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("Option");
    }
    public void OnClickQuitBtn()
    {
        SoundEffect.Play();
        Debug.Log("게임 종료");
        Application.Quit();
    }

}
