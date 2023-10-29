using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menuPanel;
    private bool isPause;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause == false)
            {
                menuPanel.SetActive(true);
                Time.timeScale = 0f;
            }

            else
            {
                menuPanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    public void ResumeBtn()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SaveBtn()
    {

    }

    public void MenuBtn()
    {

    }

    public void QuitBtn()
    {

    }
}
