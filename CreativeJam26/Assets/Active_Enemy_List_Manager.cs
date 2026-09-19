using UnityEngine;
using System.Collections.Generic;

// SCOPE: maintains a list of active enemies with their progress through the map. Useful for targeting furthest along enemy in range
public class Active_Enemy_List_Manager : MonoBehaviour
{
    public List<GameObject> activeEnemies;

    void Awake() {
        activeEnemies = new List<GameObject>();
    }
    
    void Update() {
        if (activeEnemies.Count > 0) {
            // bubble sort to maintain list accuracy (only runs one pass per frame, but if it gets slightly behind that's ok)
            for (int i = 0; i < activeEnemies.Count - 1; i++) {
                if (activeEnemies[i].GetComponent<Enemy_Controller>().progress > activeEnemies[i + 1].GetComponent<Enemy_Controller>().progress) {
                    (activeEnemies[i], activeEnemies[i + 1]) = (activeEnemies[i + 1], activeEnemies[i]);
                }
            }
            
            // remove enemies that have reached the end
            while (activeEnemies.Count > 0 && activeEnemies[^1].GetComponent<Enemy_Controller>().progress >= .99f) {
                activeEnemies.RemoveAt(activeEnemies.Count - 1);
            }
            // remove enemies that were killed
            for (int i = 0; i < activeEnemies.Count; i++) {
                if (activeEnemies[i].GetComponent<Enemy_Controller>().health <= 0) {
                    activeEnemies.RemoveAt(i);
                }
            }
        }
    }

    public void AddEnemyToList(GameObject newEnemy) {
        activeEnemies.Add(newEnemy);
    }
}