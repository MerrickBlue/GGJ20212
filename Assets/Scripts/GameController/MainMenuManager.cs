using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        AudioManager.AudioManag.StartMusic();
        AudioManager.AudioManag.FadeInIntro(1.75f);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("SCE_desertLevel_001");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("SCE_MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
