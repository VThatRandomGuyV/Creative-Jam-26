using System.Collections;
using UnityEngine;
using System.Collections.Generic;

// SCOPE: spawns enemies from a queue. provides script to populate queue (used by wave manager)
public class Enemy_Spawner : MonoBehaviour {

    public float spawnRate;
    private Queue<GameObject> enemySpawnQueue;
    private float timeSinceSpawn;
    private Active_Enemy_List_Manager AELM;
    public Spline current_path;
    public int int_layer;

    void Awake() {
        enemySpawnQueue = new Queue<GameObject>();
        AELM = GetComponent<Active_Enemy_List_Manager>();
    }

    // Update is called once per frame
    void Update() {
        timeSinceSpawn += Time.deltaTime;
        if (timeSinceSpawn > spawnRate) {
            if (enemySpawnQueue.Count == 0) return;
            timeSinceSpawn = 0;
            GameObject nextObject = enemySpawnQueue.Dequeue();
            nextObject.GetComponent<Enemy_Controller>().spline = current_path;
            nextObject.layer = int_layer;
            GameObject newObject = Instantiate(nextObject);
            AELM.AddEnemyToList(newObject);
        }
    }

    public void AddToQueue(GameObject obj) {
        enemySpawnQueue.Enqueue(obj);
    }
}
