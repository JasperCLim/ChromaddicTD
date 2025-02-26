using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int Lives; // current number of lives
    public int startLives = 3; // starting number of lives


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Lives = startLives;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
