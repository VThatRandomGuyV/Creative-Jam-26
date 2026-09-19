using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required namespace
using UnityEditor;


public class TowerManagement : MonoBehaviour
{
    public static TowerManagement main;

    [Header("References")]
    [SerializeField] private Tower[] towers;
    [SerializeField] int selectedTower; 
    [SerializeField] TowerHolder selectedHolder;

    void Awake(){
        main = this;
    }

    public Tower GetSelectedTower() {
        Debug.Log(selectedTower);
        return towers[selectedTower];
    }
    // Update is called once per frame
   
    public void SetSelectedTower(int _selectedTower)
    {
        selectedTower = _selectedTower;
    }
    public void setSelectedHolder(TowerHolder _selectedHolder)
    {
        selectedHolder = _selectedHolder;
    }
    public void BuildSelectedTower() {
        Debug.Log("BBB");
       selectedHolder.BuildTower();
    }
    public void DebugLog()
    {
        Debug.Log("CCCC");
    }
    
        
    




}
