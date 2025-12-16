using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject howToAttackPanel;
    public GameObject functionalityPanel;

    // Called when Continue button is pressed
    public void ShowFunctionalityScreen()
    {
        howToAttackPanel.SetActive(false);
        functionalityPanel.SetActive(true);
    }

    // Optional: go back to attack screen
    public void ShowHowToAttackScreen()
    {
        functionalityPanel.SetActive(false);
        howToAttackPanel.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
