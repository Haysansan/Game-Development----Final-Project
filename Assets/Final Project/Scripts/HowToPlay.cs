using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject howToAttackPanel;
    public GameObject functionalityPanel;

    public void ShowFunctionalityScreen()
    {
        howToAttackPanel.SetActive(false);
        functionalityPanel.SetActive(true);
    }

    public void ShowHowToAttackScreen()
    {
        functionalityPanel.SetActive(false);
        howToAttackPanel.SetActive(true);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameplayScene");
    }
}
