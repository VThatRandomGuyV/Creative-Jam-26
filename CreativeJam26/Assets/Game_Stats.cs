using UnityEngine;
using TMPro;
public class Game_Stats : MonoBehaviour {
    public static Game_Stats Instance { get; private set; }
    public int Health;
    
    public int Gold1;
    public int Gold2;
    public int Gold3;

    public TextMeshProUGUI gold1Text;
    public TextMeshProUGUI gold2Text;
    public TextMeshProUGUI healthText;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        // Updates every frame; for better performance, call this only when the score changes
        healthText.text = "HP: " + Health.ToString();
        gold1Text.text = "Gold: " + Gold1.ToString();
        gold2Text.text = "Gold: " + Gold2.ToString();
    }

}
