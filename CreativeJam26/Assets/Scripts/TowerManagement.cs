using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required namespace
using UnityEditor;


public class TowerManagement : MonoBehaviour
{
    public static TowerManagement main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    private int selectedTower = 0;
    void Awake(){
        main = this;
    }

    public GameObject GetSelectedTower() {
        return towerPrefabs[selectedTower];
    }
    // Update is called once per frame
   


}
