using UnityEngine;

public class Game_Stats : MonoBehaviour {
    public static Game_Stats Instance { get; private set; }
    public int Health;
    
    public int Gold1;
    public int Gold2;
    public int Gold3;
    
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

}
