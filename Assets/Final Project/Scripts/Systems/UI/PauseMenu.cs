using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public static bool isPaused;

    [Header("Button Colors")]
    public Color normalButtonColor = new Color(1, 1, 1, 0); // Transparent
    public Color hoverButtonColor = Color.white;
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.black;

    void Start()
    {
        pauseMenu.SetActive(false);

        // Initialize all buttons in the pause menu
        Button[] buttons = pauseMenu.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            ButtonSetup(btn);
        }
    }

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

    void ButtonSetup(Button btn)
    {
        // Ensure button has Image and Text
        Image btnImage = btn.GetComponent<Image>();
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();

        // Set default colors
        if (btnImage != null) btnImage.color = normalButtonColor;
        if (btnText != null) btnText.color = normalTextColor;

        // Add hover events
        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = btn.gameObject.AddComponent<EventTrigger>();

        // Pointer Enter
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            if (btnImage != null) btnImage.color = hoverButtonColor;
            if (btnText != null) btnText.color = hoverTextColor;
        });
        trigger.triggers.Add(entryEnter);

        // Pointer Exit
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            if (btnImage != null) btnImage.color = normalButtonColor;
            if (btnText != null) btnText.color = normalTextColor;
        });
        trigger.triggers.Add(entryExit);
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
