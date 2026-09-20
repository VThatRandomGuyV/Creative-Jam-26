using UnityEngine;

public class Bank_Controller : MonoBehaviour {
    private GameObject bankStuff;
    private GameObject bankStuff2;
    public bool bankBought;

    void Awake() {
        bankStuff = GameObject.Find("Bank Stuff");
        bankStuff2 = GameObject.Find("Bank Stuff2");
    }
    
    public void OnMouseDown() {
        Time.timeScale = 0f;
        if (!bankBought) {
            var vector32 = bankStuff2.transform.position;
            vector32.y = 540;
            bankStuff2.transform.position = vector32;
        }
        var vector3 = bankStuff.transform.position;
        vector3.y = 540;
        bankStuff.transform.position = vector3;
    }

    public void Deposit() {
        Game_Stats.Instance.Gold1 -= 100;
        Game_Stats.Instance.Gold2 += 100;
    }

    public void Withdraw() {
        Game_Stats.Instance.Gold1 += 100;
        Game_Stats.Instance.Gold2 -= 100;
    }

    public void BuyBank() {
        bankBought = true;
        Game_Stats.Instance.Gold1 -= 50;
        Game_Stats.Instance.Gold2 -= 50;
        var vector32 = bankStuff2.transform.position;
        vector32.y = 2000;
        bankStuff2.transform.position = vector32;
        var vector3 = bankStuff.transform.position;
        vector3.y = 540;
        bankStuff.transform.position = vector3;
    }

    public void Exit() {
        Time.timeScale = 1f;
            var vector3 = bankStuff.transform.position;
            vector3.y = 2000;
            bankStuff.transform.position = vector3;
            var vector32 = bankStuff2.transform.position;
            vector32.y = 2000;
            bankStuff2.transform.position = vector32;
    }
}
