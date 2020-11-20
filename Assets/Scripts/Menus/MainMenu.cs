using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI, howToUI, recipesUI, settingsUI;
    public GameObject bg;

    void Update() 
    {
        if (SceneManager.GetActiveScene().name == "Game" && (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // This method makes the menu do stuff, and not run bad
    public void PlayGame()
    {
        SceneManager.LoadScene("Match_Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        if(SceneManager.GetActiveScene().name == "Game") {
            GameObject.Find("UI ELEMENTS").GetComponents<AudioSource>()[1].Stop();
            pauseMenuUI.SetActive(false);
            bg.SetActive(false);
            howToUI.SetActive(false);
            recipesUI.SetActive(false);
            settingsUI.SetActive(false);
            Time.timeScale = 1f;
            gameIsPaused = false;
        }
        SceneManager.LoadScene("Main_Menu");
    }

    public void Resume()
    {
        if(SceneManager.GetActiveScene().name == "Game") {
            pauseMenuUI.SetActive(false);
            bg.SetActive(false);
            howToUI.SetActive(false);
            recipesUI.SetActive(false);
            settingsUI.SetActive(false);
            GameObject.Find("UI ELEMENTS").GetComponents<AudioSource>()[1].UnPause();
        }
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Pause() 
    {
        pauseMenuUI.SetActive(true);
        bg.SetActive(true);
        GameObject.Find("UI ELEMENTS").GetComponents<AudioSource>()[1].Pause();
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

}
