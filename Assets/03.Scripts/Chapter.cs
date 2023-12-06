using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Chapter : MonoBehaviour
{
    public AudioSource SoundEffect;
    
    public void OnClickPrologueBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("");
    }
    public void OnClickTutorialBtn()
    {
        SoundEffect.Play();
        SceneManager.LoadScene("JackHouse");
    }
}
