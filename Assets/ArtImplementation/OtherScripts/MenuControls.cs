using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Start()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    /*
    public void showAbout()
    {
        if (!showAboutText)
        {
            aboutText.SetActive(true);
            showAboutText = true;
        } else
        {
            aboutText.SetActive(false);
            showAboutText = false;
        }
    }*/
}
