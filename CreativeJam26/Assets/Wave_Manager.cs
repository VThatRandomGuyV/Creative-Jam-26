using System.Collections.Generic;
using UnityEngine;

// SCOPE: allows the user to create custom waves that are then passed to the enemy spawner when the game starts
public class Wave_Manager : MonoBehaviour {

    private List<(GameObject, int, float)> waves;
    private float gameTime;
    public GameObject enemySpawner;
    private Enemy_Spawner spawner;

    // enemy types
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;

    void Awake() {
        spawner = enemySpawner.GetComponent<Enemy_Spawner>();

        //enemy1 = Resources.Load<GameObject>("Enemy1");
        //enemy2 = Resources.Load<GameObject>("Enemy2");
        
        waves = new List<(GameObject, int, float)> { // enemy prefab, number to spawn, time it spawns
            (enemy1, 10, 5),
            (enemy2, 3, 5),
            (enemy2,  5, 10)
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
