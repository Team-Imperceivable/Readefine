using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Canvas settingsCanvas;

    public void StartGame()
    {
        SceneManager.LoadScene("Level_1");
        menuCanvas.enabled = true;
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        settingsCanvas.enabled = true;
        menuCanvas.enabled = false;
    }
}
