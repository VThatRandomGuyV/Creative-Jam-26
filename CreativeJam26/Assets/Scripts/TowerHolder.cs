using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    [SerializeField] private GameObject towerMenu;
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
        Debug.Log( this.transform.position);
        towerMenu.transform.position = this.transform.position;
        if (tower == null) {
        Debug.Log(TowerManagement.main.GetSelectedTower().name);
        Tower towerToBuild = TowerManagement.main.GetSelectedTower();
        tower = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
