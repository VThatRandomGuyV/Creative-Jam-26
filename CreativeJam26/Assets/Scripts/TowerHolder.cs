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
    //[SerializeField] private GameObject towerMenu;
    [SerializeField] private Tower tower;
    private Color startColor;

    private void Start(){
        //towerMenu.SetActive(false);

        startColor = sr.color;
    }
    private void OnMouseEnter(){
        sr.color = hoverColor;
    }
    private void OnMouseExit(){
        sr.color = startColor;
    }

    public void OnMouseDown()
    {
        
    }
    private void SpawnTower1() {
            //towerMenu.SetActive(true);
            //Debug.Log( this.transform.position);
            //towerMenu.transform.position = this.transform.position;
            //if (tower == null) {
            //Debug.Log(TowerManagement.main.GetSelectedTower().name);
            Tower towerToBuild = tower;
            Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
            this.gameObject.SetActive(false);
    }
}
    // Update is called once per frame
    //void Update()
    //{
        
    //}