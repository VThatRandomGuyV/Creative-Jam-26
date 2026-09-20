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
            (enemies[0], 5, 6),
            (enemies[0], 8, 20),
            (enemies[1], 4, 40),
            (enemies[0], 6, 55),
            (enemies[1], 6, 70),
            (enemies[0], 8, 80),
            (enemies[1], 5, 95),
            (enemies[2], 3, 110),
            (enemies[3], 4, 125),
            (enemies[0], 12, 135),
            (enemies[1], 10, 135),
            (enemies[2], 4, 150),
            (enemies[3], 6, 165),
            (enemies[0], 10, 175),
            (enemies[2], 5, 185),
            (enemies[4], 2, 200),
            (enemies[1], 12, 200),
            (enemies[3], 8, 215),
            (enemies[5], 4, 225),
            (enemies[0], 15, 225),
            (enemies[6], 2, 235),
            (enemies[2], 6, 240),
            (enemies[4], 3, 250),
            (enemies[1], 14, 250),
            (enemies[5], 5, 260),
            (enemies[3], 10, 265),
            (enemies[6], 3, 270),
            (enemies[0], 12, 275),
            (enemies[5], 4, 285),
            (enemies[4], 2, 290),
            (enemies[6], 2, 295)
        };
    }
    
    void Update() {
        gameTime += Time.deltaTime;

        bool startedWave = false;
        while (waves.Count > 0 && gameTime > waves[0].Item3) {
            (GameObject, int, float) nextEnemyGroup = waves[0];
            waves.RemoveAt(0);
            startedWave = true;
            for (int i = 0; i < nextEnemyGroup.Item2; i++) {
                spawner.AddToQueue(nextEnemyGroup.Item1);
            }
        }
        if (startedWave) AudioManager.Play(AudioCue.WaveStart);
    }
}
