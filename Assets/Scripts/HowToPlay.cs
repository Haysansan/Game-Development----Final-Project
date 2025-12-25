using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject howToAttackPanel;
    public GameObject functionalityPanel;

    public void ShowHowToAttackScreen()
    {
        howToAttackPanel.SetActive(true);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameplayScene");
    }
}
