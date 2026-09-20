using System.Threading.Tasks;
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    private GameObject currentTower = null;
    private int towerLevel = 1;
    public bool baby;
    public GameObject ruin; 
    
    private void OnMouseDown()
    {
        // Pass 'this' slot and whether it currently has a tower
        bool hasTower = currentTower != null;
        TowerMenuManager.Instance.OpenMenu(this, transform.position, hasTower);
    }

    public async Task BuildTower(GameObject towerPrefab)
    {
        if (currentTower == null)
        {
            
            // Spawn the tower at the slot's position
            currentTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
            towerLevel = 1;
            if (baby)
            {
                currentTower.GetComponent<TowerOperation>().targetMask = 128;
            }
            else
            {
                if (currentTower.GetComponent<TowerOperation>().quality == 0)
                {
                   ruin = Instantiate(ruin, transform.position, Quaternion.identity);
                   ruin.transform.position += new Vector3 (9,0,0); 
                }
                else if (currentTower.GetComponent<TowerOperation>().quality == 2)
                {

                    Vector3 twinOffset = new Vector3(8,0,0) ;
                    GameObject oldTower = Instantiate(towerPrefab, transform.position + twinOffset, Quaternion.identity);
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
            // Add visual or stat scaling here
        }
    }
}
