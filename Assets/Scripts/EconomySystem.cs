using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EconomySystem : MonoBehaviour
{
    [Header("Economy Settings")]
    [SerializeField] private int startingCurrency = 200;
    [SerializeField] public TextMeshProUGUI currencyText;
    
    [Header("Tower Costs")]
    [SerializeField] private int towerCost = 50;
    
    private int currentCurrency;
    private EnemySpawner enemySpawner;
    
    void Start()
    {
        // Initialize currency
        currentCurrency = startingCurrency;
        
        // Find the enemy spawner
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        
        // Initialize UI
        if (currencyText == null)
        {
            Debug.LogWarning("Currency Text UI is not set in EconomySystem!");
        }
        else
        {
            UpdateCurrencyDisplay();
        }
    }
    
    // Check if player can afford a tower
    public bool CanAffordTower(int towerIndex)
    {
        int cost = GetTowerCost(towerIndex);
        return currentCurrency >= cost;
    }
    
    // Get tower cost
    public int GetTowerCost(int towerIndex)
    {
        return towerCost;
    }
    
    // Purchase a tower if possible
    public bool PurchaseTower(int towerIndex)
    {
        int cost = GetTowerCost(towerIndex);
        
        if (currentCurrency >= cost)
        {
            currentCurrency -= cost;
            UpdateCurrencyDisplay();
            return true;
        }
        return false;
    }
    
    // Add currency
    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        UpdateCurrencyDisplay();
    }
    
    // Update the UI display
    private void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            currencyText.text = "Gold: " + currentCurrency.ToString();
        }
    }
    
    // Fixed reward after each round
    public int GetRoundReward()
    {
        // Fixed 200 gold reward per round
        return 200;
    }
    
    // Extra money for killing enemies
    // public void RewardForKill(string enemyColor)
    // {
    //     return;
    // }
}
