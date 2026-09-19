using System.Runtime.Serialization;
using UnityEngine;

public class BuildButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject towerManager;
    [SerializeField] private int relevantTower;

    
    public TowerManagement other;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildPlannedTower()
    {
        Debug.Log("aa");
        other.BuildSelectedTower();
    }
}
