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
        isPaused = false;

        // Initialize buttons
        Button[] buttons = pauseMenu.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            ButtonSetup(btn);
        }

        // Start game with locked cursor
        LockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void ButtonSetup(Button btn)
    {
        Image btnImage = btn.GetComponent<Image>();
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();

        if (btnImage != null) btnImage.color = normalButtonColor;
        if (btnText != null) btnText.color = normalTextColor;

        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = btn.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        // Pointer Enter
        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) =>
        {
            if (btnImage != null) btnImage.color = hoverButtonColor;
            if (btnText != null) btnText.color = hoverTextColor;
        });
        trigger.triggers.Add(enter);

        // Pointer Exit
        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) =>
        {
            if (btnImage != null) btnImage.color = normalButtonColor;
            if (btnText != null) btnText.color = normalTextColor;
        });
        trigger.triggers.Add(exit);
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        UnlockCursor();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        LockCursor();
    }

    public void Tutorial()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        MainMenuUIManager.openHowToPlayDirectly = true;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        MainMenuUIManager.openMainMenuDirectly = true;

        SceneManager.LoadScene("MainMenuScene");
    }


    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
