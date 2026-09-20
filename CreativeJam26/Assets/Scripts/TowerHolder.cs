using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSlot : MonoBehaviour
{
    private GameObject currentTower = null;
    private int towerLevel = 1;
    public bool baby;
    public GameObject ruin;
    private void OnMouseUpAsButton()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        bool hasTower = currentTower != null;
        TowerMenuManager.Instance.OpenMenu(this, transform.position, hasTower);
    }

    public void BuildTower(GameObject towerPrefab)
    {
        
        if (currentTower == null)
        {
            currentTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
            towerLevel = 1;
            if (baby)
            {
                currentTower.GetComponent<TowerOperation>().targetMask = 128; 
            }
            else
            {
                Debug.Log("aaa" + (currentTower.GetComponent<TowerOperation>().quality) );
                if (currentTower.GetComponent<TowerOperation>().quality ==  0)
                {
                   ruin = Instantiate(ruin, transform.position, Quaternion.identity);
                   ruin.transform.position += new Vector3 (9,0,0); 
                }
                if (currentTower.GetComponent<TowerOperation>().quality ==  2)
                {
                    Vector3 twinOffest = new Vector3(8,0,0);
                    GameObject oldTower = Instantiate(towerPrefab, transform.position + twinOffest, Quaternion.identity);
                    oldTower.GetComponent<TowerOperation>().targetMask = 128;
                }
 
            }
            Debug.Log($"{gameObject.name}: Tower built!");
            this.gameObject.SetActive(false);
        }
    }

    public void UpgradeTower()
    {
        if (currentTower != null)
        {
            towerLevel++;
            Debug.Log($"{gameObject.name}: Tower upgraded to Level {towerLevel}!");
        }
    }
}