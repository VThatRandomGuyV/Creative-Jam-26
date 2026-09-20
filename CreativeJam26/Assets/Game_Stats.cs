using UnityEngine;
using TMPro;

public class Game_Stats : MonoBehaviour
{
    public static Game_Stats Instance { get; private set; }

    [Header("Player Stats")]
    public int Health = 100;
    public int Gold1 = 100;
    public int Gold2 = 100;
    public int Gold3;

    [Header("UI Text References")]
    public TextMeshProUGUI gold1Text;
    public TextMeshProUGUI gold2Text;
    public TextMeshProUGUI healthText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        healthText.text = "HP: " + Health.ToString();
        gold1Text.text = "Gold: " + Gold1.ToString();
        gold2Text.text = "Gold: " + Gold2.ToString();
    }

    // Called externally by the LimitedTimeEventsManager
    public void ProcessChoiceResult(string chosenValue)
    {
        Debug.Log($"Game_Stats processing event result: {chosenValue}");

        // ==========================================
        // ELEMENT 0: YOU FOUND GOLD
        // ==========================================
        if (chosenValue == "Gain_Gold_1")
        {
            // Use it now in the past (+300 gold)
            Gold1 += 300;
            Debug.Log("Player chose to use the gold now! Added 300 Gold.");
        }
        else if (chosenValue == "Gain_Gold_2")
        {
            // Bury the treasure
            // Example: Maybe you want to give a different gold type or store a future reward flag
            Gold2 += 300;
            Debug.Log("Player buried the treasure for later!");
        }

        // ==========================================
        // ELEMENT 1: YOU MEET A WITCH!
        // ==========================================
        else if (chosenValue == "Witch_1")
        {
            // Pay her 300 to gain 20hp
            if (Gold1 >= 300)
            {
                Gold1 -= 300;
                Health += 20;
                Debug.Log("Paid the witch 300 gold. Gained 20 HP!");
            }
            else
            {
                Debug.LogWarning("Player didn't have enough Gold1 to pay the witch!");
                // Optional: Deduct whatever gold they have, or handle penalties here
            }
        }
        else if (chosenValue == "Witch_2")
        {
            // nahhhhh
            Debug.Log("Player walked away from the witch. Nothing happens.");
        }

        // ==========================================
        // ELEMENT 2: IS THE GAME TOO EASY?
        // ==========================================
        else if (chosenValue == "EASY_1")
        {
            // YES
            Debug.Log("Player thinks the game is too easy! Increasing difficulty...");
            // Example logic: Make enemies stronger or reduce future rewards
        }
        else if (chosenValue == "EASY_2")
        {
            // NO
            Debug.Log("Player thinks the difficulty is fine. Keeping it normal.");
        }

        // ==========================================
        // FALLBACK
        // ==========================================
        else
        {
            Debug.LogWarning($"Choice value '{chosenValue}' wasn't handled in Game_Stats.");
        }
    }

}
