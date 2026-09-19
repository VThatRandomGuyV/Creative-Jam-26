using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    [SerializeField] private GameObject towerMenu;
    [SerializeField] private GameObject towerManager;
    private GameObject tower;
    private Color startColor;

    private void Start(){
        towerMenu.SetActive(false);

        startColor = sr.color;
    }
    private void OnMouseEnter(){
        sr.color = hoverColor;
    }
    private void OnMouseExit(){
        sr.color = startColor;
    }
    private void OnMouseDown() {
        towerMenu.SetActive(true);
//        Debug.Log( this.transform.position);
        Vector3 menuOffset =new Vector3 (490,273,0);
        towerMenu.transform.position = this.transform.position ;
        TowerManagement.main.setSelectedHolder(this);     
        
/*  
        if (tower == null) {
        Debug.Log(TowerManagement.main.GetSelectedTower().name);
        Tower towerToBuild = TowerManagement.main.GetSelectedTower();
        tower = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        }
  */
    }
    public void BuildTower()
    {

        Tower towerToBuild = TowerManagement.main.GetSelectedTower();
        tower = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
