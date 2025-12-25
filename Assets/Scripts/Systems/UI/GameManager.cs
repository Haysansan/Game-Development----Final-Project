using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Call this when player wins
    public void PlayerWins()
    {
        SceneManager.LoadScene("VictoryScene");
    }

    // Call this when player loses
    public void PlayerLoses()
    {
        SceneManager.LoadScene("DefeatScene");
    }

    // Optional: Restart the game
    public void RestartGame()
    {
        SceneManager.LoadScene("GameplayScene");
    }

// Start is called before the first frame update
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
