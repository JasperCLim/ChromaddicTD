using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public Text livesText;

    /*
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        livesText.text = "Lives Remaining: " + PlayerStats.Lives.ToString(); // update lives of player
    }
}
