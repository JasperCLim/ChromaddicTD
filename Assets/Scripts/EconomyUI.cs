using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI roundText;
    
    private EconomySystem economySystem;
    private EnemySpawner enemySpawner;
    
    void Start()
    {
        // Find econ/enemy references
        economySystem = FindFirstObjectByType<EconomySystem>();
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        
        // Connect the UI to the economy system
        if (economySystem != null && currencyText != null)
        {
            // The EconomySystem will use this text component to display currency
            economySystem.GetComponent<EconomySystem>().currencyText = currencyText;
        }
    }
    
    void Update()
    {
        // Update round display
        if (enemySpawner != null && roundText != null)
        {
            roundText.text = $"Round: {enemySpawner.GetRound()}";
        }
    }
}
