using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    private GameObject currentTower = null;
    private int towerLevel = 1;
    public bool baby;

    private void OnMouseDown()
    {
        // Pass 'this' slot and whether it currently has a tower
        bool hasTower = currentTower != null;
        TowerMenuManager.Instance.OpenMenu(this, transform.position, hasTower);
    }

    public void BuildTower(GameObject towerPrefab)
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
