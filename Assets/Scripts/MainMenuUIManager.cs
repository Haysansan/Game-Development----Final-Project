using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUIManager : MonoBehaviour
{
    // 🔹 FLAG: detect if coming from gameplay
    public static bool openMainMenuDirectly = false;
    public static bool openHowToPlayDirectly = false;

    [Header("Panels")]
    public GameObject mainScreenPanel;
    public GameObject mainMenuPanel;
    public GameObject howToPlayPanel;
    public GameObject exitConfirmPanel;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Start()
    {
        if (openHowToPlayDirectly)
        {
            ShowHowToPlay();
            openHowToPlayDirectly = false;
        }
        else if (openMainMenuDirectly)
        {
            ShowMainMenu();
            openMainMenuDirectly = false;
        }
        else
        {
            ShowMainScreen();
        }

        fadeImage.color = new Color(0, 0, 0, 0);
    }

    // ---------- SCREEN CONTROL ----------
    public void ShowMainScreen()
    {
        mainScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        exitConfirmPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        mainScreenPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        howToPlayPanel.SetActive(false);
        exitConfirmPanel.SetActive(false);
    }

    public void ShowHowToPlay()
    {
        mainScreenPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        exitConfirmPanel.SetActive(false);
    }

    // ---------- EXIT FLOW ----------
    public void ShowExitConfirm()
    {
        exitConfirmPanel.SetActive(true);
    }

    public void CancelExit()
    {
        exitConfirmPanel.SetActive(false);
    }

    public void ConfirmExit()
    {
        SaveSettings();
        StartCoroutine(FadeAndExit());
    }

    // ---------- SAVE SETTINGS ----------
    void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", 1f);
        PlayerPrefs.SetInt("HasLaunched", 1);
        PlayerPrefs.Save();
    }

    // ---------- FADE & QUIT ----------
    IEnumerator FadeAndExit()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
