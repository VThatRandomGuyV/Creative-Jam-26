using System.Collections.Generic;
using UnityEngine;

// SCOPE: allows the user to create custom waves that are then passed to the enemy spawner when the game starts
public class Wave_Manager : MonoBehaviour {

    private List<(GameObject, int, float)> waves;
    private float gameTime;
    public GameObject enemySpawner;
    private Enemy_Spawner spawner;

    // enemy types
    public List<GameObject> enemies;
    

    void Awake() {
        spawner = enemySpawner.GetComponent<Enemy_Spawner>();
        
        waves = new List<(GameObject, int, float)> { // enemy prefab, number to spawn, time it spawns
            (enemies[5], 5, 0),
            (enemies[6], 5, 0),
            (enemies[0], 5, 30),
            (enemies[3], 5, 40),
            (enemies[0], 10, 45),
            (enemies[2], 1, 45),
            (enemies[4], 1, 60),
            (enemies[2], 10, 60)
        };
    }
    
    void Update() {
        gameTime += Time.deltaTime;

        if (waves.Count > 0) {
            if (gameTime > waves[0].Item3) {
                (GameObject, int, float) nextEnemyGroup = waves[0];
                waves.RemoveAt(0);
                for (int i = 0; i < nextEnemyGroup.Item2; i++) {
                    spawner.AddToQueue(nextEnemyGroup.Item1);
                }
            }
        }
    }
}
