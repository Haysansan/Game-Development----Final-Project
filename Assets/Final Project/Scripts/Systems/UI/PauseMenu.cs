using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    public static bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeInHierarchy)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
       pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    //public void OpenOptions()
    //{
    //    pauseMenu.SetActive(false);
    //    optionsMenu.SetActive(true);
    //}

    //public void CloseOptions()
    //{
    //    optionsMenu.SetActive(false);
    //    pauseMenu.SetActive(true);
    //}
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Tutorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tutorial");
    }


    public void Exit()
    {
        Application.Quit();
    }
}
