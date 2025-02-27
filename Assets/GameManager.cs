using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool gameEnded = false;

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

    // end the game
    void EndGame() {
        gameEnded = true;
        Debug.Log("Game Over!");
    }
}
