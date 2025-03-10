using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool gameEnded = false;
    public CanvasGroup gameOverMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnded) {
            return;
        }

        // end game if player has no lives
        if (PlayerStats.Lives <= 0) {
            EndGame();
        }
    }

    // restart the game
    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // end the game
    void EndGame() {
        gameEnded = true;

        // display the game over menu and make it interactable
        gameOverMenu.alpha = 1;
        gameOverMenu.interactable = true;

        Debug.Log("Game Over!");
    }
}
